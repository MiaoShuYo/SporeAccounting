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
    [Migration("20241125162736_AddRequestMethod")]
    partial class AddRequestMethod
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("SporeAccounting.Models.AccountBook", b =>
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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Remarks")
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AccountBook");
                });

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

                    b.HasIndex("UserId");

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
                            Id = "6f89370d-c248-417e-b6df-5633990a9492",
                            Abbreviation = "CNY",
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9012),
                            CreateUserId = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            IsDeleted = false,
                            Name = "人民币"
                        },
                        new
                        {
                            Id = "c3124663-b1b4-4cb9-9846-6e1e97a4ac49",
                            Abbreviation = "USD",
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9023),
                            CreateUserId = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            IsDeleted = false,
                            Name = "美元"
                        },
                        new
                        {
                            Id = "17c2487c-4576-4102-9f02-5e6cbd12ac52",
                            Abbreviation = "EUR",
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9032),
                            CreateUserId = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            IsDeleted = false,
                            Name = "欧元"
                        },
                        new
                        {
                            Id = "79885c41-4a36-4b92-9621-37fefec22605",
                            Abbreviation = "JPY",
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9040),
                            CreateUserId = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            IsDeleted = false,
                            Name = "日元"
                        },
                        new
                        {
                            Id = "775320b6-2ecd-49bc-8f98-78afce82cdf7",
                            Abbreviation = "GBP",
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9050),
                            CreateUserId = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            IsDeleted = false,
                            Name = "英镑"
                        },
                        new
                        {
                            Id = "372f33e9-2db1-4f03-83e2-1ea5fbcdfea5",
                            Abbreviation = "MOP",
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9064),
                            CreateUserId = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            IsDeleted = false,
                            Name = "澳门币"
                        },
                        new
                        {
                            Id = "ea561a2f-5afd-4062-b1d9-1d7fa953bbcb",
                            Abbreviation = "HKD",
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9130),
                            CreateUserId = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            IsDeleted = false,
                            Name = "港元"
                        },
                        new
                        {
                            Id = "cb840646-b0d5-4cb2-82ea-25062d29e68b",
                            Abbreviation = "KRW",
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9139),
                            CreateUserId = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            IsDeleted = false,
                            Name = "韩圆"
                        },
                        new
                        {
                            Id = "36ef32b5-e6f2-404e-b421-ccde00a7080f",
                            Abbreviation = "TWD",
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9148),
                            CreateUserId = "cef80881-fe89-4b1f-85ad-83184777d61b",
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
                            Id = "c9f9b44c-4edb-4c9f-9610-0183a8abb912",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(8976),
                            CreateUserId = "b47637e2-603f-4df0-abe9-88d70fa870ee",
                            IsDeleted = false,
                            Name = "其他",
                            Type = -1
                        });
                });

            modelBuilder.Entity("SporeAccounting.Models.IncomeExpenditureRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("AccountBookId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<decimal>("AfterAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("BeforAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("CreateUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("CurrencyId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<DateTime?>("DeleteDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("DeleteUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("IncomeExpenditureClassificationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime>("RecordDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Remark")
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("UpdateDateTime")
                        .HasColumnType("datetime");

                    b.Property<string>("UpdateUserId")
                        .HasColumnType("nvarchar(36)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(36)");

                    b.HasKey("Id");

                    b.HasIndex("AccountBookId");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("IncomeExpenditureClassificationId");

                    b.HasIndex("UserId");

                    b.ToTable("IncomeExpenditureRecord");
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
                            Id = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(7617),
                            CreateUserId = "b47637e2-603f-4df0-abe9-88d70fa870ee",
                            IsDeleted = false,
                            RoleName = "Administrator"
                        },
                        new
                        {
                            Id = "10389aa0-b6f2-4241-9a77-ca8020656bb6",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(7638),
                            CreateUserId = "b47637e2-603f-4df0-abe9-88d70fa870ee",
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

                    b.Property<string>("RequestMethod")
                        .IsRequired()
                        .HasColumnType("nvarchar(10)");

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
                            Id = "b47637e2-603f-4df0-abe9-88d70fa870ee",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(7709),
                            CreateUserId = "b47637e2-603f-4df0-abe9-88d70fa870ee",
                            Email = "admin@miaoshu.xyz",
                            IsDeleted = false,
                            Password = "JibbEaukgua7Y8ylDFE9DkvRgSWgcLnI2M1xL9IvQe4=",
                            PhoneNumber = "",
                            RoleId = "cef80881-fe89-4b1f-85ad-83184777d61b",
                            Salt = "e3c85037b35f46a7b464e06243b5eecc",
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("SporeAccounting.Models.AccountBook", b =>
                {
                    b.HasOne("SporeAccounting.Models.SysUser", "User")
                        .WithMany("AccountBooks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SporeAccounting.Models.Config", b =>
                {
                    b.HasOne("SporeAccounting.Models.SysUser", "User")
                        .WithMany("Configs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SporeAccounting.Models.IncomeExpenditureClassification", b =>
                {
                    b.HasOne("SporeAccounting.Models.IncomeExpenditureClassification", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("SporeAccounting.Models.IncomeExpenditureRecord", b =>
                {
                    b.HasOne("SporeAccounting.Models.AccountBook", "AccountBook")
                        .WithMany("IncomeExpenditureRecords")
                        .HasForeignKey("AccountBookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SporeAccounting.Models.Currency", "Currency")
                        .WithMany("IncomeExpenditureRecords")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SporeAccounting.Models.IncomeExpenditureClassification", "IncomeExpenditureClassification")
                        .WithMany("IncomeExpenditureRecords")
                        .HasForeignKey("IncomeExpenditureClassificationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SporeAccounting.Models.SysUser", "User")
                        .WithMany("IncomeExpenditureRecords")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AccountBook");

                    b.Navigation("Currency");

                    b.Navigation("IncomeExpenditureClassification");

                    b.Navigation("User");
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

            modelBuilder.Entity("SporeAccounting.Models.AccountBook", b =>
                {
                    b.Navigation("IncomeExpenditureRecords");
                });

            modelBuilder.Entity("SporeAccounting.Models.Currency", b =>
                {
                    b.Navigation("IncomeExpenditureRecords");
                });

            modelBuilder.Entity("SporeAccounting.Models.IncomeExpenditureClassification", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("IncomeExpenditureRecords");
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

            modelBuilder.Entity("SporeAccounting.Models.SysUser", b =>
                {
                    b.Navigation("AccountBooks");

                    b.Navigation("Configs");

                    b.Navigation("IncomeExpenditureRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
