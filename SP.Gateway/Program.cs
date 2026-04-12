using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Scalar.AspNetCore;
using SP.Gateway.Middleware;
using SP.Common.Redis;
using SP.Gateway.Services;
using SP.Gateway.Services.Impl;
using SP.Common.Logger;
using SP.Common.ExceptionHandling;
using SP.Common.Nacos;
using SP.Common.Nacos.Configuration;
using SP.Gateway.ServiceDiscovery;

var builder = WebApplication.CreateBuilder(args);

var gatewaySecret = builder.Configuration["GatewaySecret"];
if (string.IsNullOrWhiteSpace(gatewaySecret))
{
    throw new InvalidOperationException("GatewaySecret 未配置，网关拒绝启动");
}


// 基础服务
builder.Services.AddControllers();
builder.Services.ConfigureDetailedModelValidation();

// 覆盖 Nacos 注册的 IP/Port 为宿主机IP + 对外端口（通过环境变量或配置传入）
var hostIp = Environment.GetEnvironmentVariable("HOST_IP") ?? builder.Configuration["HOST_IP"];
var exposePort = Environment.GetEnvironmentVariable("EXPOSE_PORT") ?? builder.Configuration["EXPOSE_PORT"];

if (!string.IsNullOrWhiteSpace(hostIp) || !string.IsNullOrWhiteSpace(exposePort))
{
    var overrides = new Dictionary<string, string?>();
    if (!string.IsNullOrWhiteSpace(hostIp)) overrides["nacos:Ip"] = hostIp;
    if (!string.IsNullOrWhiteSpace(exposePort)) overrides["nacos:Port"] = exposePort;
    builder.Configuration.AddInMemoryCollection(overrides);
}

// Nacos 配置中心（OpenAPI）
builder.Configuration.AddSpNacosConfiguration(builder.Configuration.GetSection("nacos"));

// 注册 SP.Common Nacos OpenAPI 封装
builder.Services.AddSpNacos(builder.Configuration);

// 添加 HTTP 客户端用于获取微服务的 OpenAPI 文档
builder.Services.AddHttpClient();
builder.Services.AddHttpClient("OpenApiProxy", client => { client.Timeout = TimeSpan.FromSeconds(10); });

// 添加Redis服务
builder.Services.AddRedisService(builder.Configuration);

// 注入loki日志服务
builder.Services.AddLoggerService(builder.Configuration);

// HttpContext 访问器（用于下游日志处理器关联当前请求）
builder.Services.AddHttpContextAccessor();

// 注册服务发现和配置服务
builder.Services.AddSingleton<INacosServiceDiscoveryService, NacosServiceDiscoveryService>();
builder.Services.AddSingleton<IGatewayConfigService, NacosGatewayConfigService>();
// 移除 Scoped 注册，改为单例注册，避免在中间件中从根提供程序解析 Scoped 服务
builder.Services.AddSingleton<ITokenIntrospectionService>(provider =>
{
    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient("TokenIntrospection");
    var logger = provider.GetRequiredService<ILogger<TokenIntrospectionService>>();
    var configService = provider.GetRequiredService<IGatewayConfigService>();
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new TokenIntrospectionService(httpClient, logger, configService, configuration);
});

// 添加HTTP客户端
builder.Services.AddHttpClient("IdentityServiceHealthCheck", client => { client.Timeout = TimeSpan.FromSeconds(10); });
builder.Services.AddHttpClient("TokenIntrospection", client => { client.Timeout = TimeSpan.FromSeconds(30); });

// 注册TokenIntrospectionService，使用专门的HttpClient
// 已通过单例方式在上方统一注册

// Ocelot + Nacos 服务发现，并添加下游响应日志处理器
builder.Services.AddOcelot(builder.Configuration)
    .AddDelegatingHandler<DownstreamLoggingHandler>(true);

// 使用 SP.Common 的 Nacos OpenAPI 封装替代 Ocelot.Provider.Nacos
builder.Services.AddSpNacosServiceDiscoveryForOcelot();

var app = builder.Build();

// 中间件管道
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.MapScalarApiReference("/scalar", options =>
    {
        options.Title = "SporeAccounting API Gateway";
        options.AddDocument("SPFinanceService",      "Finance Service",      "/openapi/proxy/SPFinanceService");
        options.AddDocument("SPCurrencyService",     "Currency Service",     "/openapi/proxy/SPCurrencyService");
        options.AddDocument("SPIdentityService",     "Identity Service",     "/openapi/proxy/SPIdentityService");
        options.AddDocument("SPConfigService",       "Config Service",       "/openapi/proxy/SPConfigService");
        options.AddDocument("SPMLService",           "ML Service",           "/openapi/proxy/SPMLService");
        options.AddDocument("SPNotificationService", "Notification Service", "/openapi/proxy/SPNotificationService");
        options.AddDocument("SPReportService",       "Report Service",       "/openapi/proxy/SPReportService");
        options.AddDocument("SPResourceService",     "Resource Service",     "/openapi/proxy/SPResourceService");
    });
}

// 全局异常处理（包含请求缓冲），优先放在最前面
app.UseFullExceptionHandling();

app.UseHttpsRedirection();

// 添加完整认证中间件
app.UseMiddleware<SPAuthenticationMiddleware>();

// 上游请求/响应日志（放在认证之后，便于记录用户信息）
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.MapControllers();

// 启用 Ocelot 作为网关
await app.UseOcelot();

app.Run();