using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Models.ViewModels;
using SporeAccounting.Server.Interface;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace SporeAccounting.Controllers
{
    /// <summary>
    /// 角色接口
    /// </summary>
    [Route("api/[controller]/")]
    [ApiController]
    [Authorize(Roles = "Administrator")]
    public class SysRoleController : BaseController
    {
        private readonly ISysRoleServer _sysRoleServer;
        private readonly IMapper _mapper;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sysRoleServer"></param>
        /// <param name="mapper"></param>
        public SysRoleController(ISysRoleServer sysRoleServer, IMapper mapper)
        {
            _sysRoleServer = sysRoleServer;
            _mapper = mapper;
        }

        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public ActionResult<ResponseData<bool>> Add([FromBody] SysRoleViewModel role)
        {
            try
            {
                bool isExist = _sysRoleServer.IsExistByRoleName(role.RoleName);
                if (isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"角色{role.RoleName}已存在！", false));
                }

                SysRole dbRole = _mapper.Map<SysRole>(role);
                dbRole.CanDelete = true;
                dbRole.CreateUserId = GetUserId();
                _sysRoleServer.Add(dbRole);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }

        /// <summary>
        /// 删除角色（逻辑删除）
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Remove/{roleId}")]
        public ActionResult<ResponseData<bool>> Remove([FromRoute] string roleId)
        {
            try
            {
                bool isExist = _sysRoleServer.IsExistById(roleId);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, $"角色不存在！", false));
                }
                bool canDelete = _sysRoleServer.CanDelete(roleId);
                if (!canDelete)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"角色{roleId}不可删除！", false));
                }
                _sysRoleServer.Delete(roleId, GetUserId());
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="roleView"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Edit")]
        public ActionResult<ResponseData<bool>> Edit([FromBody] SysRoleEditViewModel roleView)
        {
            try
            {
                //判断角色是否存在
                bool isExist = _sysRoleServer.IsExistById(roleView.RoleId);
                if (!isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.NotFound, $"角色不存在！", false));
                }

                //判断角色名字是否重复
                isExist = _sysRoleServer.IsRepeat(roleView.RoleId, roleView.RoleName);
                if (isExist)
                {
                    return Ok(new ResponseData<bool>(HttpStatusCode.Conflict, $"角色{roleView.RoleName}已存在！", false));
                }

                SysRole role = _mapper.Map<SysRole>(roleView);
                //TODO：这里暂时写死，等权限和授权完成后再改为动态获取
                role.UpdateUserId = GetUserId();
                _sysRoleServer.Update(role);
                return Ok(new ResponseData<bool>(HttpStatusCode.OK, data: true));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }

        /// <summary>
        /// 根据角色名查询（分页查询）
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Query")]
        public ActionResult<ResponseData<List<SysRoleQueryViewModel>>> Query([FromQuery] string? roleName)
        {
            try
            {
                List<SysRole> roles = _sysRoleServer.Query(roleName);
                List<SysRoleQueryViewModel> rolesQuery = _mapper.Map<List<SysRoleQueryViewModel>>(roles);
                return Ok(new ResponseData<List<SysRoleQueryViewModel>>(HttpStatusCode.OK, data: rolesQuery));
            }
            catch (Exception e)
            {
                return Ok(new ResponseData<bool>(HttpStatusCode.InternalServerError, "服务器异常", false));
            }
        }
    }
}
