using Microsoft.EntityFrameworkCore;
using SP.CurrencyService.Models.Entity;

namespace SP.CurrencyService.DB;

/// <summary>
/// 币种服务数据库上下文
/// </summary>
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
        // 使用固定 ID 和固定时间，确保每次运行 `HasData` 的结果确定性一致，避免重复迁移
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Currency>().HasData(new List<Currency>()
        {
            new Currency()
            {
                Id = 1000001L,
                Name = "人民币",
                Abbreviation = "CNY",
                CreateUserId = adminUserId,
                CreateDateTime = seedDate
            },
            new Currency()
            {
                Id = 1000002L,
                Name = "美元",
                Abbreviation = "USD",
                CreateUserId = adminUserId,
                CreateDateTime = seedDate
            },
            new Currency()
            {
                Id = 1000003L,
                Name = "欧元",
                Abbreviation = "EUR",
                CreateUserId = adminUserId,
                CreateDateTime = seedDate
            },
            new Currency()
            {
                Id = 1000004L,
                Name = "日元",
                Abbreviation = "JPY",
                CreateUserId = adminUserId,
                CreateDateTime = seedDate
            },
            new Currency()
            {
                Id = 1000005L,
                Name = "英镑",
                Abbreviation = "GBP",
                CreateUserId = adminUserId,
                CreateDateTime = seedDate
            },
            new Currency()
            {
                Id = 1000006L,
                Name = "澳门币",
                Abbreviation = "MOP",
                CreateUserId = adminUserId,
                CreateDateTime = seedDate
            },
            new Currency()
            {
                Id = 1000007L,
                Name = "港元",
                Abbreviation = "HKD",
                CreateUserId = adminUserId,
                CreateDateTime = seedDate
            },
            new Currency()
            {
                Id = 1000008L,
                Name = "韩圆",
                Abbreviation = "KRW",
                CreateUserId = adminUserId,
                CreateDateTime = seedDate
            },
            new Currency()
            {
                Id = 1000009L,
                Name = "新台币",
                Abbreviation = "TWD",
                CreateUserId = adminUserId,
                CreateDateTime = seedDate
            }
        });
    }
}