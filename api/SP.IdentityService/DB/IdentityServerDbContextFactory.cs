using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SP.Common.Nacos.Configuration;

namespace SP.IdentityService.DB;

/// <summary>
/// 设计期 DbContext 工厂，供 dotnet ef 等设计期工具使用。
/// 独立加载 appsettings 与 Nacos 配置以获取 MySQL 连接字符串，
/// 避免在设计期构建完整应用主机（如 JWT/OpenIddict 等运行期依赖）。
/// </summary>
public class IdentityServerDbContextFactory : IDesignTimeDbContextFactory<IdentityServerDbContext>
{
    /// <summary>
    /// 创建设计期使用的 DbContext 实例
    /// </summary>
    /// <param name="args">命令行参数</param>
    /// <returns>IdentityServerDbContext 实例</returns>
    public IdentityServerDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        // 从 Nacos 配置中心加载（包含 ConnectionStrings:MySQLConnection）
        var finalConfig = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddSpNacosConfiguration(configuration.GetSection("nacos"))
            .Build();

        return new IdentityServerDbContext(finalConfig);
    }
}
