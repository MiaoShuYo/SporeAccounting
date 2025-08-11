using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SP.Common.Middleware;
using SP.Common;
using SP.ConfigService.DB;
using SP.ConfigService.Service;
using SP.ConfigService.Service.Impl;
using Nacos.V2.DependencyInjection;
using Nacos.AspNetCore.V2;
using SP.Common.ConfigService;
using SP.Common.Logger;
using SP.Common.Redis;

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
            new string[] {}
        }
    });
});

// 添加Nacos服务注册
builder.Services.AddNacosAspNet(builder.Configuration);
// 添加Nacos配置中心
builder.Configuration.AddNacosV2Configuration(builder.Configuration.GetSection("nacos"));
builder.Services.AddNacosV2Naming(builder.Configuration);

// 注册HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// 注册ContextSession
builder.Services.AddScoped<ContextSession>();

// 注册AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApplicationMiddleware>();

app.UseHttpsRedirection();

// 添加认证中间件
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();