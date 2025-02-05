using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using SporeAccounting.BaseModels.ViewModel.Response;


namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 收支分类接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Consumer,Administrator")]
    public class IncomeExpenditureClassificationController : BaseController
    {
        private readonly IIncomeExpenditureClassificationServer _incomeExpenditureClassificationService;
        private readonly IMapper _mapper;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="incomeExpenditureClassificationService"></param>
        /// <param name="mapper"></param>
        public IncomeExpenditureClassificationController(
            IIncomeExpenditureClassificationServer incomeExpenditureClassificationService, IMapper mapper)
        {
            _incomeExpenditureClassificationService = incomeExpenditureClassificationService;
            _mapper = mapper;
        }

        /// <summary>
        /// 查询父分类下的子分类
        /// </summary>
        /// <param name="parentClassificationId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Query/{parentClassificationId}")]
        public ActionResult<ResponseData<List<IncomeExpenditureClassificationInfoViewModel>>> Query(
            [FromRoute] string parentClassificationId)
        {
            try
            {
                List<IncomeExpenditureClassification> classifications =
                    _incomeExpenditureClassificationService.Query(parentClassificationId).ToList();

                List<IncomeExpenditureClassificationInfoViewModel>
                    classificationInfoViewModels =
                        _mapper.Map<List<IncomeExpenditureClassificationInfoViewModel>>(classifications);

                return Ok(new ResponseData<List<IncomeExpenditureClassificationInfoViewModel>>(HttpStatusCode.OK,
                    data: classificationInfoViewModels));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<List<string>>(HttpStatusCode.InternalServerError, "服务器异常", null));
            }
        }
        
        /// <summary>
        /// 查询父级分类
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("QueryParent")]
        public ActionResult<ResponseData<List<IncomeExpenditureClassificationInfoViewModel>>>
            QueryParent()
        {
            try
            {
                //查询父级分类
                List<IncomeExpenditureClassification> classifications =
                    _incomeExpenditureClassificationService.QueryParent().ToList();
                //转换为视图模型
                List<IncomeExpenditureClassificationInfoViewModel>
                    classificationInfoViewModels =
                        _mapper.Map<List<IncomeExpenditureClassificationInfoViewModel>>(classifications);

                return Ok(new ResponseData<List<IncomeExpenditureClassificationInfoViewModel>>(HttpStatusCode.OK,
                    data: classificationInfoViewModels));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<List<string>>(HttpStatusCode.InternalServerError, "服务器异常", null));
            }
        }

        /// <summary>
        /// 根据收支类型查询分类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("QueryByType")]
        public ActionResult<ResponseData<List<IncomeExpenditureClassificationInfoViewModel>>>
            QueryByType([FromQuery] IncomeExpenditureTypeEnmu type)
        {
            try
            {
                string userId = GetUserId();
                List<IncomeExpenditureClassification> classifications =
                    _incomeExpenditureClassificationService.Query(type, userId).ToList();

                List<IncomeExpenditureClassificationInfoViewModel>
                    classificationInfoViewModels =
                        _mapper.Map<List<IncomeExpenditureClassificationInfoViewModel>>(classifications);

                return Ok(new ResponseData<List<IncomeExpenditureClassificationInfoViewModel>>(HttpStatusCode.OK,
                    data: classificationInfoViewModels));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<List<string>>(HttpStatusCode.InternalServerError, "服务器异常", null));
            }
        }

        /// <summary>
        /// 分页查询收支分类
        /// </summary>
        /// <param name="incomeExpenditureClassificationPageViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Query")]
        public ActionResult<ResponseData<PageResponseViewModel<IncomeExpenditureClassificationInfoViewModel>>>
            Query([FromBody] IncomeExpenditureClassificationPageViewModel incomeExpenditureClassificationPageViewModel)
        {
            try
            {
                (int rowCount, int pageCount, List<IncomeExpenditureClassification> incomeExpenditureClassifications) =
                    _incomeExpenditureClassificationService.GetByPage(incomeExpenditureClassificationPageViewModel);

                List<IncomeExpenditureClassificationInfoViewModel> classificationInfoViewModels =
                    _mapper.Map<List<IncomeExpenditureClassificationInfoViewModel>>(incomeExpenditureClassifications);

                PageResponseViewModel<IncomeExpenditureClassificationInfoViewModel> pageResponse =
                    new PageResponseViewModel<IncomeExpenditureClassificationInfoViewModel>
                    {
                        Data = classificationInfoViewModels,
                        PageCount = pageCount,
                        RowCount = rowCount
                    };

                return Ok(new ResponseData<PageResponseViewModel<IncomeExpenditureClassificationInfoViewModel>>(
                    HttpStatusCode.OK, data: pageResponse));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }

        /// <summary>
        /// 新增收支分类
        /// </summary>
        /// <param name="classificationAddViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public ActionResult<ResponseData<bool>> Add(
            [FromBody] IncomeExpenditureClassificationViewModel classificationAddViewModel)
        {
            try
            {
                //是否存在
                bool isExist =
                    _incomeExpenditureClassificationService.IsExist(classificationAddViewModel.Name, GetUserId());
                if (isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict,
                        $"分类{classificationAddViewModel.Name}已存在！", false));
                }

                if (!string.IsNullOrEmpty(classificationAddViewModel.ParentClassificationId))
                {
                    //判断类型是否和父级的类型一样
                    IncomeExpenditureClassification parentClassification =
                        _incomeExpenditureClassificationService.QueryById(classificationAddViewModel
                            .ParentClassificationId);
                    if (parentClassification.Type != classificationAddViewModel.Type)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.Conflict,
                            $"分类{classificationAddViewModel.Name}的类型和父级类型不一致！", false));
                    }

                    //判断父级是否是子集
                    if (parentClassification.ParentIncomeExpenditureClassificationId != null)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"子分类不能再创建子类！", false));
                    }
                }

                IncomeExpenditureClassification classification =
                    _mapper.Map<IncomeExpenditureClassification>(classificationAddViewModel);
                classification.CreateUserId = GetUserId();
                classification.CanDelete = true;
                classification.CreateDateTime = DateTime.Now;
                _incomeExpenditureClassificationService.Add(classification);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }

        /// <summary>
        /// 删除收支分类
        /// </summary>
        /// <param name="classificationId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{classificationId}")]
        public ActionResult<ResponseData<bool>> Delete([FromRoute] string classificationId)
        {
            try
            {
                bool isExist = _incomeExpenditureClassificationService.IsExist(classificationId);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, $"分类不存在！", false));
                }

                bool canDelete = _incomeExpenditureClassificationService.CanDelete(classificationId);
                if (!canDelete)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"分类{classificationId}不可删除！", false));
                }

                //是否存在子类型
                bool hasChild = _incomeExpenditureClassificationService.HasChild(classificationId);
                if (hasChild)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"分类{classificationId}存在子分类，不允许删除！",
                        false));
                }

                _incomeExpenditureClassificationService.Delete(classificationId);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }

        /// <summary>
        /// 修改收支分类
        /// </summary>
        /// <param name="classificationViewModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Update")]
        public ActionResult<ResponseData<bool>> Update(
            [FromBody] IncomeExpenditureClassificationEditViewModel classificationViewModel)
        {
            try
            {
                //是否存在
                bool isExist = _incomeExpenditureClassificationService.IsExist(classificationViewModel.Id);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, $"分类不存在！", false));
                }

                //判断类型是否和父级的类型一样
                if (!string.IsNullOrEmpty(classificationViewModel.ParentClassificationId))
                {
                    //判断类型是否和父级的类型一样
                    IncomeExpenditureClassification parentClassification =
                        _incomeExpenditureClassificationService.QueryById(
                            classificationViewModel.ParentClassificationId);
                    if (parentClassification.Type != classificationViewModel.Type)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.Conflict,
                            $"分类{classificationViewModel.Name}的类型和父级类型不一致！", false));
                    }

                    //判断父级是否是子集
                    if (parentClassification.ParentIncomeExpenditureClassificationId != null)
                    {
                        return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"子分类不能再创建子类！", false));
                    }
                }

                //不能将类型修改为其他，也不能把其他类型修改为别的类型
                IncomeExpenditureClassification classificationOld =
                    _incomeExpenditureClassificationService.QueryById(classificationViewModel.Id);
                if (classificationOld.Type == IncomeExpenditureTypeEnmu.Other)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"分类{classificationOld.Name}不允许修改！",
                        false));
                }

                if (classificationViewModel.Type == IncomeExpenditureTypeEnmu.Other)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict,
                        $"分类{classificationViewModel.Name}不允许修改为其他分类！", false));
                }

                IncomeExpenditureClassification classification =
                    _mapper.Map<IncomeExpenditureClassification>(classificationViewModel);
                classification.UpdateDateTime = DateTime.Now;
                classification.UpdateUserId = GetUserId();
                _incomeExpenditureClassificationService.Update(classification);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }
    }
}