using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SP.Common;

namespace SP.IdentityService.DB;

/// <summary>
/// IdentityServer数据库上下文
/// </summary>
public class IdentityServerDbContext : DbContext
{
    /// <summary>
    /// 数据库连接配置
    /// </summary>
    IConfiguration _dbConfig;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dbConfig"></param>
    public IdentityServerDbContext(IConfiguration dbConfig)
    {
        _dbConfig = dbConfig;
    }

    /// <summary>
    /// 配置数据库模型
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // 配置 OpenIddict
        modelBuilder.UseOpenIddict();
        // 配置 IdentityUserRole<long> 的主键
        modelBuilder.Entity<IdentityUserRole<long>>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });
        // 配置表名
        modelBuilder.Entity<IdentityUser<long>>().ToTable("IdentityUsers");
        modelBuilder.Entity<IdentityRole<long>>().ToTable("IdentityRoles");
        modelBuilder.Entity<IdentityUserRole<long>>().ToTable("IdentityUserRoles");
        // 创建管理员账号
        var adminUser = new IdentityUser<long>
        {
            Id = Snow.GetId(),
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "1234567890@qq.com"
        };
        // 创建管理员角色
        var adminRole = new IdentityRole<long>
        {
            Id = Snow.GetId(),
            Name = "admin",
            NormalizedName = "ADMIN"
        };
        // 设置管理员账号密码
        var passwordHasher = new PasswordHasher<IdentityUser<long>>();
        adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "123*asdasd");
        // 设置管理员的管理员角色
        IdentityUserRole<long> userRole = new IdentityUserRole<long>
        {
            UserId = adminUser.Id,
            RoleId = adminRole.Id
        };
        modelBuilder.Entity<IdentityUser<long>>().HasData(adminUser);
        modelBuilder.Entity<IdentityRole<long>>().HasData(adminRole);
        modelBuilder.Entity<IdentityUserRole<long>>().HasData(userRole);
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