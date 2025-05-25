using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using SP.Common.ExceptionHandling.Exceptions;
using SP.Common.Message.Model;
using SP.IdentityService.Models.Entity;
using SP.IdentityService.Models.Request;
using SP.IdentityService.Service;

namespace SP.IdentityService.Controllers;

/// <summary>
/// 授权控制器
/// </summary>
[Route("connect")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    /// <summary>
    /// 授权控制器构造函数
    /// </summary>
    /// <param name="authorizationService"></param>
    public AuthorizationController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    /// <summary>
    /// 令牌端点 - 获取访问令牌
    /// </summary>
    /// <remarks>
    /// 请求示例:
    /// 
    ///     POST /connect/token
    ///     Content-Type: application/x-www-form-urlencoded
    ///     
    ///     grant_type=password&amp;username=admin&amp;password=123*asdasd&amp;scope=api offline_access
    ///
    ///     或者刷新令牌:
    ///     
    ///     grant_type=refresh_token&amp;refresh_token=YOUR_REFRESH_TOKEN&amp;scope=api
    ///     
    ///     或者客户端凭证模式:
    ///     
    ///     grant_type=client_credentials&amp;client_id=YOUR_CLIENT_ID&amp;client_secret=YOUR_CLIENT_SECRET&amp;scope=api
    ///
    /// 注意：
    /// 1. 必须使用表单（form-data）方式提交，Content-Type为application/x-www-form-urlencoded
    /// 2. 不要将参数放在URL查询字符串中
    /// 3. 在刷新令牌模式下，refresh_token必须放在请求体中，不能放在URL中
    /// 4. 客户端凭证模式适用于服务器到服务器的API调用，不关联特定用户
    /// </remarks>
    /// <returns>返回访问令牌信息</returns>
    /// <response code="200">返回访问令牌</response>
    /// <response code="400">请求格式不正确或不支持的授权类型</response>
    /// <response code="403">认证失败</response>
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> Token()
    {
        // 检查是否通过查询参数传递敏感信息
        if (Request.Query.Count > 0 &&
            (Request.Query.ContainsKey("refresh_token") || Request.Query.ContainsKey("password")))
        {
            throw new BadRequestException("不要在URL中包含敏感信息(refresh_token/password)，请使用表单提交方式");
        }

        var request = HttpContext.GetOpenIddictServerRequest();
        if (request == null)
        {
            throw new BadRequestException("请求格式不正确，请使用表单(application/x-www-form-urlencoded)提交");
        }

        // 处理资源所有者密码模式
        if (request.IsPasswordGrantType())
        {
            // 验证用户名和密码
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                throw new BusinessException("用户名或密码不能为空");
            }

            var principal =
                await _authorizationService.LoginByPasswordAsync(request.Username, request.Password,
                    request.GetScopes());
            // 确保 SignIn 方法只在授权端点调用
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // 处理刷新令牌
        if (request.IsRefreshTokenGrantType())
        {
            // 从 request 上下文中获取之前存储的身份验证票据
            var principal =
                (await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme))
                ?.Principal;

            var newPrincipal =
                await _authorizationService.RefreshTokenAsync(request.RefreshToken, request.GetScopes(), principal);

            return SignIn(newPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // 处理客户端凭证模式
        if (request.IsClientCredentialsGrantType())
        {
            var clientId = request.ClientId;
            if (string.IsNullOrEmpty(clientId))
            {
                throw new BusinessException("client_id不能为空");
            }

            var principal =
                await _authorizationService.HandleClientCredentialsAsync(clientId, request.GetScopes());
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // 不支持的授权类型
        return BadRequest(new
        {
            error = OpenIddictConstants.Errors.UnsupportedGrantType,
            error_description = "不支持的授权类型。"
        });
    }

    /// <summary>
    /// 注册用户
    /// </summary>
    /// <param name="user"></param>
    [HttpPost("register")]
    public async Task<ActionResult<long>> Register([FromBody] UserAddRequest user)
    {
        var result = await _authorizationService.AddUserAsync(user);
        return Ok(result);
    }

    /// <summary>
    /// 发送邮件
    /// </summary>
    /// <param name="email"></param>
    [HttpPost("emails/send")]
    public async Task<ActionResult> SendEmail([FromBody] SendEmailRequest email)
    {
        await _authorizationService.SendEmailAsync(email);
        return Ok();
    }

    /// <summary>
    /// 绑定邮箱
    /// </summary>
    /// <param name="verifyCode"></param>
    [HttpPost("email/bind")]
    public async Task<ActionResult> BindEmail([FromBody] VerifyCodeRequest verifyCode)
    {
        await _authorizationService.AddEmailAsync(verifyCode);
        return Ok();
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="resetPasswordRequest"></param>
    [HttpPut("password/reset")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
    {
        if (resetPasswordRequest == null || string.IsNullOrEmpty(resetPasswordRequest.Email) ||
            string.IsNullOrEmpty(resetPasswordRequest.ResetCode) ||
            string.IsNullOrEmpty(resetPasswordRequest.NewPassword))
        {
            throw new BadRequestException("请求参数不完整");
        }

        await _authorizationService.ResetPasswordAsync(resetPasswordRequest);
        return Ok();
    }
}