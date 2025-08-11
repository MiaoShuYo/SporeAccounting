using Microsoft.EntityFrameworkCore;
using SP.ConfigService.Models.Entity;

namespace SP.ConfigService.DB;

/// <summary>
/// 配置服务数据库上下文
/// </summary>
public class ConfigServiceDbContext : DbContext
{
    /// <summary> 
    /// 配置表
    /// </summary>
    public DbSet<Config?> Configs { get; set; }

    /// <summary>
    /// 数据库连接配置
    /// </summary>
    private readonly IConfiguration _dbConfig;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbConfig"></param>
    public ConfigServiceDbContext(IConfiguration dbConfig)
    {
        _dbConfig = dbConfig;
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