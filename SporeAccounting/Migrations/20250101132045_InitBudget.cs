using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    IncomeExpenditureClassificationId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: false),
                    Remaining = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    SysUserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreateUserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    UpdateUserId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    DeleteDateTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    DeleteUserId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budget_SysUser_SysUserId",
                        column: x => x.SysUserId,
                        principalTable: "SysUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BudgetIncomeExpenditureClassification",
                columns: table => new
                {
                    BudgetsId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    IncomeExpenditureClassificationsId = table.Column<string>(type: "nvarchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetIncomeExpenditureClassification", x => new { x.BudgetsId, x.IncomeExpenditureClassificationsId });
                    table.ForeignKey(
                        name: "FK_BudgetIncomeExpenditureClassification_Budget_BudgetsId",
                        column: x => x.BudgetsId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BudgetIncomeExpenditureClassification_IncomeExpenditureClass~",
                        column: x => x.IncomeExpenditureClassificationsId,
                        principalTable: "IncomeExpenditureClassification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_Budget_SysUserId",
                table: "Budget",
                column: "SysUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetIncomeExpenditureClassification_IncomeExpenditureClass~",
                table: "BudgetIncomeExpenditureClassification",
                column: "IncomeExpenditureClassificationsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetIncomeExpenditureClassification");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "098e71cf-4630-467b-a530-cea4b30e9070",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9738));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "409f7f1d-3430-4f82-9180-520ac1dadbc9",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9707));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "422a920d-12e9-4263-a1b6-9d6e4e3366ea",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9713));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4b6a9c6f-d77f-4087-af5d-2d4f85375bda",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9720));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "551b2b37-dfd8-49df-bfc5-c78f068b2d01",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9732));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7b01d6fa-e673-4bfd-8112-3e988971d91c",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9744));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a374bbfa-99bd-4f14-9f11-49260528d7a4",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9751));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e25b4885-cf61-4249-b86f-0130defd1d57",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9700));

            migrationBuilder.UpdateData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9686));

            migrationBuilder.UpdateData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "10ce6d08-3de2-466e-a9bb-e15cb4eec56f",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(9657));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "10389aa0-b6f2-4241-9a77-ca8020656bb6",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(8566));

            migrationBuilder.UpdateData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "cef80881-fe89-4b1f-85ad-83184777d61b",
                column: "CreateDateTime",
                value: new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(8555));

            migrationBuilder.UpdateData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "b47637e2-603f-4df0-abe9-88d70fa870ee",
                columns: new[] { "CreateDateTime", "Password", "Salt" },
                values: new object[] { new DateTime(2025, 1, 1, 21, 15, 55, 708, DateTimeKind.Local).AddTicks(8639), "BoyB+fJ7JARVJ7rysfZ+Pga3pP+QWNzYFBGIGhBb4iI=", "d03cafe7ca874a688bb31df0c71a891c" });
        }
    }
}
