using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SP.CurrencyService.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    CreateDateTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "创建时间"),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false, comment: "创建用户"),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "修改时间"),
                    UpdateUserId = table.Column<long>(type: "bigint", nullable: true, comment: "修改用户"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExchangeRateRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExchangeRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false, comment: "汇率"),
                    SourceCurrencyId = table.Column<long>(type: "long", nullable: false, comment: "源币种"),
                    TargetCurrencyId = table.Column<long>(type: "long", nullable: false, comment: "目标币种"),
                    ConvertCurrency = table.Column<string>(type: "nvarchar(20)", nullable: false, comment: "币种转换"),
                    Date = table.Column<DateTime>(type: "date", nullable: false, comment: "汇率日期"),
                    CreateDateTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "创建时间"),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false, comment: "创建用户"),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "修改时间"),
                    UpdateUserId = table.Column<long>(type: "bigint", nullable: true, comment: "修改用户"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRateRecords", x => x.Id);
                },
                comment: "汇率记录")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { 7342403857206693888L, "CNY", new DateTime(2025, 6, 22, 12, 11, 34, 843, DateTimeKind.Local).AddTicks(845), 7333155174099406848L, false, "人民币", null, null },
                    { 7342403857353494528L, "USD", new DateTime(2025, 6, 22, 12, 11, 34, 878, DateTimeKind.Local).AddTicks(3708), 7333155174099406848L, false, "美元", null, null },
                    { 7342403857508683776L, "EUR", new DateTime(2025, 6, 22, 12, 11, 34, 916, DateTimeKind.Local).AddTicks(3860), 7333155174099406848L, false, "欧元", null, null },
                    { 7342403857672261632L, "JPY", new DateTime(2025, 6, 22, 12, 11, 34, 955, DateTimeKind.Local).AddTicks(7747), 7333155174099406848L, false, "日元", null, null },
                    { 7342403857840033792L, "GBP", new DateTime(2025, 6, 22, 12, 11, 34, 995, DateTimeKind.Local).AddTicks(2994), 7333155174099406848L, false, "英镑", null, null },
                    { 7342403857995223040L, "MOP", new DateTime(2025, 6, 22, 12, 11, 35, 34, DateTimeKind.Local).AddTicks(8064), 7333155174099406848L, false, "澳门币", null, null },
                    { 7342403858167189504L, "HKD", new DateTime(2025, 6, 22, 12, 11, 35, 73, DateTimeKind.Local).AddTicks(610), 7333155174099406848L, false, "港元", null, null },
                    { 7342403858334961664L, "KRW", new DateTime(2025, 6, 22, 12, 11, 35, 116, DateTimeKind.Local).AddTicks(3899), 7333155174099406848L, false, "韩圆", null, null },
                    { 7342403858506928128L, "TWD", new DateTime(2025, 6, 22, 12, 11, 35, 153, DateTimeKind.Local).AddTicks(620), 7333155174099406848L, false, "新台币", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "ExchangeRateRecords");
        }
    }
}
