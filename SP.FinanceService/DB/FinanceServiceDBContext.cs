using Microsoft.EntityFrameworkCore;
using SP.FinanceService.Models.Entity;

namespace SP.FinanceService.DB;

/// <summary>
/// 财务服务数据库上下文
/// </summary>
public class FinanceServiceDbContext : DbContext
{
    /// <summary>
    /// 收支分类
    /// </summary>
    public DbSet<TransactionCategory> TransactionCategories { get; set; }

    /// <summary>
    /// 账本
    /// </summary>
    public DbSet<AccountBook> AccountBooks { get; set; }

    /// <summary>
    /// 财务记录
    /// </summary>
    public DbSet<Accounting> Accountings { get; set; }


    /// <summary>
    /// 数据库连接配置
    /// </summary>
    private readonly IConfiguration _dbConfig;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbConfig"></param>
    public FinanceServiceDbContext(IConfiguration dbConfig)
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