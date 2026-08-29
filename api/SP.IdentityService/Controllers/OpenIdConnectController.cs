using System.Text;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using SP.Common.ConfigService;

namespace SP.IdentityService.Controllers
{
    /// <summary>
    /// OpenID Connect 发现端点控制器
    /// </summary>
    [Route(".well-known")]
    [ApiController]
    public class OpenIdConnectController : ControllerBase
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly ILogger<OpenIdConnectController> _logger;
        private readonly JwtConfigService _jwtConfigService;

        /// <summary>
        /// OpenIdConnectController 构造函数
        /// </summary>
        /// <param name="applicationManager"></param>
        /// <param name="scopeManager"></param>
        /// <param name="logger"></param>
        /// <param name="jwtConfigService"></param>
        public OpenIdConnectController(
            IOpenIddictApplicationManager applicationManager,
            IOpenIddictScopeManager scopeManager,
            ILogger<OpenIdConnectController> logger, JwtConfigService jwtConfigService)
        {
            _applicationManager = applicationManager;
            _scopeManager = scopeManager;
            _logger = logger;
            _jwtConfigService = jwtConfigService;
        }

        /// <summary>
        /// OpenID Connect 发现端点
        /// </summary>
        /// <returns>OpenID Connect 配置信息</returns>
        // 标准 OIDC Discovery 路径：/.well-known/openid-configuration
        // 兼容历史路径：/.well-known/openid_configuration
        [HttpGet("openid-configuration")]
        [HttpGet("openid_configuration")]
        public IActionResult GetConfiguration()
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                var configuration = new
                {
                    // 必需字段
                    issuer = baseUrl,
                    token_endpoint = $"{baseUrl}/api/auth/token",
                    userinfo_endpoint = $"{baseUrl}/api/auth/userinfo",
                    jwks_uri = $"{baseUrl}/.well-known/jwks",

                    // 可选字段
                    end_session_endpoint = $"{baseUrl}/api/auth/logout",
                    revocation_endpoint = $"{baseUrl}/api/auth/revoke",
                    introspection_endpoint = $"{baseUrl}/api/auth/introspect",

                    // 支持的主体类型
                    subject_types_supported = new[] { "public" },

                    // 支持的ID令牌签名算法
                    id_token_signing_alg_values_supported = new[]
                    {
                        "RS256" // 仅公布非对称算法，避免泄露对称密钥体系信息
                    },
                    
                    // 支持的授权范围
                    scopes_supported = new[]
                    {
                        "openid", // 必需
                        "profile", // 用户基本信息
                        "email", // 邮箱信息
                        "api", // API访问权限
                        "offline_access" // 离线访问（刷新令牌）
                    },

                    // 支持的令牌端点认证方法
                    token_endpoint_auth_methods_supported = new[]
                    {
                        "client_secret_basic", // 基本认证
                        "client_secret_post", // 表单认证
                        "client_secret_jwt", // JWT认证
                        "private_key_jwt" // 私钥JWT认证
                    },

                    // 支持的声明类型
                    claims_supported = new[]
                    {
                        "sub", // 主体标识符
                        "name", // 用户名
                        "email", // 邮箱
                        "role", // 角色
                        "iat", // 签发时间
                        "exp", // 过期时间
                        "iss", // 签发者
                        "aud" // 受众
                    },
                    
                    // 支持的授权类型 - 只保留密码模式相关
                    grant_types_supported = new[]
                    {
                        "client_credentials", // 客户端凭证模式
                        "password", // 密码模式
                        "refresh_token" // 刷新令牌模式
                    }
                };

                _logger.LogDebug("返回OpenID Connect配置信息");
                return Ok(configuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取OpenID Connect配置时发生错误");
                return StatusCode(500, new
                {
                    error = "server_error",
                    error_description = "获取配置时发生内部错误"
                });
            }
        }

        /// <summary>
        /// JWKS (JSON Web Key Set) 端点
        /// </summary>
        /// <returns>JWT签名密钥信息</returns>
        [HttpGet("jwks")]
        public async Task<IActionResult> GetJwks()
        {
            try
            {
                var keys = new List<object>();

                _logger.LogWarning("JWKS 请求已命中：当前实例使用对称签名密钥，不返回任何公钥以避免密钥泄露");

                var jwks = new
                {
                    keys = keys
                };

                _logger.LogDebug("返回JWKS信息，包含 {Count} 个密钥", keys.Count);
                return Ok(jwks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取JWKS时发生错误");
                return StatusCode(500, new
                {
                    error = "server_error",
                    error_description = "获取JWKS时发生内部错误"
                });
            }
        }

        /// <summary>
        /// 健康检查端点
        /// </summary>
        /// <returns>服务健康状态</returns>
        [HttpGet("health")]
        public IActionResult GetHealth()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "SPIdentityService",
                version = "1.0.0"
            });
        }
    }
}