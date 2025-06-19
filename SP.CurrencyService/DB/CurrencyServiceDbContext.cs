using Microsoft.EntityFrameworkCore;
using SP.CurrencyService.Models.Entity;

namespace SP.CurrencyService.DB;

public class CurrencyServiceDbContext : DbContext
{
    /// <summary>
    /// 汇率记录
    /// </summary>
    public DbSet<ExchangeRateRecord> ExchangeRateRecords { get; set; }

    /// <summary>
    /// 数据库连接配置
    /// </summary>
    private readonly IConfiguration _dbConfig;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbConfig"></param>
    public CurrencyServiceDbContext(IConfiguration dbConfig)
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