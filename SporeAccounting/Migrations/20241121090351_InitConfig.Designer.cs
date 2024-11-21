﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SporeAccounting;

#nullable disable

namespace SporeAccounting.Migrations
{
    [DbContext(typeof(SporeAccountingDBContext))]
    [Migration("20241121090351_InitConfig")]
    partial class InitConfig
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("SporeAccounting.Models.Config", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("ConfigTypeEnum")
                        .HasColumnType("int");

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

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Config");
                });

            modelBuilder.Entity("SporeAccounting.Models.Currency", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.ToTable("Currency");

                    b.HasData(
                        new
                        {
                            Id = "0382fc54-9a3b-49be-8ac4-7a1307cd2481",
                            Abbreviation = "CNY",
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(886),
                            CreateUserId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            IsDeleted = false,
                            Name = "人民币"
                        },
                        new
                        {
                            Id = "3e6726b7-5583-4011-95f0-5c8851eb13eb",
                            Abbreviation = "USD",
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(893),
                            CreateUserId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            IsDeleted = false,
                            Name = "美元"
                        },
                        new
                        {
                            Id = "61bc551c-ecb5-4120-8b7d-fa35bc1307b0",
                            Abbreviation = "EUR",
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(900),
                            CreateUserId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            IsDeleted = false,
                            Name = "欧元"
                        },
                        new
                        {
                            Id = "e288fab2-3239-4be0-9682-dcb53e8c8cd2",
                            Abbreviation = "JPY",
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(906),
                            CreateUserId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            IsDeleted = false,
                            Name = "日元"
                        },
                        new
                        {
                            Id = "2abdc350-2b4e-462e-a8fd-3490dc391db2",
                            Abbreviation = "GBP",
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(912),
                            CreateUserId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            IsDeleted = false,
                            Name = "英镑"
                        },
                        new
                        {
                            Id = "e37cd628-00e0-4036-b952-6f44038a549a",
                            Abbreviation = "MOP",
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(920),
                            CreateUserId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            IsDeleted = false,
                            Name = "澳门币"
                        },
                        new
                        {
                            Id = "a1c46451-e57f-418b-9e8c-b22587832dea",
                            Abbreviation = "HKD",
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(927),
                            CreateUserId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            IsDeleted = false,
                            Name = "港元"
                        },
                        new
                        {
                            Id = "4225e640-f0f4-4151-bc9d-ee156b2f1fb4",
                            Abbreviation = "KRW",
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(935),
                            CreateUserId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            IsDeleted = false,
                            Name = "韩圆"
                        },
                        new
                        {
                            Id = "b23014ed-08bd-4db3-aa4a-5432db0176fa",
                            Abbreviation = "TWD",
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(941),
                            CreateUserId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            IsDeleted = false,
                            Name = "新台币"
                        });
                });

            modelBuilder.Entity("SporeAccounting.Models.ExchangeRateRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("ConvertCurrency")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("CreateUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<DateTime?>("DeleteDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("DeleteUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<decimal>("ExchangeRate")
                        .HasColumnType("decimal(10,2)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.ToTable("ExchangeRate");
                });

            modelBuilder.Entity("SporeAccounting.Models.IncomeExpenditureClassification", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<bool>("CanDelete")
                        .HasColumnType("tinyint(1)");

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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ParentClassificationId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("ParentId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.HasIndex("ParentId");

                    b.ToTable("IncomeExpenditureClassification");

                    b.HasData(
                        new
                        {
                            Id = "e3e389f7-8e56-4492-acf9-28220459b044",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(832),
                            CreateUserId = "c8873ec9-cf40-49d1-b8e5-4507e9d25b53",
                            IsDeleted = false,
                            Name = "其他",
                            Type = -1
                        });
                });

            modelBuilder.Entity("SporeAccounting.Models.SysRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<bool>("CanDelete")
                        .HasColumnType("tinyint(1)");

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
                            Id = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(19),
                            CreateUserId = "c8873ec9-cf40-49d1-b8e5-4507e9d25b53",
                            IsDeleted = false,
                            RoleName = "Administrator"
                        },
                        new
                        {
                            Id = "1c9bb153-ba4c-458b-8f24-67de6d09ba6e",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(28),
                            CreateUserId = "c8873ec9-cf40-49d1-b8e5-4507e9d25b53",
                            IsDeleted = false,
                            RoleName = "Consumer"
                        });
                });

            modelBuilder.Entity("SporeAccounting.Models.SysRoleUrl", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<bool>("CanDelete")
                        .HasColumnType("tinyint(1)");

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

                    b.Property<string>("UrlId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("UrlId");

                    b.ToTable("SysRoleUrl");
                });

            modelBuilder.Entity("SporeAccounting.Models.SysUrl", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("CanDelete")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("CreateUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("DeleteDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("DeleteUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("SysUrl");
                });

            modelBuilder.Entity("SporeAccounting.Models.SysUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<bool>("CanDelete")
                        .HasColumnType("tinyint(1)");

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
                            Id = "c8873ec9-cf40-49d1-b8e5-4507e9d25b53",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(76),
                            CreateUserId = "c8873ec9-cf40-49d1-b8e5-4507e9d25b53",
                            Email = "admin@miaoshu.xyz",
                            IsDeleted = false,
                            Password = "9rVToUu4HHPeOTJuvQXlJ8ovy5kkwXqARFzBZjp3fFY=",
                            PhoneNumber = "",
                            RoleId = "484488fe-9930-4d4f-bf1a-59a4fedc7529",
                            Salt = "637768579c8e4d9091dc9d132ce3d9de",
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("SporeAccounting.Models.IncomeExpenditureClassification", b =>
                {
                    b.HasOne("SporeAccounting.Models.IncomeExpenditureClassification", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("SporeAccounting.Models.SysRoleUrl", b =>
                {
                    b.HasOne("SporeAccounting.Models.SysRole", "Role")
                        .WithMany("RoleUrls")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SporeAccounting.Models.SysUrl", "Url")
                        .WithMany("RoleUrls")
                        .HasForeignKey("UrlId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("Url");
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

            modelBuilder.Entity("SporeAccounting.Models.IncomeExpenditureClassification", b =>
                {
                    b.Navigation("Children");
                });

            modelBuilder.Entity("SporeAccounting.Models.SysRole", b =>
                {
                    b.Navigation("RoleUrls");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("SporeAccounting.Models.SysUrl", b =>
                {
                    b.Navigation("RoleUrls");
                });
#pragma warning restore 612, 618
        }
    }
}