using Microsoft.EntityFrameworkCore;

namespace SP.IdentityService.DB;

/// <summary>
/// IdentityServer数据库上下文
/// </summary>
public class IdentityServerDbContext : DbContext
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    IConfiguration _dbConfig;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbConfig"></param>
    public IdentityServerDbContext(IConfiguration dbConfig)
    {
        _dbConfig = dbConfig;
    }

    /// <summary>
    /// 配置数据库模型
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // 配置 OpenIddict
        modelBuilder.UseOpenIddict();
    }

    /// <summary>
    /// 数据库连接配置
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var serverVersion = ServerVersion.AutoDetect(_dbConfig.GetConnectionString("MySQLConnection"));
        optionsBuilder.UseMySql(_dbConfig.GetConnectionString("MySQLConnection"), serverVersion);
    }
}