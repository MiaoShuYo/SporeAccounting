using Microsoft.AspNetCore.Mvc;
using SP.IdentityService.Models.Request;
using SP.IdentityService.Models.Response;
using SP.IdentityService.Service;

namespace SP.IdentityService.Controllers;

/// <summary>
/// 用户控制器
/// </summary>
[Route("/api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// 用户控制器构造函数
    /// </summary>
    /// <param name="userService"></param>
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <param name="id">用户id</param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetUser([FromRoute] long id)
    {
        var result = await _userService.GetUserInfo(id);
        return Ok(result);
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    /// <param name="page">分页查询参数</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetUsers([FromQuery] UserPageRequest page)
    {
        var result = await _userService.GetUserList(page);
        return Ok(result);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id">用户id</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser([FromRoute] long id)
    {
        await _userService.DeleteUser(id);
        return Ok();
    }

    /// <summary>
    /// 禁用/启用用户
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="isDisabled">是否禁用</param>
    /// <returns></returns>
    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateUserStatus([FromRoute] long id, [FromBody] bool isDisabled)
    {
        await _userService.DisableUser(id, isDisabled);
        return Ok();
    }

    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="user">用户信息</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser([FromRoute] long id, [FromBody] UserUpdateRequest user)
    {
        await _userService.UpdateUser(id, user);
        return Ok();
    }
}