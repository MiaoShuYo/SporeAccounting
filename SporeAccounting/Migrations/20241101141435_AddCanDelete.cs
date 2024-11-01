using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class AddCanDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "274a7bc2-0b66-4400-8451-f3950e5f2c65");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "6e8f3db2-1791-42be-bb98-53cecb7eff72");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "34d16e91-4180-4905-bb97-07dd49d6065e");

            migrationBuilder.AddColumn<bool>(
                name: "CanDelete",
                table: "SysUser",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanDelete",
                table: "SysRole",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CanDelete",
                table: "SysUser");

            migrationBuilder.DropColumn(
                name: "CanDelete",
                table: "SysRole");

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "274a7bc2-0b66-4400-8451-f3950e5f2c65", new DateTime(2024, 11, 1, 1, 48, 41, 598, DateTimeKind.Local).AddTicks(2282), "6e8f3db2-1791-42be-bb98-53cecb7eff72", null, null, false, "Consumer", null, null },
                    { "34d16e91-4180-4905-bb97-07dd49d6065e", new DateTime(2024, 11, 1, 1, 48, 41, 598, DateTimeKind.Local).AddTicks(2248), "6e8f3db2-1791-42be-bb98-53cecb7eff72", null, null, false, "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "6e8f3db2-1791-42be-bb98-53cecb7eff72", new DateTime(2024, 11, 1, 1, 48, 41, 598, DateTimeKind.Local).AddTicks(2324), "6e8f3db2-1791-42be-bb98-53cecb7eff72", null, null, "admin@miaoshu.xyz", false, "cRRkFQHlOQ5+TudBv6IUsrDLT6T7AjavZavK00/herA=", "", "34d16e91-4180-4905-bb97-07dd49d6065e", "c99b12497f4a4f2c87a7f82b9f3ec7f1", null, null, "admin" });
        }
    }
}
