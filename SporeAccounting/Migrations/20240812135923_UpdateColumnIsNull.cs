using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnIsNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdateUserId",
                table: "SysUser",
                type: "nvarchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDateTime",
                table: "SysUser",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "DeleteUserId",
                table: "SysUser",
                type: "nvarchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeleteDateTime",
                table: "SysUser",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "UpdateUserId",
                keyValue: null,
                column: "UpdateUserId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateUserId",
                table: "SysUser",
                type: "nvarchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdateDateTime",
                table: "SysUser",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "DeleteUserId",
                keyValue: null,
                column: "DeleteUserId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "DeleteUserId",
                table: "SysUser",
                type: "nvarchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeleteDateTime",
                table: "SysUser",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);
        }
    }
}
