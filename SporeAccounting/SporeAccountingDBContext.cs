using Microsoft.EntityFrameworkCore;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
using SporeAccounting.Server;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;

namespace SporeAccounting;

/// <summary>
/// 数据库连接上下文
/// </summary>
public class SporeAccountingDBContext : DbContext
{
    /// <summary>
    /// 用户表
    /// </summary>
    public DbSet<SysUser> SysUsers { get; set; }

    /// <summary>
    /// 角色表
    /// </summary>
    public DbSet<SysRole> SysRoles { get; set; }

    /// <summary>
    /// 角色可访问径表
    /// </summary>
    public DbSet<SysRoleUrl> SysRoleUrls { get; set; }

    /// <summary>
    /// 接口URL表
    /// </summary>
    public DbSet<SysUrl> SysUrls { get; set; }

    /// <summary>
    /// 汇率记录表
    /// </summary>
    public DbSet<ExchangeRateRecord> ExchangeRateRecords { get; set; }

    /// <summary>
    /// 币种表
    /// </summary>
    public DbSet<Currency> Currencies { get; set; }
    /// <summary>
    /// 用户配置表
    /// </summary>
    public DbSet<Config> Configs { get; set; }

    public DbSet<IncomeExpenditureClassification> IncomeExpenditureClassifications { get; set; }

    IConfiguration _dbConfig;

    public SporeAccountingDBContext(IConfiguration dbConfig)
    {
        _dbConfig = dbConfig;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        string adminUserId = Guid.NewGuid().ToString();
        string salt = Guid.NewGuid().ToString("N");
        string adminId = Guid.NewGuid().ToString();
        modelBuilder.Entity<SysRole>().HasData(new List<SysRole>()
        {
            new SysRole()
            {
                Id = adminId,
                RoleName = "Administrator",
                CanDelete = false,
                IsDeleted = false,
                CreateDateTime = DateTime.Now,
                CreateUserId = adminUserId
            },
            new SysRole()
            {
                RoleName = "Consumer",
                CanDelete = false,
                IsDeleted = false,
                CreateDateTime = DateTime.Now,
                CreateUserId = adminUserId
            }
        });
        modelBuilder.Entity<SysUser>().HasData(
            new SysUser
            {
                Id = adminUserId,
                UserName = "admin",
                Email = "admin@miaoshu.xyz",
                PhoneNumber = "",
                RoleId = adminId,
                IsDeleted = false,
                CanDelete = false,
                CreateDateTime = DateTime.Now,
                CreateUserId = adminUserId,
                Salt = salt,
                Password = HashPasswordWithSalt("123asdasd", salt),
            }
        );
        modelBuilder.Entity<IncomeExpenditureClassification>().HasData(new IncomeExpenditureClassification
        {
            Id = Guid.NewGuid().ToString(),
            Name = "其他",
            Type = IncomeExpenditureTypeEnmu.Other,
            CreateDateTime = DateTime.Now,
            CreateUserId = adminUserId,
            CanDelete = false,
        });

        modelBuilder.Entity<Currency>().HasData(new List<Currency>()
        {
            new Currency()
            {
                Name = "人民币",
                Abbreviation = "CNY",
                CreateUserId = adminId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Name = "美元",
                Abbreviation = "USD",
                CreateUserId = adminId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Name = "欧元",
                Abbreviation = "EUR",
                CreateUserId = adminId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Name = "日元",
                Abbreviation = "JPY",
                CreateUserId = adminId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Name = "英镑",
                Abbreviation = "GBP",
                CreateUserId = adminId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Name = "澳门币",
                Abbreviation = "MOP",
                CreateUserId = adminId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Name = "港元",
                Abbreviation = "HKD",
                CreateUserId = adminId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Name = "韩圆",
                Abbreviation = "KRW",
                CreateUserId = adminId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Name = "新台币",
                Abbreviation = "TWD",
                CreateUserId = adminId,
                CreateDateTime = DateTime.Now
            }
        });
        base.OnModelCreating(modelBuilder);
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

    private static string HashPasswordWithSalt(string password, string salt)
    {
        using (var sha256 = SHA256.Create())
        {
            string saltedPassword = password + salt;
            byte[] saltedPasswordBytes = Encoding.UTF8.GetBytes(saltedPassword);
            byte[] hashBytes = sha256.ComputeHash(saltedPasswordBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}