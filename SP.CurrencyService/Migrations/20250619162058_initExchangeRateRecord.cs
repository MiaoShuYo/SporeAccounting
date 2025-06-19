using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SP.CurrencyService.Migrations
{
    /// <inheritdoc />
    public partial class initExchangeRateRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExchangeRateRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false, comment: "Id"),
                    ExchangeRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false, comment: "汇率"),
                    ConvertCurrency = table.Column<string>(type: "nvarchar(20)", nullable: false, comment: "币种转换"),
                    Date = table.Column<DateTime>(type: "date", nullable: false, comment: "汇率日期"),
                    CreateDateTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "创建时间"),
                    CreateUserId = table.Column<string>(type: "nvarchar(36)", nullable: false, comment: "创建用户"),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "修改时间"),
                    UpdateUserId = table.Column<string>(type: "nvarchar(36)", nullable: true, comment: "修改用户"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRateRecords", x => x.Id);
                },
                comment: "汇率记录")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExchangeRateRecords");
        }
    }
}
