﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SporeAccounting;

#nullable disable

namespace SporeAccounting.Migrations
{
    [DbContext(typeof(SporeAccountingDBContext))]
    partial class SporeAccountingDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("SporeAccounting.Models.SysRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("CreateUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("DeleteDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("DeleteUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.ToTable("SysRole");

                    b.HasData(
                        new
                        {
                            Id = "05847d63-9df7-4083-a588-0feef57b0648",
                            CreateDateTime = new DateTime(2024, 10, 31, 23, 31, 16, 85, DateTimeKind.Local).AddTicks(7223),
                            CreateUserId = "26539465-d4e3-4224-9684-e3692d0772bd",
                            IsDeleted = false,
                            RoleName = "Administrator"
                        },
                        new
                        {
                            Id = "299e2358-c62c-43e4-8487-0eddc307c306",
                            CreateDateTime = new DateTime(2024, 10, 31, 23, 31, 16, 85, DateTimeKind.Local).AddTicks(7233),
                            CreateUserId = "26539465-d4e3-4224-9684-e3692d0772bd",
                            IsDeleted = false,
                            RoleName = "Consumer"
                        });
                });

            modelBuilder.Entity("SporeAccounting.Models.SysRoleUrl", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("CreateUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("DeleteDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("DeleteUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("SysRoleUrl");
                });

            modelBuilder.Entity("SporeAccounting.Models.SysUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("CreateUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("DeleteDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("DeleteUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("SysUser");

                    b.HasData(
                        new
                        {
                            Id = "26539465-d4e3-4224-9684-e3692d0772bd",
                            CreateDateTime = new DateTime(2024, 10, 31, 23, 31, 16, 85, DateTimeKind.Local).AddTicks(5734),
                            CreateUserId = "26539465-d4e3-4224-9684-e3692d0772bd",
                            Email = "admin@miaoshu.xyz",
                            IsDeleted = false,
                            Password = "6Xvz+X1e8ucTUwJsU7sRb1KJjd7WdPfnbqWiZTl9krw=",
                            PhoneNumber = "",
                            RoleId = "05847d63-9df7-4083-a588-0feef57b0648",
                            Salt = "554935227d2f4a3ea69d90195efce0a0",
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("SysRoleSysRoleUrl", b =>
                {
                    b.Property<string>("RoleUrlsId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("RolesId")
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("RoleUrlsId", "RolesId");

                    b.HasIndex("RolesId");

                    b.ToTable("SysRoleSysRoleUrl");
                });

            modelBuilder.Entity("SporeAccounting.Models.SysUser", b =>
                {
                    b.HasOne("SporeAccounting.Models.SysRole", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("SysRoleSysRoleUrl", b =>
                {
                    b.HasOne("SporeAccounting.Models.SysRoleUrl", null)
                        .WithMany()
                        .HasForeignKey("RoleUrlsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SporeAccounting.Models.SysRole", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SporeAccounting.Models.SysRole", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
