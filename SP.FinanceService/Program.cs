using System.Reflection;
using Microsoft.OpenApi;
using Quartz;
using SP.Common.Refit;
using SP.Common.ServiceDiscovery;
using SP.Common;
using SP.Common.Nacos;
using SP.Common.Nacos.Configuration;
using SP.Common.ConfigService;
using SP.Common.Logger;
using SP.Common.Message.Email;
using SP.Common.Message.Mq;
using SP.Common.Message.Mq.Consumer;
using SP.Common.Middleware;
using SP.Common.Redis;
using SP.FinanceService.DB;
using SP.FinanceService.Mq;
using SP.FinanceService.RefitClient;
using SP.FinanceService.Service;
using SP.FinanceService.Service.Impl;
using SP.Common.ExceptionHandling;
using SP.FinanceService.Task.Accounting;
using SP.FinanceService.Task.Budget;
using SP.FinanceService.Task.FinancialHealth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureDetailedModelValidation();
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

builder.Services.AddNacosRefitClient<IUserServiceApi>(
    serviceName: "SPIdentityService",
    groupName: groupName,
    clusterName: clusterName,
    scheme: "http");
builder.Services.AddNacosRefitClient<IInSiteNotificationsServiceApi>(
    serviceName: "SPNotificationService",
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
builder.Services.AddScoped<IBudgetRecordServer, BudgetRecordServerImpl>();
builder.Services.AddScoped<ISharedExpenseSettlementServer, SharedExpenseSettlementServerImpl>();
builder.Services.AddScoped<ISharedExpenseServer, SharedExpenseServerImpl>();
builder.Services.AddScoped<ISharedExpenseReminderServer, SharedExpenseReminderServerImpl>();
builder.Services.AddScoped<ISharedExpenseServer, SharedExpenseServerImpl>();
builder.Services.AddScoped<IPaymentMethodServer, PaymentMethodServerImpl>();
builder.Services.AddScoped<IFinancialHealthScoreService, FinancialHealthScoreServiceImpl>();

// 注册 IHttpContextAccessor
builder.Services.AddHttpContextAccessor();
// 注册 ContextSession
builder.Services.AddScoped<ContextSession>();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddSingleton<JwtConfigService>();

// 注册邮件配置服务
builder.Services.AddSingleton<EmailConfigService>();
// 注册邮件服务
builder.Services.AddSingleton<EmailMessage>(provider =>
{
    var configService = provider.GetRequiredService<EmailConfigService>();
    return new EmailMessage(configService.GetEmailConfig());
});

// 注册MQ配置服务
builder.Services.AddSingleton<RabbitMqConfigService>();
// 注册RabbitMqMessage服务
builder.Services.AddSingleton<RabbitMqMessage>(provider =>
{
    var configService = provider.GetRequiredService<RabbitMqConfigService>();
    var logger = provider.GetRequiredService<ILogger<RabbitMqMessage>>();
    return new RabbitMqMessage(logger, configService.GetRabbitMqConfig());
});

// 注册后台服务（MQ消费者）
builder.Services.AddHostedService<BudgetConsumerService>();
builder.Services.AddHostedService<BudgetNotificationConsumerService>();
builder.Services.AddHostedService<DeadLetterConsumerService>();

builder.Services.AddRedisService(builder.Configuration);
// 注入loki日志服务
builder.Services.AddLoggerService(builder.Configuration);

// 添加定时任务
builder.Services.AddQuartz(q =>
{
    var exchangeRateTimerJobKey = new JobKey("ExchangeRateTimer");
    q.AddJob<BudgetDepletionWatcher>(opts => opts.WithIdentity(exchangeRateTimerJobKey));
    q.AddTrigger(opts => opts
        .ForJob(exchangeRateTimerJobKey)
        .WithIdentity("ExchangeRateTimerTrigger")
        .StartNow()
        .WithCronSchedule("0 0 1 * * ?"));
});
// 添加定时开销记录任务
builder.Services.AddQuartz(q =>
{
    var accountingWatcherJobKey = new JobKey("AccountingWatcherJob");
    q.AddJob<AccountingWatcher>(opts => opts.WithIdentity(accountingWatcherJobKey));
    q.AddTrigger(opts => opts
        .ForJob(accountingWatcherJobKey)
        .WithIdentity("AccountingWatcherTrigger")
        .StartNow()
        .WithCronSchedule("0 0 0 * * ?")); // 每天午夜12点执行
});
// 添加分摊提醒任务
builder.Services.AddQuartz(q =>
{
    var sharedExpenseReminderJobKey = new JobKey("SharedExpenseReminderWatcherJob");
    q.AddJob<SP.FinanceService.Task.SharedExpense.SharedExpenseReminderWatcher>(opts =>
        opts.WithIdentity(sharedExpenseReminderJobKey));
    q.AddTrigger(opts => opts
        .ForJob(sharedExpenseReminderJobKey)
        .WithIdentity("SharedExpenseReminderWatcherTrigger")
        .StartNow()
        .WithCronSchedule("0 0/30 * * * ?")); // 每30分钟执行
});
// 添加财务健康评分月度计算任务
builder.Services.AddQuartz(q =>
{
    var healthScoreJobKey = new JobKey("FinancialHealthScoreJob");
    q.AddJob<FinancialHealthScoreTask>(opts => opts.WithIdentity(healthScoreJobKey));
    q.AddTrigger(opts => opts
        .ForJob(healthScoreJobKey)
        .WithIdentity("FinancialHealthScoreTrigger")
        .StartNow()
        .WithCronSchedule("0 0 2 1 * ?")); // 每月1日凌晨2点执行
});
builder.Services.AddQuartzHostedService(options =>
{
    //启用 Quartz 的托管服务，`WaitForJobsToComplete = true` 表示在应用程序停止时等待任务完成后再关闭。
    options.WaitForJobsToComplete = true;
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