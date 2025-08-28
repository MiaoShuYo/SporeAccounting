using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace SP.Common.Refit;

/// <summary>
/// 为所有下游 Refit 请求添加网关签名与用户透传头
/// </summary>
public class GatewaySignatureHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public GatewaySignatureHandler(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 添加网关签名
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var secret = _configuration["GatewaySecret"] ?? "SP_Gateway_Secret_2024";
        var signature = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{timestamp}.{secret}"));
        request.Headers.Remove("X-Gateway-Signature");
        request.Headers.Add("X-Gateway-Signature", signature);

        // 透传用户信息（若存在）
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext?.User;
        if (user?.Identity?.IsAuthenticated == true)
        {
            var userId = user.FindFirstValue("UserId");
            var userName = user.FindFirstValue("UserName");
            var email = user.FindFirstValue("Email");

            if (!string.IsNullOrEmpty(userId))
                request.Headers.TryAddWithoutValidation("X-User-Id", userId);
            if (!string.IsNullOrEmpty(userName))
                request.Headers.TryAddWithoutValidation("X-User-Name", userName);
            if (!string.IsNullOrEmpty(email))
                request.Headers.TryAddWithoutValidation("X-User-Email", email);
        }

        // 如果上游带了 Authorization，则透传（有些服务可能需要）
        if (httpContext?.Request?.Headers?.TryGetValue("Authorization", out var authHeader) == true)
        {
            if (!request.Headers.Contains("Authorization"))
            {
                request.Headers.Authorization = AuthenticationHeaderValue.Parse(authHeader!);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}


