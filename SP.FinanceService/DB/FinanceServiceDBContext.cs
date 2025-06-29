using Microsoft.EntityFrameworkCore;
using SP.FinanceService.Models.Entity;

namespace SP.FinanceService.DB;

/// <summary>
/// 财务服务数据库上下文
/// </summary>
public class FinanceServiceDBContext:DbContext
{
    
    public DbSet<TransactionCategory> TransactionCategories { get; set; }
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    private readonly IConfiguration _dbConfig;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbConfig"></param>
    public FinanceServiceDBContext(IConfiguration dbConfig)
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