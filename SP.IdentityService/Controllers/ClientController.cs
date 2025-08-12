using Microsoft.AspNetCore.Mvc;
using SP.IdentityService.Services;

namespace SP.IdentityService.Controllers;

/// <summary>
/// 客户端管理控制器
/// </summary>
[Route("api/clients")]
[ApiController]
public class ClientController : ControllerBase
{
    private readonly IClientRegistrationService _clientRegistrationService;
    private readonly ILogger<ClientController> _logger;

    public ClientController(
        IClientRegistrationService clientRegistrationService,
        ILogger<ClientController> logger)
    {
        _clientRegistrationService = clientRegistrationService;
        _logger = logger;
    }

    /// <summary>
    /// 注册客户端
    /// </summary>
    /// <param name="request">注册请求</param>
    /// <returns>注册结果</returns>
    [HttpPost("register")]
    public async Task<ActionResult> RegisterClient([FromBody] ClientRegistrationRequest request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.ClientId))
            {
                return BadRequest(new { error = "client_id_required", error_description = "客户端ID不能为空" });
            }

            if (string.IsNullOrEmpty(request.ClientSecret))
            {
                return BadRequest(new { error = "client_secret_required", error_description = "客户端密钥不能为空" });
            }

            if (string.IsNullOrEmpty(request.DisplayName))
            {
                return BadRequest(new { error = "display_name_required", error_description = "显示名称不能为空" });
            }

            var result = await _clientRegistrationService.RegisterClientAsync(
                request.ClientId,
                request.ClientSecret,
                request.DisplayName,
                request.Permissions ?? new string[0]);

            return Ok(new
            {
                success = result,
                message = "客户端注册成功",
                client_id = request.ClientId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "注册客户端时发生错误");
            return StatusCode(500, new
            {
                error = "registration_failed",
                error_description = ex.Message
            });
        }
    }

    /// <summary>
    /// 删除客户端
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{clientId}")]
    public async Task<ActionResult> DeleteClient(string clientId)
    {
        try
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return BadRequest(new { error = "client_id_required", error_description = "客户端ID不能为空" });
            }

            var result = await _clientRegistrationService.DeleteClientAsync(clientId);

            return Ok(new
            {
                success = result,
                message = "客户端删除成功",
                client_id = clientId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除客户端时发生错误");
            return StatusCode(500, new
            {
                error = "deletion_failed",
                error_description = ex.Message
            });
        }
    }

    /// <summary>
    /// 获取客户端列表
    /// </summary>
    /// <returns>客户端列表</returns>
    [HttpGet]
    public async Task<ActionResult> GetClients()
    {
        try
        {
            var clients = await _clientRegistrationService.GetClientsAsync();

            return Ok(new
            {
                clients = clients,
                count = clients.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取客户端列表时发生错误");
            return StatusCode(500, new
            {
                error = "retrieval_failed",
                error_description = ex.Message
            });
        }
    }

    /// <summary>
    /// 检查客户端是否存在
    /// </summary>
    /// <param name="clientId">客户端ID</param>
    /// <returns>是否存在</returns>
    [HttpGet("{clientId}/exists")]
    public async Task<ActionResult> ClientExists(string clientId)
    {
        try
        {
            if (string.IsNullOrEmpty(clientId))
            {
                return BadRequest(new { error = "client_id_required", error_description = "客户端ID不能为空" });
            }

            var exists = await _clientRegistrationService.ClientExistsAsync(clientId);

            return Ok(new
            {
                client_id = clientId,
                exists = exists
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查客户端是否存在时发生错误");
            return StatusCode(500, new
            {
                error = "check_failed",
                error_description = ex.Message
            });
        }
    }

    /// <summary>
    /// 初始化默认客户端（仅用于开发环境）
    /// </summary>
    /// <returns>初始化结果</returns>
    [HttpPost("initialize")]
    public async Task<ActionResult> InitializeDefaultClients()
    {
        try
        {
            var result = await _clientRegistrationService.InitializeDefaultClientsAsync();

            return Ok(new
            {
                success = result,
                message = result ? "默认客户端初始化成功" : "默认客户端初始化失败"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化默认客户端时发生错误");
            return StatusCode(500, new
            {
                error = "initialization_failed",
                error_description = ex.Message
            });
        }
    }
}

/// <summary>
/// 客户端注册请求
/// </summary>
public class ClientRegistrationRequest
{
    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// 客户端密钥
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// 显示名称
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// 权限列表
    /// </summary>
    public string[]? Permissions { get; set; }
}
