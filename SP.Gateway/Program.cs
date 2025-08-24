using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Nacos;
using Nacos.V2.DependencyInjection;
using Nacos.AspNetCore.V2;
using SP.Gateway.Middleware;
using SP.Common.Redis;
using SP.Gateway.Services;
using SP.Gateway.Services.Impl;
using SP.Common.Logger;
using SP.Common.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);


// 基础服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 添加Nacos服务注册
builder.Services.AddNacosAspNet(builder.Configuration);
// Nacos 配置中心
builder.Services.AddNacosV2Naming(builder.Configuration);
builder.Configuration.AddNacosV2Configuration(builder.Configuration.GetSection("nacos"));

// 添加 HTTP 客户端用于获取微服务的 OpenAPI 文档
builder.Services.AddHttpClient();

// 添加Redis服务
builder.Services.AddRedisService(builder.Configuration);

// 注入loki日志服务
builder.Services.AddLoggerService(builder.Configuration);

// HttpContext 访问器（用于下游日志处理器关联当前请求）
builder.Services.AddHttpContextAccessor();

// 注册服务发现和配置服务
builder.Services.AddSingleton<INacosServiceDiscoveryService, NacosServiceDiscoveryService>();
builder.Services.AddSingleton<IGatewayConfigService, NacosGatewayConfigService>();
builder.Services.AddScoped<ITokenIntrospectionService, TokenIntrospectionService>();

// 添加HTTP客户端
builder.Services.AddHttpClient("IdentityServiceHealthCheck", client => { client.Timeout = TimeSpan.FromSeconds(10); });
builder.Services.AddHttpClient("TokenIntrospection", client => { client.Timeout = TimeSpan.FromSeconds(30); });

// 注册TokenIntrospectionService，使用专门的HttpClient
builder.Services.AddScoped<ITokenIntrospectionService>(provider =>
{
    var httpClient = provider.GetRequiredService<IHttpClientFactory>().CreateClient("TokenIntrospection");
    var logger = provider.GetRequiredService<ILogger<TokenIntrospectionService>>();
    var configService = provider.GetRequiredService<IGatewayConfigService>();
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new TokenIntrospectionService(httpClient, logger, configService, configuration);
});

// Ocelot + Nacos 服务发现，并添加下游响应日志处理器
builder.Services.AddOcelot(builder.Configuration)
    .AddNacosDiscovery()
    .AddDelegatingHandler<DownstreamLoggingHandler>(true);

if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Local")
{
    // Swagger 配置
    builder.Services.AddSwaggerGen();
    builder.Services.AddSwaggerForOcelot(builder.Configuration);
}

var app = builder.Build();

// 中间件管道
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerForOcelotUI(opt =>
    {
        opt.PathToSwaggerGenerator = "/swagger/docs";
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