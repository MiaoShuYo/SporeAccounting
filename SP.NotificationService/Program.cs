using System.Reflection;
using Microsoft.OpenApi;
using SP.Common;
using SP.Common.Middleware;
using SP.Common.Refit;
using SP.Common.ServiceDiscovery;
using SP.Common.Nacos;
using SP.Common.Nacos.Configuration;
using SP.NotificationService.DB;
using SP.NotificationService.RefitClient;
using SP.NotificationService.Service;
using SP.NotificationService.Service.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        };
        return Task.CompletedTask;
    });
    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        operation.Security = [new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference("Bearer"),
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

// 注册通用服务发现
builder.Services.AddSingleton<IServiceDiscovery, NacosOpenApiServiceDiscovery>();

// 注册 Refit 客户端（基于 Nacos 服务发现）
var nacosSection = builder.Configuration.GetSection("nacos");
var groupName = nacosSection.GetValue<string>("GroupName") ?? "DEFAULT_GROUP";
var clusterName = nacosSection.GetValue<string>("ClusterName") ?? "DEFAULT";

builder.Services.AddNacosRefitClient<IIdentityServiceApi>(
    serviceName: "SPIdentityService",
    groupName: groupName,
    clusterName: clusterName,
    scheme: "http");
// 注册 IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
// 注册ContextSession
builder.Services.AddScoped<ContextSession>();

// 注册 DbContext
builder.Services.AddDbContext<NotificationServiceDBContext>(ServiceLifetime.Scoped);// 注册站内信服务
builder.Services.AddScoped<IInSiteNotificationsServer, InSiteNotificationsServerImpl>();// 注册AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODA4Nzg0MDAwIiwiaWF0IjoiMTc3NzI5NzQ4NSIsImFjY291bnRfaWQiOiIwMTlkY2YyZDc5ZWE3YTllOTMwZjlkNDg2OGM0Y2UwNSIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa3E3anl2dDFmbWRzYjBycDk3em10djFmIiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.zYn-infaMt82vZGyIBFOm58zf9IKVL3iYzzPjhg2N9a5l9kx9cLeJnC0Fj-Cg4p2Y6yICpl2zrMKBjzbPvgcvKuFOFmqa5LaLgipNRjPIU2HbiG17xUd0n6yzAADO9gDUtuk1OZ-wIEYvw5HkPjYNintzBDbXEkgI9Y_ERmJwiJyamxXlQNdRTslfUI2b506tYsz-YE6DxX-JyALtxvApT-7ZNY3u9xSBYcKSmw6Sw1xaznnrpv9XrkxeUXWWYb1Drp6dyLId94hceUAntUW7etsyot-qVqKLn6USlT1t3hHciWV73X6mMc_igOR3SAQXRjCPmqNoqz9rXHW7BX87g", Assembly.GetExecutingAssembly());


var app = builder.Build();

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