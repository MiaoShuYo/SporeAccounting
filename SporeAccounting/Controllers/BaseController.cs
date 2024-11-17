using Microsoft.AspNetCore.Mvc;

namespace SporeAccounting.Controllers;

/// <summary>
/// 控制器基类
/// </summary>
public class BaseController:ControllerBase
{
    public string GetUserId()
    {
        HttpContext.Request.Headers.TryGetValue("UserId", out var userId);
        return userId.ToString();
    }
}