using Microsoft.EntityFrameworkCore;
using SP.NotificationService.Models.Entity;

namespace SP.NotificationService.DB;

/// <summary>
/// 消息服务数据库上下文
/// </summary>
public class NotificationServiceDBContext:DbContext
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    private readonly IConfiguration _dbConfig;
    
    /// <summary>
    /// 站内通知
    /// </summary>
    public DbSet<InSiteNotification> InSiteNotifications { get; set; }
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbConfig"></param>
    public NotificationServiceDBContext(IConfiguration dbConfig)
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