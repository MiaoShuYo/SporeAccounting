using System.Data;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.BaseModels.ViewModel.Response;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;

namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 角色可访问URL
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SysRoleUrlController : ControllerBase
    {
        private readonly ISysRoleUrlServer _sysRoleUrlServer;
        public SysRoleUrlController(ISysRoleUrlServer sysRoleUrlServer)
        {
            _sysRoleUrlServer = sysRoleUrlServer;
        }
        /// <summary>
        /// 根据角色Id查询角色可访问的URL
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Query/{roleId}")]
        public ActionResult<ResponseData<List<string>>> Query([FromRoute] string roleId)
        {
            try
            {
                List<string> urls = _sysRoleUrlServer.Query(roleId);
                return Ok(new ResponseData<List<string>>(HttpStatusCode.OK, data: urls));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<List<string>>(HttpStatusCode.InternalServerError, "服务器异常", null));
            }
        }
        [HttpPost]
        [Route("Query")]
        public ActionResult<ResponseData<PageResponseViewModel<SysRoleUrlQueryViewModel>>> Query([FromBody] SysRoleUrlPageViewModel sysRoleUrlPageViewModel)
        {
            try
            {
                (int rowCount, int pageCount, List<SysRoleUrl> sysRoleUrls) = _sysRoleUrlServer.GetByPage(sysRoleUrlPageViewModel);
                List<SysRoleUrlQueryViewModel> sysRoleUrlQuery = new List<SysRoleUrlQueryViewModel>();
                foreach (var item in sysRoleUrls)
                {
                    sysRoleUrlQuery.Add(new SysRoleUrlQueryViewModel
                    {
                        Id = item.Id,
                        RoleId = item.RoleId,
                        RoleName = item.Role.RoleName,
                        Url = item.Url
                    });
                }

                PageResponseViewModel<SysRoleUrlQueryViewModel> pageResponse =
                    new PageResponseViewModel<SysRoleUrlQueryViewModel>();
                pageResponse.Data = sysRoleUrlQuery;
                pageResponse.PageCount = pageCount;
                pageResponse.RowCount = rowCount;
                return Ok(new ResponseData<PageResponseViewModel<SysRoleUrlQueryViewModel>>(HttpStatusCode.OK, data: pageResponse));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }

        /// <summary>
        /// 新增角色可访问的URL
        /// </summary>
        /// <param name="roleUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public ActionResult<ResponseData<bool>> Add([FromBody] SysRoleUrlViewModel roleUrlViewModel)
        {
            try
            {
                bool isExist = _sysRoleUrlServer.IsExist(roleUrlViewModel.RoleId, roleUrlViewModel.Url);
                if (isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"角色{roleUrlViewModel.RoleId}已存在URL{roleUrlViewModel.Url}！", false));
                }

                SysRoleUrl roleUrl = new SysRoleUrl();
                roleUrl.RoleId = roleUrlViewModel.RoleId;
                roleUrl.Url = roleUrlViewModel.Url;
                //TODO：这里暂时写死，等权限和授权完成后再改为动态获取
                roleUrl.CreateUserId = "08f35c1e-117f-431d-979d-9e51e29b0b7d";
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
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{roleId}/{url}")]
        public ActionResult<ResponseData<bool>> Delete([FromRoute] string roleId, [FromRoute] string url)
        {
            try
            {
                bool isExist = _sysRoleUrlServer.IsExist(roleId, url);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"角色{roleId}不存在URL{url}！", false));
                }
                _sysRoleUrlServer.Delete(roleId, url);
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
                _sysRoleUrlServer.Edit(roleUrl.Id,roleUrl.RoleId, roleUrl.Url);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }
    }
}
