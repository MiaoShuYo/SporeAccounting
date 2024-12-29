using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class SysUrlDesSetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SysUrl",
                type: "nvarchar(200)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)");

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "098e71cf-4630-467b-a530-cea4b30e9070",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3762));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "409f7f1d-3430-4f82-9180-520ac1dadbc9",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3732));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "422a920d-12e9-4263-a1b6-9d6e4e3366ea",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3739));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4b6a9c6f-d77f-4087-af5d-2d4f85375bda",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3746));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "551b2b37-dfd8-49df-bfc5-c78f068b2d01",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3756));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7b01d6fa-e673-4bfd-8112-3e988971d91c",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3769));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a374bbfa-99bd-4f14-9f11-49260528d7a4",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3775));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e25b4885-cf61-4249-b86f-0130defd1d57",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3673));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3666));

            migrationBuilder.UpdateData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "10ce6d08-3de2-466e-a9bb-e15cb4eec56f",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(3637));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "10389aa0-b6f2-4241-9a77-ca8020656bb6",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(2586));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "cef80881-fe89-4b1f-85ad-83184777d61b",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(2574));

            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "b47637e2-603f-4df0-abe9-88d70fa870ee",
                columns: new[] { "CreateDateTime", "Password", "Salt" },
                values: new object[] { new DateTime(2024, 12, 28, 23, 29, 14, 940, DateTimeKind.Local).AddTicks(2639), "LU3/N4MRQyPe9XqarjjWIf+E37f7LvuvporT/W450xk=", "b6490ae1ade4411dac41045bfab1f3e6" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SysUrl",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SysUrl",
                type: "nvarchar(200)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "098e71cf-4630-467b-a530-cea4b30e9070",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5366));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "409f7f1d-3430-4f82-9180-520ac1dadbc9",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5337));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "422a920d-12e9-4263-a1b6-9d6e4e3366ea",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5343));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4b6a9c6f-d77f-4087-af5d-2d4f85375bda",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5350));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "551b2b37-dfd8-49df-bfc5-c78f068b2d01",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5360));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7b01d6fa-e673-4bfd-8112-3e988971d91c",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5372));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a374bbfa-99bd-4f14-9f11-49260528d7a4",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5378));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e25b4885-cf61-4249-b86f-0130defd1d57",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5330));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5320));

            migrationBuilder.UpdateData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "10ce6d08-3de2-466e-a9bb-e15cb4eec56f",
                column: "CreateDateTime",
                value: new DateTime(2024, 12, 28, 23, 6, 21, 164, DateTimeKind.Local).AddTicks(5285));

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
    }
}
