using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class SetSysUrlDes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "098e71cf-4630-467b-a530-cea4b30e9070", "HKD", new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5366), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "港元", null, null },
                    { "409f7f1d-3430-4f82-9180-520ac1dadbc9", "EUR", new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5337), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "欧元", null, null },
                    { "422a920d-12e9-4263-a1b6-9d6e4e3366ea", "JPY", new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5343), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "日元", null, null },
                    { "4b6a9c6f-d77f-4087-af5d-2d4f85375bda", "GBP", new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5350), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "英镑", null, null },
                    { "551b2b37-dfd8-49df-bfc5-c78f068b2d01", "MOP", new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5360), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "澳门币", null, null },
                    { "7b01d6fa-e673-4bfd-8112-3e988971d91c", "KRW", new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5372), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "韩圆", null, null },
                    { "a374bbfa-99bd-4f14-9f11-49260528d7a4", "TWD", new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5378), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "新台币", null, null },
                    { "e25b4885-cf61-4249-b86f-0130defd1d57", "USD", new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5330), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "美元", null, null },
                    { "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6", "CNY", new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5320), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "人民币", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "10ce6d08-3de2-466e-a9bb-e15cb4eec56f", false, new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5285), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "10389aa0-b6f2-4241-9a77-ca8020656bb6",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(4136));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "cef80881-fe89-4b1f-85ad-83184777d61b",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(4126));

            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "b47637e2-603f-4df0-abe9-88d70fa870ee",
                columns: new[] { "CreateDateTime", "Password", "Salt" },
                values: new object[] { new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(4192), "8816PyRD6fr1+G0/hAyaOeg5SuPSG3mPddSlpSt/PdA=", "8ba3f33c5fe743f68ac92ce9381d8179" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "098e71cf-4630-467b-a530-cea4b30e9070");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "409f7f1d-3430-4f82-9180-520ac1dadbc9");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "422a920d-12e9-4263-a1b6-9d6e4e3366ea");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4b6a9c6f-d77f-4087-af5d-2d4f85375bda");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "551b2b37-dfd8-49df-bfc5-c78f068b2d01");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7b01d6fa-e673-4bfd-8112-3e988971d91c");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a374bbfa-99bd-4f14-9f11-49260528d7a4");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e25b4885-cf61-4249-b86f-0130defd1d57");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "10ce6d08-3de2-466e-a9bb-e15cb4eec56f");

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

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "10389aa0-b6f2-4241-9a77-ca8020656bb6",
                column: "CreateDateTime",
                value: new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(7638));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "cef80881-fe89-4b1f-85ad-83184777d61b",
                column: "CreateDateTime",
                value: new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(7617));

            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "b47637e2-603f-4df0-abe9-88d70fa870ee",
                columns: new[] { "CreateDateTime", "Password", "Salt" },
                values: new object[] { new DateTime(2024, 11, 26, 0, 27, 32, 260, DateTimeKind.Local).AddTicks(7709), "JibbEaukgua7Y8ylDFE9DkvRgSWgcLnI2M1xL9IvQe4=", "e3c85037b35f46a7b464e06243b5eecc" });
        }
    }
}
