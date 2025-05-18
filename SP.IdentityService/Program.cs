using Microsoft.AspNetCore.Identity;
using Nacos.AspNetCore.V2;
using Nacos.V2;
using Nacos.V2.DependencyInjection;
using Serilog;
using SP.Common.Redis;
using SP.IdentityService.DB;

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
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // 密码策略配置
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // 锁定设置
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // 用户设置
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<IdentityServerDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddOpenIddict(builder.Configuration);


        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // 添加认证中间件
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}