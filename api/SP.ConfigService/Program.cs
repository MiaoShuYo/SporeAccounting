using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SP.Common.Middleware;
using SP.Common;
using SP.ConfigService.DB;
using SP.ConfigService.Service;
using SP.ConfigService.Service.Impl;
using SP.Common.ConfigService;
using SP.Common.Nacos;
using SP.Common.Nacos.Configuration;
using SP.Common.Logger;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Consumer;
using SP.Common.Redis;
using SP.Common.ExceptionHandling;
using SP.ConfigService.Mq;

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

// 注册HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 注册ContextSession
builder.Services.AddScoped<ContextSession>();

// 注册AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODA4Nzg0MDAwIiwiaWF0IjoiMTc3NzI5NzQ4NSIsImFjY291bnRfaWQiOiIwMTlkY2YyZDc5ZWE3YTllOTMwZjlkNDg2OGM0Y2UwNSIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa3E3anl2dDFmbWRzYjBycDk3em10djFmIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.zYn-infaMt82vZGyIBFOm58zf9IKVL3iYzzPjhg2N9a5l9kx9cLeJnC0Fj-Cg4p2Y6yICpl2zrMKBjzbPvgcvKuFOFmqa5LaLgipNRjPIU2HbiG17xUd0n6yzAADO9gDUtuk1OZ-wIEYvw5HkPjYNintzBDbXEkgI9Y_ERmJwiJyamxXlQNdRTslfUI2b506tYsz-YE6DxX-JyALtxvApT-7ZNY3u9xSBYcKSmw6Sw1xaznnrpv9XrkxeUXWWYb1Drp6dyLId94hceUAntUW7etsyot-qVqKLn6USlT1t3hHciWV73X6mMc_igOR3SAQXRjCPmqNoqz9rXHW7BX87g", Assembly.GetExecutingAssembly());

// 注册数据库上下文
builder.Services.AddDbContext<ConfigServiceDbContext>(ServiceLifetime.Scoped);
// 注册服务 - 改为Scoped生命周期
builder.Services.AddScoped<IConfigServer, ConfigServerImpl>();
// 注册JwtConfigService（ApplicationMiddleware需要）
builder.Services.AddSingleton<JwtConfigService>();

// 注册Redis服务
builder.Services.AddRedisService(builder.Configuration);
// 注入loki日志服务
builder.Services.AddLoggerService(builder.Configuration);

// 注册RabbitMQ配置和消息服务
builder.Services.AddSingleton<RabbitMqConfigService>();
builder.Services.AddSingleton<RabbitMqMessage>(provider =>
{
    var configService = provider.GetRequiredService<RabbitMqConfigService>();
    var logger = provider.GetRequiredService<ILogger<RabbitMqMessage>>();
    return new RabbitMqMessage(logger, configService.GetRabbitMqConfig());
});

// 注册默认币种消费者
builder.Services.AddHostedService<UserConfigDefaultCurrencyConsumerService>();
builder.Services.AddHostedService<DeadLetterConsumerService>();

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