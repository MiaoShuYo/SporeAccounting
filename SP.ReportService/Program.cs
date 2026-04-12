using System.Reflection;
using SP.Common;
using SP.Common.ConfigService;
using SP.Common.Logger;
using SP.Common.Middleware;
using SP.ReportService.DB;
using SP.ReportService.Service;
using SP.ReportService.Service.Impl;
using SP.Common.ExceptionHandling;
using SP.Common.ServiceDiscovery;
using SP.Common.Refit;
using SP.Common.Nacos;
using SP.Common.Nacos.Configuration;
using SP.ReportService.RefitClient;

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

// Nacos 配置中心（OpenAPI）
builder.Configuration.AddSpNacosConfiguration(builder.Configuration.GetSection("nacos"));

// 注册 SP.Common Nacos OpenAPI 封装
builder.Services.AddSpNacos(builder.Configuration);
// 注册 DbContext
builder.Services.AddDbContext<ReportServiceDBContext>(ServiceLifetime.Scoped);
// 注册 IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
// 注册 ContextSession
builder.Services.AddScoped<ContextSession>();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddSingleton<JwtConfigService>();
builder.Services.AddScoped<IReportServer, ReportServerImpl>();
// 注入loki日志服务
builder.Services.AddLoggerService(builder.Configuration);

// 注册通用服务发现
builder.Services.AddSingleton<IServiceDiscovery, NacosOpenApiServiceDiscovery>();

// 注册 Refit 客户端（基于通用服务发现 + Nacos，无需硬编码 BaseUrl）
var nacosSection = builder.Configuration.GetSection("nacos");
var groupName = nacosSection.GetValue<string>("GroupName") ?? "DEFAULT_GROUP";
var clusterName = nacosSection.GetValue<string>("ClusterName") ?? "DEFAULT";

builder.Services.AddNacosRefitClient<IBudgetServiceApi>(
    serviceName: "SPFinanceService",
    groupName: groupName,
    clusterName: clusterName,
    scheme: "http");
builder.Services.AddNacosRefitClient<IBudgetRecordServiceApi>(
    serviceName: "SPFinanceService",
    groupName: groupName,
    clusterName: clusterName,
    scheme: "http");

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