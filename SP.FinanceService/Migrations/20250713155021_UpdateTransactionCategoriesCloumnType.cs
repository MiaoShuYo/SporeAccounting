using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SP.FinanceService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionCategoriesCloumnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<sbyte>(
                name: "Type",
                table: "TransactionCategory",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "TransactionCategory",
                type: "int",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "tinyint");
        }
    }
}
