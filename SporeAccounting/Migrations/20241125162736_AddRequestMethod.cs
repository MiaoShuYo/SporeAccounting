using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "RequestMethod",
                table: "SysUrl",
                type: "nvarchar(10)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "17c2487c-4576-4102-9f02-5e6cbd12ac52", "EUR", new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9032), "cef80881-fe89-4b1f-85ad-83184777d61b", null, null, false, "欧元", null, null },
                    { "36ef32b5-e6f2-404e-b421-ccde00a7080f", "TWD", new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9148), "cef80881-fe89-4b1f-85ad-83184777d61b", null, null, false, "新台币", null, null },
                    { "372f33e9-2db1-4f03-83e2-1ea5fbcdfea5", "MOP", new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9064), "cef80881-fe89-4b1f-85ad-83184777d61b", null, null, false, "澳门币", null, null },
                    { "6f89370d-c248-417e-b6df-5633990a9492", "CNY", new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9012), "cef80881-fe89-4b1f-85ad-83184777d61b", null, null, false, "人民币", null, null },
                    { "775320b6-2ecd-49bc-8f98-78afce82cdf7", "GBP", new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9050), "cef80881-fe89-4b1f-85ad-83184777d61b", null, null, false, "英镑", null, null },
                    { "79885c41-4a36-4b92-9621-37fefec22605", "JPY", new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9040), "cef80881-fe89-4b1f-85ad-83184777d61b", null, null, false, "日元", null, null },
                    { "c3124663-b1b4-4cb9-9846-6e1e97a4ac49", "USD", new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9023), "cef80881-fe89-4b1f-85ad-83184777d61b", null, null, false, "美元", null, null },
                    { "cb840646-b0d5-4cb2-82ea-25062d29e68b", "KRW", new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9139), "cef80881-fe89-4b1f-85ad-83184777d61b", null, null, false, "韩圆", null, null },
                    { "ea561a2f-5afd-4062-b1d9-1d7fa953bbcb", "HKD", new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(9130), "cef80881-fe89-4b1f-85ad-83184777d61b", null, null, false, "港元", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "c9f9b44c-4edb-4c9f-9610-0183a8abb912", false, new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(8976), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "10389aa0-b6f2-4241-9a77-ca8020656bb6", false, new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(7638), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "Consumer", null, null },
                    { "cef80881-fe89-4b1f-85ad-83184777d61b", false, new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(7617), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "b47637e2-603f-4df0-abe9-88d70fa870ee", false, new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(7709), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, "admin@miaoshu.xyz", false, "JibbEaukgua7Y8ylDFE9DkvRgSWgcLnI2M1xL9IvQe4=", "", "cef80881-fe89-4b1f-85ad-83184777d61b", "e3c85037b35f46a7b464e06243b5eecc", null, null, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "17c2487c-4576-4102-9f02-5e6cbd12ac52");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "36ef32b5-e6f2-404e-b421-ccde00a7080f");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "372f33e9-2db1-4f03-83e2-1ea5fbcdfea5");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "6f89370d-c248-417e-b6df-5633990a9492");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "775320b6-2ecd-49bc-8f98-78afce82cdf7");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "79885c41-4a36-4b92-9621-37fefec22605");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "c3124663-b1b4-4cb9-9846-6e1e97a4ac49");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "cb840646-b0d5-4cb2-82ea-25062d29e68b");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "ea561a2f-5afd-4062-b1d9-1d7fa953bbcb");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "c9f9b44c-4edb-4c9f-9610-0183a8abb912");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "10389aa0-b6f2-4241-9a77-ca8020656bb6");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "b47637e2-603f-4df0-abe9-88d70fa870ee");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "cef80881-fe89-4b1f-85ad-83184777d61b");

            migrationBuilder.DropColumn(
                name: "RequestMethod",
                table: "SysUrl");

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
        }
    }
}
