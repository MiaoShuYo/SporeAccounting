using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitBudget1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_SysUser_SysUserId",
                table: "Budget");

            migrationBuilder.AlterColumn<string>(
                name: "SysUserId",
                table: "Budget",
                type: "nvarchar(36)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)");

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "098e71cf-4630-467b-a530-cea4b30e9070",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3453));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "409f7f1d-3430-4f82-9180-520ac1dadbc9",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3424));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "422a920d-12e9-4263-a1b6-9d6e4e3366ea",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3430));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4b6a9c6f-d77f-4087-af5d-2d4f85375bda",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3437));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "551b2b37-dfd8-49df-bfc5-c78f068b2d01",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3447));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7b01d6fa-e673-4bfd-8112-3e988971d91c",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3459));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a374bbfa-99bd-4f14-9f11-49260528d7a4",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3524));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e25b4885-cf61-4249-b86f-0130defd1d57",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3417));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3410));

            migrationBuilder.UpdateData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "10ce6d08-3de2-466e-a9bb-e15cb4eec56f",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 783, DateTimeKind.Local).AddTicks(3384));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "10389aa0-b6f2-4241-9a77-ca8020656bb6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 782, DateTimeKind.Local).AddTicks(2523));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "cef80881-fe89-4b1f-85ad-83184777d61b",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 4, 0, 42, 2, 782, DateTimeKind.Local).AddTicks(2511));

            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "b47637e2-603f-4df0-abe9-88d70fa870ee",
                columns: new[] { "CreateDateTime", "Password", "Salt" },
                values: new object[] { new DateTime(2025, 1, 4, 0, 42, 2, 782, DateTimeKind.Local).AddTicks(2599), "YYRHFEblalx8fm/n9my90spMhXIPaQZFWz8l4ZHqQGc=", "2f2e84dcc1cb47599f587019a3115d68" });

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_SysUser_SysUserId",
                table: "Budget",
                column: "SysUserId",
                principalTable: "SysUser",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budget_SysUser_SysUserId",
                table: "Budget");

            migrationBuilder.UpdateData(
                table: "Budget",
                keyColumn: "SysUserId",
                keyValue: null,
                column: "SysUserId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "SysUserId",
                table: "Budget",
                type: "nvarchar(36)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "098e71cf-4630-467b-a530-cea4b30e9070",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2641));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "409f7f1d-3430-4f82-9180-520ac1dadbc9",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2609));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "422a920d-12e9-4263-a1b6-9d6e4e3366ea",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2618));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4b6a9c6f-d77f-4087-af5d-2d4f85375bda",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2625));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "551b2b37-dfd8-49df-bfc5-c78f068b2d01",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2634));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7b01d6fa-e673-4bfd-8112-3e988971d91c",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2647));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a374bbfa-99bd-4f14-9f11-49260528d7a4",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2653));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e25b4885-cf61-4249-b86f-0130defd1d57",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2603));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2594));

            migrationBuilder.UpdateData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "10ce6d08-3de2-466e-a9bb-e15cb4eec56f",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(2561));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "10389aa0-b6f2-4241-9a77-ca8020656bb6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(1221));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "cef80881-fe89-4b1f-85ad-83184777d61b",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(1209));

            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "b47637e2-603f-4df0-abe9-88d70fa870ee",
                columns: new[] { "CreateDateTime", "Password", "Salt" },
                values: new object[] { new DateTime(2025, 1, 1, 21, 20, 44, 589, DateTimeKind.Local).AddTicks(1283), "cV/vCfl5WRBT9xqeRpw0XQyqqoRk8oFComdtBGqj0u4=", "69784d7198b3446c871a339bfd1d4544" });

            migrationBuilder.AddForeignKey(
                name: "FK_Budget_SysUser_SysUserId",
                table: "Budget",
                column: "SysUserId",
                principalTable: "SysUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
