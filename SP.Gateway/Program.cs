using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Nacos;
using Nacos.V2.DependencyInjection;
using Nacos.AspNetCore.V2;
using SP.Gateway.Middleware;
using SP.Common.Redis;
using SP.Gateway.Services;
using SP.Gateway.Services.Impl;

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
    return new TokenIntrospectionService(httpClient, logger, configService);
});

// Ocelot + Nacos 服务发现
builder.Services.AddOcelot(builder.Configuration)
    .AddNacosDiscovery();

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

app.UseHttpsRedirection();

// 添加完整认证中间件
app.UseMiddleware<SPAuthenticationMiddleware>();

app.MapControllers();

// 启用 Ocelot 作为网关
await app.UseOcelot();

app.Run();