using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAccountBookMoney : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "079a780e-67ab-42a1-8a99-58aa812bfd77");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "1a30cc31-f847-4247-a243-248a3ad9d13a");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "36b3cccc-cbd6-4e9d-81d6-a0e616ba4ed7");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7e2ed102-4d0f-49fa-83d0-a51469fc39c8");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "88424068-b069-474c-b3a8-6b77657e0312");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a4d8cf89-3180-4291-8869-9205538d8960");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "b17ff54a-2cdd-40c6-9192-b552268497cc");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "c8b1bc60-6efc-4cd6-8f5f-cf44c03e94ce");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "f8cc5da3-5daf-4e77-b55a-3468748d7a92");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "5bbe8aae-f4b2-4495-b985-cccd3fc6d7f6");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "65d52218-417d-4b55-a41e-af18f4aa67d9");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "7d50a013-0a3e-443c-9c80-8366fa2b8ac2");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "b8cebe50-d7bd-4a98-b567-af5effb300f1");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "AccountBook");

            migrationBuilder.AddColumn<string>(
                name: "AccountBookId",
                table: "IncomeExpenditureRecord",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "0693dd76-dbfb-472c-8f29-d8c2531627e5", "USD", new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7754), "49fe63be-c0e6-4377-a13a-94393ec71e75", null, null, false, "美元", null, null },
                    { "4f2e4597-c50d-4ded-b2a0-7f3a0e08ea6c", "MOP", new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7786), "49fe63be-c0e6-4377-a13a-94393ec71e75", null, null, false, "澳门币", null, null },
                    { "7e1589ba-d12d-4a09-93e3-f7a56fab4adf", "JPY", new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7767), "49fe63be-c0e6-4377-a13a-94393ec71e75", null, null, false, "日元", null, null },
                    { "c50fde42-b821-4a76-b123-df091c9bea1f", "GBP", new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7774), "49fe63be-c0e6-4377-a13a-94393ec71e75", null, null, false, "英镑", null, null },
                    { "d0a53c6b-dc4a-4fd2-b8f8-f8845a472563", "HKD", new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7793), "49fe63be-c0e6-4377-a13a-94393ec71e75", null, null, false, "港元", null, null },
                    { "e24c196e-c8a9-4352-a55e-586118dd7eba", "EUR", new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7760), "49fe63be-c0e6-4377-a13a-94393ec71e75", null, null, false, "欧元", null, null },
                    { "e447f0f8-634e-44e8-b3a2-c9f3d109a112", "TWD", new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7806), "49fe63be-c0e6-4377-a13a-94393ec71e75", null, null, false, "新台币", null, null },
                    { "ec65efe0-6f43-4347-b0cf-e6d707aad912", "KRW", new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7799), "49fe63be-c0e6-4377-a13a-94393ec71e75", null, null, false, "韩圆", null, null },
                    { "ecafa7ca-6c81-4a87-8758-ad1176db2f7e", "CNY", new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7745), "49fe63be-c0e6-4377-a13a-94393ec71e75", null, null, false, "人民币", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "d241c205-18c6-4b07-86d2-05035dea95b3", false, new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(7717), "a3d86aeb-3eec-4c58-a98a-7b040011f416", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "49fe63be-c0e6-4377-a13a-94393ec71e75", false, new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(6380), "a3d86aeb-3eec-4c58-a98a-7b040011f416", null, null, false, "Administrator", null, null },
                    { "5bb40666-8842-4683-b0f7-9a4dd022689b", false, new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(6400), "a3d86aeb-3eec-4c58-a98a-7b040011f416", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "a3d86aeb-3eec-4c58-a98a-7b040011f416", false, new DateTime(2024, 11, 25, 0, 25, 38, 289, DateTimeKind.Local).AddTicks(6460), "a3d86aeb-3eec-4c58-a98a-7b040011f416", null, null, "admin@miaoshu.xyz", false, "JLv8ei76H4GSj2Z1+m5yrwVj4cEjIv464wbjCu/CEKc=", "", "49fe63be-c0e6-4377-a13a-94393ec71e75", "63323887912b41bab5f95d9a1fb8e338", null, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_IncomeExpenditureRecord_AccountBookId",
                table: "IncomeExpenditureRecord",
                column: "AccountBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeExpenditureRecord_AccountBook_AccountBookId",
                table: "IncomeExpenditureRecord",
                column: "AccountBookId",
                principalTable: "AccountBook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomeExpenditureRecord_AccountBook_AccountBookId",
                table: "IncomeExpenditureRecord");

            migrationBuilder.DropIndex(
                name: "IX_IncomeExpenditureRecord_AccountBookId",
                table: "IncomeExpenditureRecord");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "0693dd76-dbfb-472c-8f29-d8c2531627e5");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4f2e4597-c50d-4ded-b2a0-7f3a0e08ea6c");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7e1589ba-d12d-4a09-93e3-f7a56fab4adf");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "c50fde42-b821-4a76-b123-df091c9bea1f");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "d0a53c6b-dc4a-4fd2-b8f8-f8845a472563");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e24c196e-c8a9-4352-a55e-586118dd7eba");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e447f0f8-634e-44e8-b3a2-c9f3d109a112");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "ec65efe0-6f43-4347-b0cf-e6d707aad912");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "ecafa7ca-6c81-4a87-8758-ad1176db2f7e");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "d241c205-18c6-4b07-86d2-05035dea95b3");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "5bb40666-8842-4683-b0f7-9a4dd022689b");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "a3d86aeb-3eec-4c58-a98a-7b040011f416");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "49fe63be-c0e6-4377-a13a-94393ec71e75");

            migrationBuilder.DropColumn(
                name: "AccountBookId",
                table: "IncomeExpenditureRecord");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "AccountBook",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "079a780e-67ab-42a1-8a99-58aa812bfd77", "HKD", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6549), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "港元", null, null },
                    { "1a30cc31-f847-4247-a243-248a3ad9d13a", "EUR", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6517), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "欧元", null, null },
                    { "36b3cccc-cbd6-4e9d-81d6-a0e616ba4ed7", "TWD", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6561), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "新台币", null, null },
                    { "7e2ed102-4d0f-49fa-83d0-a51469fc39c8", "GBP", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6531), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "英镑", null, null },
                    { "88424068-b069-474c-b3a8-6b77657e0312", "JPY", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6524), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "日元", null, null },
                    { "a4d8cf89-3180-4291-8869-9205538d8960", "CNY", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6501), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "人民币", null, null },
                    { "b17ff54a-2cdd-40c6-9192-b552268497cc", "KRW", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6555), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "韩圆", null, null },
                    { "c8b1bc60-6efc-4cd6-8f5f-cf44c03e94ce", "USD", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6510), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "美元", null, null },
                    { "f8cc5da3-5daf-4e77-b55a-3468748d7a92", "MOP", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6542), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "澳门币", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "5bbe8aae-f4b2-4495-b985-cccd3fc6d7f6", false, new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6471), "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "65d52218-417d-4b55-a41e-af18f4aa67d9", false, new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(5295), "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", null, null, false, "Consumer", null, null },
                    { "b8cebe50-d7bd-4a98-b567-af5effb300f1", false, new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(5282), "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", null, null, false, "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", false, new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(5355), "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", null, null, "admin@miaoshu.xyz", false, "eFACbEM7k2qm411QbWSRemzWozm3ZZ1mrpxVYC06tkU=", "", "b8cebe50-d7bd-4a98-b567-af5effb300f1", "2dd0533d86df4322b072f0be77838019", null, null, "admin" });
        }
    }
}
