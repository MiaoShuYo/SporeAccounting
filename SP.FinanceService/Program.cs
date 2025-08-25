using System.Reflection;
using Microsoft.OpenApi.Models;
using Nacos.AspNetCore.V2;
using Nacos.V2.DependencyInjection;
using Nacos.V2;
using Refit;
using SP.Common.Refit;
using SP.Common.ServiceDiscovery;
using SP.Common;
using SP.Common.ConfigService;
using SP.Common.Logger;
using SP.Common.Message.Mq;
using SP.Common.Middleware;
using SP.Common.Redis;
using SP.FinanceService.DB;
using SP.FinanceService.RefitClient;
using SP.FinanceService.Service;
using SP.FinanceService.Service.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // 添加XML文档
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // 添加JWT认证配置
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

// 添加Nacos服务注册
builder.Services.AddNacosAspNet(builder.Configuration);
// 添加Nacos配置中心
builder.Configuration.AddNacosV2Configuration(builder.Configuration.GetSection("nacos"));
builder.Services.AddNacosV2Naming(builder.Configuration);

// 注册通用服务发现
builder.Services.AddSingleton<IServiceDiscovery, NacosServiceDiscovery>();

// 注册 Refit 客户端（基于通用服务发现 + Nacos，无需硬编码 BaseUrl）
var nacosSection = builder.Configuration.GetSection("nacos");
var groupName = nacosSection.GetValue<string>("GroupName") ?? "DEFAULT_GROUP";
var clusterName = nacosSection.GetValue<string>("ClusterName") ?? "DEFAULT";

builder.Services.AddNacosRefitClient<ICurrencyServiceApi>(
    serviceName: "SPCurrencyService",
    groupName: groupName,
    clusterName: clusterName,
    scheme: "http");

builder.Services.AddNacosRefitClient<IConfigServiceApi>(
    serviceName: "SPConfigService",
    groupName: groupName,
    clusterName: clusterName,
    scheme: "http");

// 注册 DbContext
builder.Services.AddDbContext<FinanceServiceDbContext>(ServiceLifetime.Scoped);

// 注册服务
builder.Services.AddScoped<ITransactionCategoryServer, TransactionCategoryServerImpl>();
builder.Services.AddScoped<IAccountBookServer, AccountBookServerImpl>();
builder.Services.AddScoped<IAccountingServer, AccountingServerImpl>();
builder.Services.AddScoped<IBudgetServer, BudgetServerImpl>();
builder.Services.AddScoped<ICurrencyService, CurrencyServiceImpl>();

// 注册 IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
// 注册 ContextSession
builder.Services.AddScoped<ContextSession>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<JwtConfigService>();

// 注册MQ配置服务
builder.Services.AddSingleton<RabbitMqConfigService>();
// 注册RabbitMqMessage服务
builder.Services.AddScoped<RabbitMqMessage>(provider =>
{
    var configService = provider.GetRequiredService<RabbitMqConfigService>();
    var logger = provider.GetRequiredService<ILogger<RabbitMqMessage>>();
    return new RabbitMqMessage(logger, configService.GetRabbitMqConfig());
});

builder.Services.AddRedisService(builder.Configuration);
// 注入loki日志服务
builder.Services.AddLoggerService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseMiddleware<ApplicationMiddleware>();

app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();