using Microsoft.EntityFrameworkCore;
using SP.MLService.Models.Entity;

namespace SP.MLService.DB;

/// <summary>
/// ML服务数据库上下文
/// </summary>
public class MLServiceDbContext : DbContext
{
    /// <summary>
    /// AI使用记录
    /// </summary>
    public DbSet<AIUsageRecord> AIUsageRecords { get; set; }

    /// <summary>
    /// 数据库连接配置
    /// </summary>
    private readonly IConfiguration _mlConfig;
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="mlConfig">配置对象</param>
    public MLServiceDbContext(IConfiguration mlConfig)
    {
        _mlConfig = mlConfig;
    }

    /// <summary>
    /// 数据库连接配置
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var serverVersion = ServerVersion.AutoDetect(_mlConfig.GetConnectionString("MySQLConnection"));
        optionsBuilder.UseMySql(_mlConfig.GetConnectionString("MySQLConnection"), serverVersion);
    }
}