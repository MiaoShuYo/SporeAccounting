using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SP.Common;
using SP.IdentityService.Models;

namespace SP.IdentityService.DB;

/// <summary>
/// IdentityServer数据库上下文
/// </summary>
public class IdentityServerDbContext : IdentityDbContext<SpUser, SpRole, long>
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
        SeedData(modelBuilder);
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

    private void SeedData(ModelBuilder builder)
    {
        // 添加默认角色
        SpRole adminRole = new SpRole { Id = Snow.GetId(), Name = "Admin", NormalizedName = "ADMIN" };
        SpRole userRole = new SpRole { Id = Snow.GetId(), Name = "User", NormalizedName = "USER" };
        builder.Entity<SpRole>().HasData(adminRole,userRole);

        // 添加默认用户
        var hasher = new PasswordHasher<SpUser>();
        SpUser adminUser = new SpUser
        {
            Id = Snow.GetId(),
            UserName = "admin",
            NormalizedUserName = "admin",
            Email = "494324190@qq.com",
            NormalizedEmail = "494324190@qq.com",
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null, "123*asdasd")
        };
        builder.Entity<SpUser>().HasData(adminUser);
        // 添加用户角色
        builder.Entity<IdentityUserRole<long>>().HasData(
            new IdentityUserRole<long>
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            }
        );
    }
}