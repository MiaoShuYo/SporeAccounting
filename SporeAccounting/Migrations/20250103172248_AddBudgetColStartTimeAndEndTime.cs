using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetColStartTimeAndEndTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "098e71cf-4630-467b-a530-cea4b30e9070",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7186));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "409f7f1d-3430-4f82-9180-520ac1dadbc9",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7155));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "422a920d-12e9-4263-a1b6-9d6e4e3366ea",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7163));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4b6a9c6f-d77f-4087-af5d-2d4f85375bda",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7170));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "551b2b37-dfd8-49df-bfc5-c78f068b2d01",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7180));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7b01d6fa-e673-4bfd-8112-3e988971d91c",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7193));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a374bbfa-99bd-4f14-9f11-49260528d7a4",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7199));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e25b4885-cf61-4249-b86f-0130defd1d57",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7149));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7141));

            migrationBuilder.UpdateData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "10ce6d08-3de2-466e-a9bb-e15cb4eec56f",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(7117));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "10389aa0-b6f2-4241-9a77-ca8020656bb6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(5944));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "cef80881-fe89-4b1f-85ad-83184777d61b",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(5933));

            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "b47637e2-603f-4df0-abe9-88d70fa870ee",
                columns: new[] { "CreateDateTime", "Password", "Salt" },
                values: new object[] { new DateTime(2025, 1, 4, 1, 22, 47, 854, DateTimeKind.Local).AddTicks(6022), "u3TXsFkIzpZIbZrKNwuBuyo2ivTGbYHJbRB5zYnVeC8=", "3fc4f01b712045cfb9ef5fcf7e944a99" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "098e71cf-4630-467b-a530-cea4b30e9070",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6839));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "409f7f1d-3430-4f82-9180-520ac1dadbc9",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6809));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "422a920d-12e9-4263-a1b6-9d6e4e3366ea",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6815));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4b6a9c6f-d77f-4087-af5d-2d4f85375bda",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6822));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "551b2b37-dfd8-49df-bfc5-c78f068b2d01",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6832));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7b01d6fa-e673-4bfd-8112-3e988971d91c",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6845));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a374bbfa-99bd-4f14-9f11-49260528d7a4",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6851));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e25b4885-cf61-4249-b86f-0130defd1d57",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6802));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6794));

            migrationBuilder.UpdateData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "10ce6d08-3de2-466e-a9bb-e15cb4eec56f",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(6764));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "10389aa0-b6f2-4241-9a77-ca8020656bb6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(5467));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "cef80881-fe89-4b1f-85ad-83184777d61b",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(5455));

            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "b47637e2-603f-4df0-abe9-88d70fa870ee",
                columns: new[] { "CreateDateTime", "Password", "Salt" },
                values: new object[] { new DateTime(2025, 1, 4, 1, 17, 26, 235, DateTimeKind.Local).AddTicks(5588), "mvrwgJUkT6jDQ1ZCpAMMGb1riA99TG6JFma7+zGhUPQ=", "ea790a3517684aa689544659ee4dc8cf" });
        }
    }
}
