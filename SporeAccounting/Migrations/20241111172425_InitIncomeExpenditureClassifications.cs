using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitIncomeExpenditureClassifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "d5f0fc96-0a8f-4b8c-9555-a41642d547aa");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "22df3bad-66df-40a7-a3a1-c0c3d06e5a99");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "5e575fd7-8c2a-434a-afe3-f0daaad7adb2");

            migrationBuilder.AddColumn<bool>(
                name: "CanDelete",
                table: "SysRoleUrl",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "IncomeExpenditureClassification",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ParentClassificationId = table.Column<string>(type: "nvarchar(36)", nullable: true),
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
                    table.PrimaryKey("PK_IncomeExpenditureClassification", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "70aea7ea-3920-47c7-9f63-04e5199d7f8b", false, new DateTime(2024, 11, 12, 1, 24, 22, 769, DateTimeKind.Local).AddTicks(4871), "a9c6cfc3-4aa2-423b-b39b-d826fdd76046", null, null, false, "Administrator", null, null },
                    { "b294a994-6390-492a-8e55-a66b9181af04", false, new DateTime(2024, 11, 12, 1, 24, 22, 769, DateTimeKind.Local).AddTicks(4880), "a9c6cfc3-4aa2-423b-b39b-d826fdd76046", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "a9c6cfc3-4aa2-423b-b39b-d826fdd76046", false, new DateTime(2024, 11, 12, 1, 24, 22, 769, DateTimeKind.Local).AddTicks(4929), "a9c6cfc3-4aa2-423b-b39b-d826fdd76046", null, null, "admin@miaoshu.xyz", false, "pLvm6EDorcRhWNfBSrTBbnMorgnYRiWNxyrGVd4/ZMA=", "", "70aea7ea-3920-47c7-9f63-04e5199d7f8b", "a6812bb53e1d47ddaf2ceaee272c7e9b", null, null, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomeExpenditureClassification");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "b294a994-6390-492a-8e55-a66b9181af04");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "a9c6cfc3-4aa2-423b-b39b-d826fdd76046");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "70aea7ea-3920-47c7-9f63-04e5199d7f8b");

            migrationBuilder.DropColumn(
                name: "CanDelete",
                table: "SysRoleUrl");

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "5e575fd7-8c2a-434a-afe3-f0daaad7adb2", false, new DateTime(2024, 11, 9, 23, 12, 59, 238, DateTimeKind.Local).AddTicks(9634), "22df3bad-66df-40a7-a3a1-c0c3d06e5a99", null, null, false, "Administrator", null, null },
                    { "d5f0fc96-0a8f-4b8c-9555-a41642d547aa", false, new DateTime(2024, 11, 9, 23, 12, 59, 238, DateTimeKind.Local).AddTicks(9655), "22df3bad-66df-40a7-a3a1-c0c3d06e5a99", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "22df3bad-66df-40a7-a3a1-c0c3d06e5a99", false, new DateTime(2024, 11, 9, 23, 12, 59, 238, DateTimeKind.Local).AddTicks(9800), "22df3bad-66df-40a7-a3a1-c0c3d06e5a99", null, null, "admin@miaoshu.xyz", false, "xg5Ko6Blelj8ragUu37ks58Gwa+d8hnnJMK2de5/E8w=", "", "5e575fd7-8c2a-434a-afe3-f0daaad7adb2", "93bf761918924dd6825a52d4680ed6ac", null, null, "admin" });
        }
    }
}
