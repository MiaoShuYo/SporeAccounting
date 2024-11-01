using Microsoft.EntityFrameworkCore;
using SporeAccounting.BaseModels;
using SporeAccounting.Models;
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

    IConfiguration _dbConfig;
    public SporeAccountingDBContext(IConfiguration dbConfig)
    {
        _dbConfig = dbConfig;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        string adminUserId= Guid.NewGuid().ToString();
        string salt = Guid.NewGuid().ToString("N");
        string adminId = Guid.NewGuid().ToString();
        modelBuilder.Entity<SysRole>().HasData(new List<SysRole>()
        {
            new SysRole()
            {
                Id=adminId,
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
                CreateUserId =adminUserId
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