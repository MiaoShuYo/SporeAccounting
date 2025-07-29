using Microsoft.EntityFrameworkCore;
using SP.ReportService.Models.Entity;

namespace SP.ReportService.DB;

/// <summary>
/// 报表服务数据库上下文
/// </summary>
public class ReportServiceDBContext:DbContext
{
    
    public DbSet<Report> Reports { get; set; }
    
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    private readonly IConfiguration _dbConfig;
    
    public ReportServiceDBContext(IConfiguration dbConfig)
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