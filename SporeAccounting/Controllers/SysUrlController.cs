using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;
using System.Net;
using SporeAccounting.BaseModels.ViewModel.Response;

namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 接口URL控制器
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    public class SysUrlController : ControllerBase
    {
        private readonly ISysUrlServer _sysUrlServer;
        private readonly IMapper _mapper;

        public SysUrlController(ISysUrlServer sysUrlServer, IMapper mapper)
        {
            _sysUrlServer = sysUrlServer;
            _mapper = mapper;
        }

        /// <summary>
        /// 新增URL
        /// </summary>
        /// <param name="sysUrlViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public ActionResult<ResponseData<bool>> Add([FromBody] SysUrlViewModel sysUrlViewModel)
        {
            try
            {
                bool isExist = _sysUrlServer.IsExist(sysUrlViewModel.Url);
                if (isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, $"URL{sysUrlViewModel.Url}已存在"));
                }

                SysUrl sysUrl = _mapper.Map<SysUrl>(sysUrlViewModel);
                sysUrl.CreateDateTime = DateTime.Now;
                //TODO：这里暂时写死，等权限和授权完成后再改为动态获取
                sysUrl.CreateUserId = "08f35c1e-117f-431d-979d-9e51e29b0b7d";
                _sysUrlServer.Add(sysUrl);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常"));
            }
        }

        /// <summary>
        /// 删除URL（逻辑删除）
        /// </summary>
        /// <param name="urlId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{urlId}")]
        public ActionResult<ResponseData<bool>> Delete([FromRoute] string urlId)
        {
            try
            {
                bool isExist = _sysUrlServer.IsExistById(urlId);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, $"URL{urlId}不存在"));
                }

                bool canDelete = _sysUrlServer.CanDelete(urlId);
                if (!canDelete)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, $"URL{urlId}不可删除"));
                }

                SysUrl sysUrl = _sysUrlServer.Query(urlId);
                sysUrl.DeleteDateTime = DateTime.Now;
                sysUrl.IsDeleted = true;
                //TODO：这里暂时写死，等权限和授权完成后再改为动态获取
                sysUrl.DeleteUserId = "08f35c1e-117f-431d-979d-9e51e29b0b7d";
                _sysUrlServer.Delete(sysUrl);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常"));
            }
        }

        /// <summary>
        /// 修改URL
        /// </summary>
        /// <param name="sysUrlViewModel"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Edit")]
        public ActionResult<ResponseData<bool>> Edit([FromBody] SysUrlEditViewModel sysUrlViewModel)
        {
            try
            {
                bool isExist = _sysUrlServer.IsExistById(sysUrlViewModel.Id);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.BadRequest, $"URL{sysUrlViewModel.Id}不存在"));
                }

                SysUrl sysUrl = _sysUrlServer.Query(sysUrlViewModel.Id);
                sysUrl = _mapper.Map(sysUrlViewModel, sysUrl);
                sysUrl.UpdateDateTime = DateTime.Now;
                //TODO：这里暂时写死，等权限和授权完成后再改为动态获取
                sysUrl.UpdateUserId = "08f35c1e-117f-431d-979d-9e51e29b0b7d";
                _sysUrlServer.Update(sysUrl);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常"));
            }
        }

        /// <summary>
        /// 查询URL列表（分页查询）
        /// </summary>
        /// <param name="sysUrlPage"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Query")]
        public ActionResult<ResponseData<PageResponseViewModel<SysUrlQueryViewModel>>> Query(
            [FromBody] SysUrlPageViewModel sysUrlPage)
        {
            try
            {
                var (rowCount, pageCount, sysUrls) = _sysUrlServer
                    .GetByPage(sysUrlPage);
                List<SysUrlQueryViewModel> sysUrlViewModels = _mapper.Map<List<SysUrlQueryViewModel>>(sysUrls);
                PageResponseViewModel<SysUrlQueryViewModel> pageResponseViewModel =
                    new PageResponseViewModel<SysUrlQueryViewModel>
                    {
                        RowCount = rowCount,
                        PageCount = pageCount,
                        Data = sysUrlViewModels
                    };
                return Ok(new ResponseData<PageResponseViewModel<SysUrlQueryViewModel>>(HttpStatusCode.OK,
                    data: pageResponseViewModel));

            }
            catch (Exception e)
            {
                return Ok(new ResponseData<SysUrlViewModel>(HttpStatusCode.InternalServerError, "服务器异常"));
            }
        }
        /// <summary>
        /// 查询具体某个URL信息
        /// </summary>
        /// <param name="urlId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("QueryById")]
        public ActionResult<ResponseData<SysUrlQueryViewModel>> QueryById([FromQuery] string urlId)
        {
            try
            {
                SysUrl sysUrl = _sysUrlServer.Query(urlId);
                SysUrlQueryViewModel sysUrlQueryViewModel = _mapper.Map<SysUrlQueryViewModel>(sysUrl);
                return Ok(new ResponseData<SysUrlQueryViewModel>(HttpStatusCode.OK, data: sysUrlQueryViewModel));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<SysUrlQueryViewModel>(HttpStatusCode.InternalServerError, "服务器异常"));
            }
        }

    }
}
