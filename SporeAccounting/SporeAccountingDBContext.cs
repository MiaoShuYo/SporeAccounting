using Microsoft.EntityFrameworkCore;
using SporeAccounting.Models;

namespace SporeAccounting;

/// <summary>
/// 数据库连接上下文
/// </summary>
public class SporeAccountingDBContext : DbContext
{
    private DbSet<SysUser> SysUsers { get; set; }

    IConfiguration _dbConfig;
    public SporeAccountingDBContext(IConfiguration dbConfig)
    {
        _dbConfig = dbConfig;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var serverVersion = ServerVersion.AutoDetect(_dbConfig.GetConnectionString("MySQLConnection"));
        optionsBuilder.UseMySql(_dbConfig.GetConnectionString("MySQLConnection"), serverVersion);
        optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder =>
        {
            //控制台打印SQL语句
            builder.AddConsole();
        }));

    }
}