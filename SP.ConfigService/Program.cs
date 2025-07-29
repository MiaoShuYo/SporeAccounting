using SP.Common.Middleware;
using SP.ConfigService.DB;
using SP.ConfigService.Service;
using SP.ConfigService.Service.Impl;
using Nacos.V2.DependencyInjection;
using Nacos.AspNetCore.V2;
using SP.Common.ConfigService;

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

// 注册数据库上下文
builder.Services.AddDbContext<ConfigServiceDbContext>(ServiceLifetime.Scoped);
// 注册服务
builder.Services.AddSingleton<IConfigServer, ConfigServerImpl>();
// 注册JwtConfigService（ApplicationMiddleware需要）
builder.Services.AddSingleton<JwtConfigService>();

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