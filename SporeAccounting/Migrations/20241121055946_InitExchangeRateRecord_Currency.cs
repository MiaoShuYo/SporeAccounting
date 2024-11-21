using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitExchangeRateRecord_Currency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "4de809b7-2dda-4e64-b6c0-9a5198abecb8");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "81017bf4-8b16-4571-a704-043b89bd0254");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "7c976fa1-255e-46a1-b4a7-73d88a75f519");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "227267c6-2575-4fc4-ac2e-fad1e4926606");

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", nullable: false),
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
                    table.PrimaryKey("PK_Currency", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExchangeRate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ConvertCurrency = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
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
                    table.PrimaryKey("PK_ExchangeRate", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "ExchangeRate");

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

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "4de809b7-2dda-4e64-b6c0-9a5198abecb8", false, new DateTime(2024, 11, 13, 0, 13, 46, 837, DateTimeKind.Local).AddTicks(8164), "7c976fa1-255e-46a1-b4a7-73d88a75f519", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "227267c6-2575-4fc4-ac2e-fad1e4926606", false, new DateTime(2024, 11, 13, 0, 13, 46, 837, DateTimeKind.Local).AddTicks(7164), "7c976fa1-255e-46a1-b4a7-73d88a75f519", null, null, false, "Administrator", null, null },
                    { "81017bf4-8b16-4571-a704-043b89bd0254", false, new DateTime(2024, 11, 13, 0, 13, 46, 837, DateTimeKind.Local).AddTicks(7175), "7c976fa1-255e-46a1-b4a7-73d88a75f519", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "7c976fa1-255e-46a1-b4a7-73d88a75f519", false, new DateTime(2024, 11, 13, 0, 13, 46, 837, DateTimeKind.Local).AddTicks(7219), "7c976fa1-255e-46a1-b4a7-73d88a75f519", null, null, "admin@miaoshu.xyz", false, "5WeL3p+R/98wW2dkvK/XqjHF28wK8n7ucLxVFdVCno0=", "", "227267c6-2575-4fc4-ac2e-fad1e4926606", "0400965eb7514a16bf56d10dfc5f1b5a", null, null, "admin" });
        }
    }
}
