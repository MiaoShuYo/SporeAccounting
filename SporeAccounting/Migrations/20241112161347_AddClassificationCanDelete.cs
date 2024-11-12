using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class AddClassificationCanDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "64f33311-1e28-4f4a-8d3d-faa113864652");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "c1cc21f5-53df-4b84-9f30-1c3dbef072f8");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "0d6511fe-cc70-4514-abc7-6ef02e4b1552");

            migrationBuilder.AddColumn<bool>(
                name: "CanDelete",
                table: "IncomeExpenditureClassification",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CanDelete",
                table: "IncomeExpenditureClassification");

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "0d6511fe-cc70-4514-abc7-6ef02e4b1552", false, new DateTime(2024, 11, 13, 0, 2, 28, 724, DateTimeKind.Local).AddTicks(8046), "c1cc21f5-53df-4b84-9f30-1c3dbef072f8", null, null, false, "Administrator", null, null },
                    { "64f33311-1e28-4f4a-8d3d-faa113864652", false, new DateTime(2024, 11, 13, 0, 2, 28, 724, DateTimeKind.Local).AddTicks(8060), "c1cc21f5-53df-4b84-9f30-1c3dbef072f8", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "c1cc21f5-53df-4b84-9f30-1c3dbef072f8", false, new DateTime(2024, 11, 13, 0, 2, 28, 724, DateTimeKind.Local).AddTicks(8106), "c1cc21f5-53df-4b84-9f30-1c3dbef072f8", null, null, "admin@miaoshu.xyz", false, "4eGevupeD4LV439DS6/shNOc7qYBLA5W+niLbe35gFU=", "", "0d6511fe-cc70-4514-abc7-6ef02e4b1552", "847a360593bb408589aeecccb9f650ef", null, null, "admin" });
        }
    }
}
