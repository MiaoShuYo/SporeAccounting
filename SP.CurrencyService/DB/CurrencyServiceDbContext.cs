using Microsoft.EntityFrameworkCore;
using SP.Common;
using SP.CurrencyService.Models.Entity;

namespace SP.CurrencyService.DB;

public class CurrencyServiceDbContext : DbContext
{
    /// <summary>
    /// 汇率记录
    /// </summary>
    public DbSet<ExchangeRateRecord> ExchangeRateRecords { get; set; }

    /// <summary>
    /// 货币
    /// </summary>
    public DbSet<Currency> Currencies { get; set; }

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
    
    /// <summary>
    /// 模型创建
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        SeedData(modelBuilder);
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// 种子数据
    /// </summary>
    /// <param name="modelBuilder"></param>
    private void SeedData(ModelBuilder modelBuilder)
    {
        long adminUserId = 7333155174099406848;
        modelBuilder.Entity<Currency>().HasData(new List<Currency>()
        {
            new Currency()
            {
                Id = Snow.GetId(),
                Name = "人民币",
                Abbreviation = "CNY",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = Snow.GetId(),
                Name = "美元",
                Abbreviation = "USD",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = Snow.GetId(),
                Name = "欧元",
                Abbreviation = "EUR",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = Snow.GetId(),
                Name = "日元",
                Abbreviation = "JPY",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = Snow.GetId(),
                Name = "英镑",
                Abbreviation = "GBP",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = Snow.GetId(),
                Name = "澳门币",
                Abbreviation = "MOP",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = Snow.GetId(),
                Name = "港元",
                Abbreviation = "HKD",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = Snow.GetId(),
                Name = "韩圆",
                Abbreviation = "KRW",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = Snow.GetId(),
                Name = "新台币",
                Abbreviation = "TWD",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            }
        });
    }
}