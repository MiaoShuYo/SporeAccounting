using Microsoft.AspNetCore.Mvc;
using SP.MLService.Services;
using SP.Common;
using SP.MLService.Models.Response;
using SP.MLService.Models.Request;

namespace SP.MLService.Controllers;

/// <summary>
/// AI使用记录控制器
/// </summary>
[ApiController]
[Route("api/ai-usage-record")]
public class AIUsageRecordController : ControllerBase
{
    /// <summary>
    /// AI使用记录接口
    /// </summary>
    private readonly IAIUsageRecordServer _aiUsageRecordServer;
    /// <summary>
    /// 上下文会话
    /// </summary>
    private readonly ContextSession _contextSession;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="aiUsageRecordServer">AI使用记录接口</param>
    /// <param name="contextSession">上下文会话</param>
    public AIUsageRecordController(IAIUsageRecordServer aiUsageRecordServer, ContextSession contextSession)
    {
        _aiUsageRecordServer = aiUsageRecordServer;
        _contextSession = contextSession;
    }

    /// <summary>
    /// 获取用户AI使用记录
    /// </summary>
    /// <returns>AI使用记录</returns>
    [HttpGet("{userId}")]
    public async Task<ActionResult<List<AIUsageRecordResponse>>> GetAIUsageRecord()
    {
        long userId = _contextSession.UserId;
        var result = await _aiUsageRecordServer.GetAIUsageRecordAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// 新增AI使用记录
    /// </summary>
    /// <param name="request">AI使用记录请求</param>
    /// <returns>新增结果</returns>
    [HttpPost]
    public async Task<ActionResult<bool>> AddAIUsageRecord([FromBody] AIUsageRecordRequestAdd request)
    {
        var result = await _aiUsageRecordServer.AddAIUsageRecordAsync(request);
        return Ok(result);
    }
}