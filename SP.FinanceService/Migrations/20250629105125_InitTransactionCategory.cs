using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SP.FinanceService.Migrations
{
    /// <inheritdoc />
    public partial class InitTransactionCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransactionCategory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false, comment: "Id")
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CanDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TransactionCategoryId = table.Column<string>(type: "nvarchar(36)", nullable: true),
                    CreateDateTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "创建时间"),
                    CreateUserId = table.Column<long>(type: "bigint", nullable: false, comment: "创建用户"),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "修改时间"),
                    UpdateUserId = table.Column<long>(type: "bigint", nullable: true, comment: "修改用户"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCategory", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransactionCategory");
        }
    }
}
