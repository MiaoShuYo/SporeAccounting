using Microsoft.IdentityModel.Tokens;
using Nacos.AspNetCore.V2;
using OpenIddict.Validation.AspNetCore;
using SP.Common.Redis;
using SP.IdentityService.DB;
using Microsoft.EntityFrameworkCore;

namespace SP.IdentityService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 添加Nacos配置中心
        builder.Host.ConfigureAppConfiguration((_, builder) =>
        {
            var configurationRoot = builder.Build();
            builder.AddNacosV2Configuration(configurationRoot.GetSection("nacos"));
        });

        // 添加Nacos服务注册
        builder.Services.AddNacosAspNet(builder.Configuration);
        // 引入redis
        builder.Services.AddRedisService(builder.Configuration);
        
        // 注册DbContext，使用MySQL
        builder.Services.AddDbContext<IdentityServerDbContext>(ServiceLifetime.Scoped);
        builder.Services.AddOpenIddict(builder.Configuration);
        //builder.Services.AddSingleton<IRedisService, RedisService>();
        
        // Add services to the container.
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
       // builder.Services.AddHostedService<SeedData>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}