using Microsoft.EntityFrameworkCore;
using SP.ResourceService.Models.Entity;

namespace SP.ResourceService.DB;

/// <summary>
/// 资源服务数据库上下文
/// </summary>
public class ResourceServiceDbContext:DbContext
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    private readonly IConfiguration _dbConfig;
    
    public DbSet<Files> Files { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbConfig"></param>
    public ResourceServiceDbContext(IConfiguration dbConfig)
    {
        _dbConfig = dbConfig;
    }
    
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    /// <param name="optionsBuilder"></param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _dbConfig.GetConnectionString("MySQLConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("数据库连接字符串 'MySQLConnection' 未配置。请检查 Nacos 配置中心中的配置。");
        }
        
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        optionsBuilder.UseMySql(connectionString, serverVersion);
    }
}