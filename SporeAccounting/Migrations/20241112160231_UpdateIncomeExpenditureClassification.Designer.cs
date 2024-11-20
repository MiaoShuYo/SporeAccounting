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
    [Migration("20241112160231_UpdateIncomeExpenditureClassification")]
    partial class UpdateIncomeExpenditureClassification
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("SporeAccounting.Models.IncomeExpenditureClassification", b =>
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
                            Id = "0d6511fe-cc70-4514-abc7-6ef02e4b1552",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 13, 0, 2, 28, 724, DateTimeKind.Local).AddTicks(8046),
                            CreateUserId = "c1cc21f5-53df-4b84-9f30-1c3dbef072f8",
                            IsDeleted = false,
                            RoleName = "Administrator"
                        },
                        new
                        {
                            Id = "64f33311-1e28-4f4a-8d3d-faa113864652",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 13, 0, 2, 28, 724, DateTimeKind.Local).AddTicks(8060),
                            CreateUserId = "c1cc21f5-53df-4b84-9f30-1c3dbef072f8",
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
                            Id = "c1cc21f5-53df-4b84-9f30-1c3dbef072f8",
                            CanDelete = false,
                            CreateDateTime = new DateTime(2024, 11, 13, 0, 2, 28, 724, DateTimeKind.Local).AddTicks(8106),
                            CreateUserId = "c1cc21f5-53df-4b84-9f30-1c3dbef072f8",
                            Email = "admin@miaoshu.xyz",
                            IsDeleted = false,
                            Password = "4eGevupeD4LV439DS6/shNOc7qYBLA5W+niLbe35gFU=",
                            PhoneNumber = "",
                            RoleId = "0d6511fe-cc70-4514-abc7-6ef02e4b1552",
                            Salt = "847a360593bb408589aeecccb9f650ef",
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