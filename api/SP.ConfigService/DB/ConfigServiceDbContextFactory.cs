using Microsoft.EntityFrameworkCore.Design;
using SP.Common.Nacos.Configuration;

namespace SP.ConfigService.DB;

/// <summary>
/// 设计期 DbContext 工厂，供 dotnet ef 等设计期工具使用。
/// </summary>
public class ConfigServiceDbContextFactory : IDesignTimeDbContextFactory<ConfigServiceDbContext>
{
    /// <summary>
    /// 创建设计期使用的 DbContext 实例
    /// </summary>
    /// <param name="args">命令行参数</param>
    /// <returns>ConfigServiceDbContext 实例</returns>
    public ConfigServiceDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var finalConfig = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddSpNacosConfiguration(configuration.GetSection("nacos"))
            .Build();

        return new ConfigServiceDbContext(finalConfig);
    }
}