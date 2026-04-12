using System.Reflection;
using SP.Common;
using SP.Common.ConfigService;
using SP.Common.Logger;
using SP.Common.Middleware;
using SP.ResourceService;
using SP.ResourceService.DB;
using SP.Common.ExceptionHandling;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Consumer;
using SP.Common.Redis;
using SP.ResourceService.Mq;
using SP.Common.Nacos;
using SP.Common.Nacos.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureDetailedModelValidation();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new Microsoft.OpenApi.OpenApiComponents();
        document.Components.SecuritySchemes["Bearer"] = new Microsoft.OpenApi.OpenApiSecurityScheme
        {
            Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = Microsoft.OpenApi.ParameterLocation.Header,
            Type = Microsoft.OpenApi.SecuritySchemeType.ApiKey,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        };
        return Task.CompletedTask;
    });
    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        operation.Security = [new Microsoft.OpenApi.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer"),
                new List<string>()
            }
        }];
        return Task.CompletedTask;
    });
});
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
// 注册 IHttpContextAccessor
builder.Services.AddHttpContextAccessor(); 
// 注册 ContextSession
builder.Services.AddScoped<ContextSession>(); 

// 注册 DbContext
builder.Services.AddDbContext<ResourceServiceDbContext>(ServiceLifetime.Scoped);
// 注入MinIO
builder.Services.AddOssService(builder.Configuration);
// 注入OCR
builder.Services.AddOCRService(builder.Configuration);
// 注入提示词
builder.Services.AddPromptsService(builder.Configuration);
// 注入DeepSeek服务
builder.Services.AddDeepSeekService(builder.Configuration);
// 注入loki日志服务
builder.Services.AddLoggerService(builder.Configuration);
// 注入Redis服务（上传凭证绑定）
builder.Services.AddRedisService(builder.Configuration);
// 注册消息队列OCR消费者服务
builder.Services.AddHostedService<OCRConsumerService>();
builder.Services.AddHostedService<DeadLetterConsumerService>();
// 注册 RabbitMqConfigService
builder.Services.AddSingleton<RabbitMqConfigService>();
builder.Services.AddSingleton<RabbitMqMessage>(provider =>
{
    var configService = provider.GetRequiredService<RabbitMqConfigService>();
    var logger = provider.GetRequiredService<ILogger<RabbitMqMessage>>();
    return new RabbitMqMessage(logger, configService.GetRabbitMqConfig());
});
var app = builder.Build();

// 设置静态服务提供者
SP.Common.Model.SettingCommProperty.ServiceProvider = app.Services;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<ApplicationMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();