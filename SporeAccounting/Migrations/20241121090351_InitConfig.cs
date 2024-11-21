﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "1265bb8c-492f-4ead-959d-4b590933e3bd");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "1549bf32-772b-42b9-ab63-965e3275fb40");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "3c3459a1-71d4-42de-ac06-1fefa680f0bd");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "3e04f1f5-5aa7-42dd-802c-e59c8d72aee9");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "6f8b420d-91ea-492b-bd42-bcf5baed4305");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "8866428c-e3b8-4b54-87ca-b602f6ddf895");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a22f451d-27cc-455d-860c-15caab73330c");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a27f9290-3eb4-454d-b879-1cc1af2cc272");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "b7dc9376-f54e-4d20-8b91-0a73970d6f3a");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "895fe1d5-bebb-480d-b0c8-895024501b13");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "ecef295d-4ef0-4eb1-a829-2c9b6bdf594d");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "236e4d5c-b7e2-4f57-a92e-ccb6b1ff460b");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "865db296-5455-4444-8022-b96f3cebdcfb");

            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ConfigTypeEnum = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateUserId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeleteUserId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "0382fc54-9a3b-49be-8ac4-7a1307cd2481", "CNY", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(886), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "人民币", null, null },
                    { "2abdc350-2b4e-462e-a8fd-3490dc391db2", "GBP", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(912), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "英镑", null, null },
                    { "3e6726b7-5583-4011-95f0-5c8851eb13eb", "USD", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(893), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "美元", null, null },
                    { "4225e640-f0f4-4151-bc9d-ee156b2f1fb4", "KRW", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(935), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "韩圆", null, null },
                    { "61bc551c-ecb5-4120-8b7d-fa35bc1307b0", "EUR", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(900), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "欧元", null, null },
                    { "a1c46451-e57f-418b-9e8c-b22587832dea", "HKD", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(927), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "港元", null, null },
                    { "b23014ed-08bd-4db3-aa4a-5432db0176fa", "TWD", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(941), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "新台币", null, null },
                    { "e288fab2-3239-4be0-9682-dcb53e8c8cd2", "JPY", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(906), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "日元", null, null },
                    { "e37cd628-00e0-4036-b952-6f44038a549a", "MOP", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(920), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "澳门币", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "e3e389f7-8e56-4492-acf9-28220459b044", false, new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(832), "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "1c9bb153-ba4c-458b-8f24-67de6d09ba6e", false, new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(28), "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", null, null, false, "Consumer", null, null },
                    { "484488fe-9930-4d4f-bf1a-59a4fedc7529", false, new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(19), "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", null, null, false, "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", false, new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(76), "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", null, null, "admin@miaoshu.xyz", false, "9rVToUu4HHPeOTJuvQXlJ8ovy5kkwXqARFzBZjp3fFY=", "", "484488fe-9930-4d4f-bf1a-59a4fedc7529", "637768579c8e4d9091dc9d132ce3d9de", null, null, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "0382fc54-9a3b-49be-8ac4-7a1307cd2481");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "2abdc350-2b4e-462e-a8fd-3490dc391db2");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "3e6726b7-5583-4011-95f0-5c8851eb13eb");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4225e640-f0f4-4151-bc9d-ee156b2f1fb4");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "61bc551c-ecb5-4120-8b7d-fa35bc1307b0");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a1c46451-e57f-418b-9e8c-b22587832dea");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "b23014ed-08bd-4db3-aa4a-5432db0176fa");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e288fab2-3239-4be0-9682-dcb53e8c8cd2");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e37cd628-00e0-4036-b952-6f44038a549a");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "e3e389f7-8e56-4492-acf9-28220459b044");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "1c9bb153-ba4c-458b-8f24-67de6d09ba6e");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "c8873ec9-cf40-49d1-b8e5-4507e9d25b53");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "484488fe-9930-4d4f-bf1a-59a4fedc7529");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "1265bb8c-492f-4ead-959d-4b590933e3bd", "KRW", new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(551), "865db296-5455-4444-8022-b96f3cebdcfb", null, null, false, "韩圆", null, null },
                    { "1549bf32-772b-42b9-ab63-965e3275fb40", "USD", new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(505), "865db296-5455-4444-8022-b96f3cebdcfb", null, null, false, "美元", null, null },
                    { "3c3459a1-71d4-42de-ac06-1fefa680f0bd", "CNY", new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(486), "865db296-5455-4444-8022-b96f3cebdcfb", null, null, false, "人民币", null, null },
                    { "3e04f1f5-5aa7-42dd-802c-e59c8d72aee9", "GBP", new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(524), "865db296-5455-4444-8022-b96f3cebdcfb", null, null, false, "英镑", null, null },
                    { "6f8b420d-91ea-492b-bd42-bcf5baed4305", "TWD", new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(557), "865db296-5455-4444-8022-b96f3cebdcfb", null, null, false, "新台币", null, null },
                    { "8866428c-e3b8-4b54-87ca-b602f6ddf895", "HKD", new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(545), "865db296-5455-4444-8022-b96f3cebdcfb", null, null, false, "港元", null, null },
                    { "a22f451d-27cc-455d-860c-15caab73330c", "EUR", new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(512), "865db296-5455-4444-8022-b96f3cebdcfb", null, null, false, "欧元", null, null },
                    { "a27f9290-3eb4-454d-b879-1cc1af2cc272", "MOP", new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(538), "865db296-5455-4444-8022-b96f3cebdcfb", null, null, false, "澳门币", null, null },
                    { "b7dc9376-f54e-4d20-8b91-0a73970d6f3a", "JPY", new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(518), "865db296-5455-4444-8022-b96f3cebdcfb", null, null, false, "日元", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "895fe1d5-bebb-480d-b0c8-895024501b13", false, new DateTime(2024, 11, 21, 13, 59, 46, 580, DateTimeKind.Local).AddTicks(464), "236e4d5c-b7e2-4f57-a92e-ccb6b1ff460b", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "865db296-5455-4444-8022-b96f3cebdcfb", false, new DateTime(2024, 11, 21, 13, 59, 46, 579, DateTimeKind.Local).AddTicks(9584), "236e4d5c-b7e2-4f57-a92e-ccb6b1ff460b", null, null, false, "Administrator", null, null },
                    { "ecef295d-4ef0-4eb1-a829-2c9b6bdf594d", false, new DateTime(2024, 11, 21, 13, 59, 46, 579, DateTimeKind.Local).AddTicks(9603), "236e4d5c-b7e2-4f57-a92e-ccb6b1ff460b", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "236e4d5c-b7e2-4f57-a92e-ccb6b1ff460b", false, new DateTime(2024, 11, 21, 13, 59, 46, 579, DateTimeKind.Local).AddTicks(9659), "236e4d5c-b7e2-4f57-a92e-ccb6b1ff460b", null, null, "admin@miaoshu.xyz", false, "gKXP4spr6oPv3q5WH/9Pa6bILVVMkudkQ7SU32voE6E=", "", "865db296-5455-4444-8022-b96f3cebdcfb", "51f48b4501fb49c18f453e12fa429ccf", null, null, "admin" });
        }
    }
}
