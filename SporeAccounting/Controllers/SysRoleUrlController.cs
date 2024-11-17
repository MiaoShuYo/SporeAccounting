using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.BaseModels.ViewModel.Response;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;
using System.Net;

namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 角色可访问URL
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SysRoleUrlController : BaseController
    {
        private readonly ISysRoleUrlServer _sysRoleUrlServer;
        private readonly IMapper _mapper;
        public SysRoleUrlController(ISysRoleUrlServer sysRoleUrlServer,IMapper mapper)
        {
            _sysRoleUrlServer = sysRoleUrlServer;
            _mapper = mapper;
        }
        /// <summary>
        /// 根据角色Id查询角色可访问的URL
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Query/{roleId}")]
        public ActionResult<ResponseData<List<SysRoleUrlInfoVideModel>>> Query([FromRoute] string roleId)
        {
            try
            {
                List<SysRoleUrlInfoVideModel> roleUrlInfo = _sysRoleUrlServer.Query(roleId);
                return Ok(new ResponseData<List<SysRoleUrlInfoVideModel>>(HttpStatusCode.OK, data: roleUrlInfo));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<List<string>>(HttpStatusCode.InternalServerError, "服务器异常", null));
            }
        }
        [HttpPost]
        [Route("Query")]
        public ActionResult<ResponseData<PageResponseViewModel<SysRoleUrlInfoVideModel>>> Query([FromBody] SysRoleUrlPageViewModel sysRoleUrlPageViewModel)
        {
            try
            {
                (int rowCount, int pageCount, List<SysRoleUrlInfoVideModel> sysRoleUrls) = _sysRoleUrlServer.GetByPage(sysRoleUrlPageViewModel);
                PageResponseViewModel<SysRoleUrlInfoVideModel> pageResponse =
                    new PageResponseViewModel<SysRoleUrlInfoVideModel>();
                pageResponse.Data = sysRoleUrls;
                pageResponse.PageCount = pageCount;
                pageResponse.RowCount = rowCount;
                return Ok(new ResponseData<PageResponseViewModel<SysRoleUrlInfoVideModel>>(HttpStatusCode.OK, data: pageResponse));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }

        /// <summary>
        /// 新增角色可访问的URL
        /// </summary>
        /// <param name="roleUrlViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public ActionResult<ResponseData<bool>> Add([FromBody] SysRoleUrlViewModel roleUrlViewModel)
        {
            try
            {
                bool isExist = _sysRoleUrlServer.IsExist(roleUrlViewModel.RoleId, roleUrlViewModel.UrlId);
                if (isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"角色{roleUrlViewModel.RoleId}已存在{roleUrlViewModel.UrlId}！", false));
                }

                SysRoleUrl roleUrl = _mapper.Map<SysRoleUrl>(roleUrlViewModel);
                //TODO：这里暂时写死，等权限和授权完成后再改为动态获取
                roleUrl.CreateUserId = GetUserId();
                roleUrl.CreateDateTime = DateTime.Now;
                _sysRoleUrlServer.Add(roleUrl);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }
        /// <summary>
        /// 删除角色可访问的URL
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="urlId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{roleId}/{urlId}")]
        public ActionResult<ResponseData<bool>> Delete([FromRoute] string roleId, [FromRoute] string urlId)
        {
            try
            {
                bool isExist = _sysRoleUrlServer.IsExist(roleId, urlId);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, $"角色{roleId}不存在URL{urlId}！", false));
                }
                bool isDelete= _sysRoleUrlServer.IsDelete(roleId, urlId);
                if (!isDelete)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"角色{roleId}不允许删除URL{urlId}！", false));
                }
                _sysRoleUrlServer.Delete(roleId, urlId);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }
        /// <summary>
        /// 修改角色可访问的URL
        /// </summary>
        /// <param name="roleUrl"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Edit")]
        public ActionResult<ResponseData<bool>> Edit([FromBody] SysRoleUrlViewModel roleUrl)
        {
            try
            {
                bool isExist = _sysRoleUrlServer.IsExist(roleUrl.RoleId, roleUrl.UrlId);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"角色{roleUrl.RoleId}存在{roleUrl.UrlId}！", false));
                }
                _sysRoleUrlServer.Edit(roleUrl.Id, roleUrl.RoleId, roleUrl.UrlId);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }
    }
}
