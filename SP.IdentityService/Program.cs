using Microsoft.AspNetCore.Identity;
using Nacos.AspNetCore.V2;
using Nacos.V2.DependencyInjection;
using SP.Common.Redis;
using SP.IdentityService.DB;
using SP.IdentityService.Models.Entity;
using Microsoft.OpenApi.Models;
using SP.Common.ConfigService;
using SP.Common.Message.Mq;
using SP.Common.Message.Email;
using SP.IdentityService.Impl;
using SP.IdentityService.Service;
using SP.IdentityService.Service.Impl;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SP.IdentityService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        // 添加Nacos服务注册
        builder.Services.AddNacosAspNet(builder.Configuration);
        // 添加Nacos配置中心
        builder.Configuration.AddNacosV2Configuration(builder.Configuration.GetSection("nacos"));
        builder.Services.AddNacosV2Naming(builder.Configuration);
        // 引入redis
        builder.Services.AddRedisService(builder.Configuration);

        // 注册DbContext，使用MySQL
        builder.Services.AddDbContext<IdentityServerDbContext>(ServiceLifetime.Scoped);

        // 添加ASP.NET Core Identity服务
        builder.Services.AddIdentity<SpUser, SpRole>(options =>
            {
                // 密码策略配置
                options.Password.RequireDigit = true; // 要求数字
                options.Password.RequireLowercase = true; // 要求小写字母
                options.Password.RequireUppercase = true; // 要求大写字母
                options.Password.RequireNonAlphanumeric = true; // 要求特殊字符
                options.Password.RequiredLength = 6; // 最小长度
                options.Password.RequiredUniqueChars = 4; // 要求不同字符的最小数量

                // 锁定设置
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // 锁定15分钟
                options.Lockout.MaxFailedAccessAttempts = 5; // 5次失败尝试后锁定
                options.Lockout.AllowedForNewUsers = true; // 对新用户启用锁定

                // 禁用用户名和邮箱规范化
                options.User.RequireUniqueEmail = false;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            })
            .AddEntityFrameworkStores<IdentityServerDbContext>()
            .AddDefaultTokenProviders();

        // 替换默认的 UserStore
        builder.Services.AddScoped<IUserStore<SpUser>, SPUserStore>();

        builder.Services.AddOpenIddict(builder.Configuration);

        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "SP.IdentityService", Version = "v1" });
            c.OperationFilter<SwaggerTokenRequestFilter>();

            // 添加XML文档
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }

            // 添加安全定义
            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri("/connect/token", UriKind.Relative),
                        Scopes = new Dictionary<string, string>
                        {
                            { "api", "API访问权限" },
                            { "offline_access", "获取刷新令牌" }
                        }
                    }
                }
            });

            // 应用安全要求
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    new[] { "api" }
                }
            });
        });

        builder.Services.AddScoped<IAuthorizationService, AuthorizationServiceImpl>();
        builder.Services.AddScoped<IUserService, UserServiceImpl>();
        builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
        builder.Services.AddScoped<IRoleService, RoleServiceImpl>();
        builder.Services.AddSingleton<EmailConfigService>();
        builder.Services.AddSingleton<RabbitMqConfigService>();
        builder.Services.AddScoped(provider =>
        {
            var configService = provider.GetRequiredService<EmailConfigService>();
            return new EmailMessage(configService.GetEmailConfig());
        });
        builder.Services.AddScoped<RabbitMqMessage>(provider =>
        {
            var configService = provider.GetRequiredService<RabbitMqConfigService>();
            var logger = provider.GetRequiredService<ILogger<RabbitMqMessage>>();
            return new RabbitMqMessage(logger, configService.GetRabbitMqConfig());
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        // 添加认证中间件
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}