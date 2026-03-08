using Microsoft.AspNetCore.Mvc;

namespace SP.FinanceService.Controllers;

/// <summary>
/// 预算控制器
/// </summary>
[Route("api/health")]
[ApiController]
public class HealthController: ControllerBase
{
    /// <summary>
    /// 健康检查接口
    /// </summary>s
    /// <returns>服务状态</returns>
    [HttpGet]
    public ActionResult<string> GetHealth()
    {
        return Ok("Healthy");
    }
}