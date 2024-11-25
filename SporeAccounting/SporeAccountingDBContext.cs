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

    /// <summary>
    /// 账本表
    /// </summary>
    public DbSet<AccountBook> AccountBooks { get; set; }

    /// <summary>
    /// 收支记录表
    /// </summary>
    public DbSet<IncomeExpenditureRecord> IncomeExpenditureRecords { get; set; }

    public DbSet<IncomeExpenditureClassification> IncomeExpenditureClassifications { get; set; }

    IConfiguration _dbConfig;

    public SporeAccountingDBContext(IConfiguration dbConfig)
    {
        _dbConfig = dbConfig;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        string adminUserId = "b47637e2-603f-4df0-abe9-88d70fa870ee";
        string adminRoleId = "cef80881-fe89-4b1f-85ad-83184777d61b";
        string salt = Guid.NewGuid().ToString("N");
        modelBuilder.Entity<SysRole>().HasData(new List<SysRole>()
        {
            new SysRole()
            {
                Id = adminRoleId,
                RoleName = "Administrator",
                CanDelete = false,
                IsDeleted = false,
                CreateDateTime = DateTime.Now,
                CreateUserId = adminUserId
            },
            new SysRole()
            {
                Id = "10389aa0-b6f2-4241-9a77-ca8020656bb6",
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
                RoleId = adminRoleId,
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
            Id = "10ce6d08-3de2-466e-a9bb-e15cb4eec56f",
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
                Id = "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6",
                Name = "人民币",
                Abbreviation = "CNY",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = "e25b4885-cf61-4249-b86f-0130defd1d57",
                Name = "美元",
                Abbreviation = "USD",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = "409f7f1d-3430-4f82-9180-520ac1dadbc9",
                Name = "欧元",
                Abbreviation = "EUR",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = "422a920d-12e9-4263-a1b6-9d6e4e3366ea",
                Name = "日元",
                Abbreviation = "JPY",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = "4b6a9c6f-d77f-4087-af5d-2d4f85375bda",
                Name = "英镑",
                Abbreviation = "GBP",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = "551b2b37-dfd8-49df-bfc5-c78f068b2d01",
                Name = "澳门币",
                Abbreviation = "MOP",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = "098e71cf-4630-467b-a530-cea4b30e9070",
                Name = "港元",
                Abbreviation = "HKD",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = "7b01d6fa-e673-4bfd-8112-3e988971d91c",
                Name = "韩圆",
                Abbreviation = "KRW",
                CreateUserId = adminUserId,
                CreateDateTime = DateTime.Now
            },
            new Currency()
            {
                Id = "a374bbfa-99bd-4f14-9f11-49260528d7a4",
                Name = "新台币",
                Abbreviation = "TWD",
                CreateUserId = adminUserId,
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