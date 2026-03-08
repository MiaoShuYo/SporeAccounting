using Microsoft.AspNetCore.Mvc;
using SP.IdentityService.Models.Request;
using SP.IdentityService.Services;

namespace SP.IdentityService.Controllers;

/// <summary>
/// 图形验证码
/// </summary>
[ApiController]
[Route("api/captcha")] 
public class CaptchaController : ControllerBase
{
    private readonly ICaptchaService _captchaService;

    public CaptchaController(ICaptchaService captchaService)
    {
        _captchaService = captchaService;
    }

    /// <summary>
    /// 生成验证码图片（Base64）
    /// </summary>
    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        string? ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await _captchaService.CreateAsync(ip);
        return Ok(result);
    }

    /// <summary>
    /// 校验验证码
    /// </summary>
    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] CaptchaVerifyRequest request)
    {
        bool ok = await _captchaService.VerifyAsync(request.Token, request.Code, true);
        return Ok(new { success = ok });
    }
}


