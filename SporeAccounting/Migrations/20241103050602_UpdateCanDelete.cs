using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCanDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "CanDelete",
                table: "SysUrl",
                type: "nvarchar(200)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "37191752-9c34-4106-82bc-5631cf69ef6f", false, new DateTime(2024, 11, 3, 13, 6, 2, 588, DateTimeKind.Local).AddTicks(3105), "d76591a7-e77f-4a85-af94-9539142eb2a0", null, null, false, "Administrator", null, null },
                    { "f581095b-ecbc-4cf9-9cdb-c1bdebe251f6", false, new DateTime(2024, 11, 3, 13, 6, 2, 588, DateTimeKind.Local).AddTicks(3125), "d76591a7-e77f-4a85-af94-9539142eb2a0", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "d76591a7-e77f-4a85-af94-9539142eb2a0", false, new DateTime(2024, 11, 3, 13, 6, 2, 588, DateTimeKind.Local).AddTicks(3716), "d76591a7-e77f-4a85-af94-9539142eb2a0", null, null, "admin@miaoshu.xyz", false, "Fd+aomGKuAet8eUb+FVNvxqdevWqK7UjnNrv8N6uO6A=", "", "37191752-9c34-4106-82bc-5631cf69ef6f", "0e92a36d056b4034beac108e7852570a", null, null, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "f581095b-ecbc-4cf9-9cdb-c1bdebe251f6");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "d76591a7-e77f-4a85-af94-9539142eb2a0");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "37191752-9c34-4106-82bc-5631cf69ef6f");

            migrationBuilder.DropColumn(
                name: "CanDelete",
                table: "SysUrl");

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
    }
}
