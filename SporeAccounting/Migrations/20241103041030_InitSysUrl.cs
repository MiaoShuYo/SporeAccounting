using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitSysUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "af857136-ccb7-4567-b653-83af492ce849");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "7c089aeb-b383-4a58-8cae-e6987ac5a828");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "a82795b8-d07f-4f68-85ee-071ba050c3c4");

            migrationBuilder.CreateTable(
                name: "SysUrl",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", nullable: false),
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
                    table.PrimaryKey("PK_SysUrl", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "952db095-ed24-4afe-989d-425a95e29380", false, new DateTime(2024, 11, 3, 12, 10, 28, 834, DateTimeKind.Local).AddTicks(4753), "0ed877a5-982b-47bf-a6b5-983c1149ac75", null, null, false, "Administrator", null, null },
                    { "b253b817-2b3f-4cb4-b988-47584b282045", false, new DateTime(2024, 11, 3, 12, 10, 28, 834, DateTimeKind.Local).AddTicks(4768), "0ed877a5-982b-47bf-a6b5-983c1149ac75", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "0ed877a5-982b-47bf-a6b5-983c1149ac75", false, new DateTime(2024, 11, 3, 12, 10, 28, 834, DateTimeKind.Local).AddTicks(4840), "0ed877a5-982b-47bf-a6b5-983c1149ac75", null, null, "admin@miaoshu.xyz", false, "1It/wM0Hgslp001xCEYFsNBIPqfZmjxIZWBL0UvAMAI=", "", "952db095-ed24-4afe-989d-425a95e29380", "ec09492cbe984d2fa1aca3a5dec882ea", null, null, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysUrl");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "b253b817-2b3f-4cb4-b988-47584b282045");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "0ed877a5-982b-47bf-a6b5-983c1149ac75");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "952db095-ed24-4afe-989d-425a95e29380");

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "a82795b8-d07f-4f68-85ee-071ba050c3c4", false, new DateTime(2024, 11, 1, 22, 14, 33, 861, DateTimeKind.Local).AddTicks(8302), "7c089aeb-b383-4a58-8cae-e6987ac5a828", null, null, false, "Administrator", null, null },
                    { "af857136-ccb7-4567-b653-83af492ce849", false, new DateTime(2024, 11, 1, 22, 14, 33, 861, DateTimeKind.Local).AddTicks(8313), "7c089aeb-b383-4a58-8cae-e6987ac5a828", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "7c089aeb-b383-4a58-8cae-e6987ac5a828", false, new DateTime(2024, 11, 1, 22, 14, 33, 861, DateTimeKind.Local).AddTicks(8357), "7c089aeb-b383-4a58-8cae-e6987ac5a828", null, null, "admin@miaoshu.xyz", false, "L0AUoD+bHT/KlcnhtaCpKqJ6EJSwxvNmRiFJl0bsobY=", "", "a82795b8-d07f-4f68-85ee-071ba050c3c4", "3489dfabdc404fd78ad1abe6acc3e14f", null, null, "admin" });
        }
    }
}
