using System.Reflection;
using Nacos.AspNetCore.V2;
using Nacos.V2.DependencyInjection;
using Quartz;
using SP.Common.Middleware;
using SP.CurrencyService.DB;
using SP.CurrencyService.Service;
using SP.CurrencyService.Service.Impl;
using SP.CurrencyService.Task.ExchangeRate;
using SP.Common.ConfigService;
using SP.Common.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    // 添加XML文档
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 添加Nacos服务注册
builder.Services.AddNacosAspNet(builder.Configuration);
// 添加Nacos配置中心
builder.Configuration.AddNacosV2Configuration(builder.Configuration.GetSection("nacos"));
builder.Services.AddNacosV2Naming(builder.Configuration);
// 注册 DbContext
builder.Services.AddDbContext<CurrencyServiceDbContext>(ServiceLifetime.Scoped);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// 服务注册
builder.Services.AddScoped<ICurrencyServer, CurrencyServerImpl>();
builder.Services.AddScoped<IExchangeRateRecordServer, ExchangeRateRecordServerImpl>();

// 注册JwtConfigService（ApplicationMiddleware需要）
builder.Services.AddSingleton<JwtConfigService>();

// 注入redis
builder.Services.AddRedisService(builder.Configuration);

// 添加定时任务
builder.Services.AddQuartz(q =>
{
    var exchangeRateTimerJobKey = new JobKey("ExchangeRateTimer");
    q.AddJob<ExchangeRateTimer>(opts => opts.WithIdentity(exchangeRateTimerJobKey));
    q.AddTrigger(opts => opts
        .ForJob(exchangeRateTimerJobKey)
        .WithIdentity("ExchangeRateTimerTrigger")
        .StartNow()
        .WithCronSchedule("0 0 1 * * ?"));
});
builder.Services.AddQuartzHostedService(options =>
{
    //启用 Quartz 的托管服务，`WaitForJobsToComplete = true` 表示在应用程序停止时等待任务完成后再关闭。
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApplicationMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();