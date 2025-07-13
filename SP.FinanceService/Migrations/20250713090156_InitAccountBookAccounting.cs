using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SP.FinanceService.Migrations
{
    /// <inheritdoc />
    public partial class InitAccountBookAccounting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionCategoryId",
                table: "TransactionCategory");

            migrationBuilder.AddColumn<long>(
                name: "ParentId",
                table: "TransactionCategory",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AccountBook",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "创建时间"),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false, comment: "创建用户"),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "修改时间"),
                    UpdateUserId = table.Column<long>(type: "bigint", nullable: true, comment: "修改用户"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBook", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Accountings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BeforAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AfterAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionCategoryId = table.Column<long>(type: "bigint", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountBookId = table.Column<long>(type: "bigint", nullable: false),
                    CurrencyId = table.Column<long>(type: "bigint", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "创建时间"),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false, comment: "创建用户"),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "修改时间"),
                    UpdateUserId = table.Column<long>(type: "bigint", nullable: true, comment: "修改用户"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accountings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBook");

            migrationBuilder.DropTable(
                name: "Accountings");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "TransactionCategory");

            migrationBuilder.AddColumn<string>(
                name: "TransactionCategoryId",
                table: "TransactionCategory",
                type: "nvarchar(36)",
                nullable: true);
        }
    }
}
