using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIncomeExpenditureClassification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "ParentId",
                table: "IncomeExpenditureClassification",
                type: "nvarchar(36)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_IncomeExpenditureClassification_ParentId",
                table: "IncomeExpenditureClassification",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeExpenditureClassification_IncomeExpenditureClassificat~",
                table: "IncomeExpenditureClassification",
                column: "ParentId",
                principalTable: "IncomeExpenditureClassification",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomeExpenditureClassification_IncomeExpenditureClassificat~",
                table: "IncomeExpenditureClassification");

            migrationBuilder.DropIndex(
                name: "IX_IncomeExpenditureClassification_ParentId",
                table: "IncomeExpenditureClassification");

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

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "IncomeExpenditureClassification");

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
    }
}
