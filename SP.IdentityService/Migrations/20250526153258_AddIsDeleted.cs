using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SP.IdentityService.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7330631755436871680L);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 7330631755352985600L, 7330631755529146368L });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7330631755352985600L);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 7330631755529146368L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 7332790851502034944L, null, "Admin", "ADMIN" },
                    { 7332790851606892544L, null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "Email", "EmailConfirmed", "IsDeleted", "LockoutEnabled", "LockoutEnd", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { 7332790851703361536L, "494324190@qq.com", true, false, false, null, "AQAAAAIAAYagAAAAEDFn91g1UVDNMXOrlsBSbtr2gFr55PeBb+h/1z9HiFiz1MVqxt9lamTfCMBuKN7PJA==", null, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 7332790851502034944L, 7332790851703361536L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7332790851606892544L);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 7332790851502034944L, 7332790851703361536L });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7332790851502034944L);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 7332790851703361536L);

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 7330631755352985600L, null, "Admin", "ADMIN" },
                    { 7330631755436871680L, null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { 7330631755529146368L, "494324190@qq.com", true, false, null, "AQAAAAIAAYagAAAAEFTPgTimW6lHZm0UL0Z2o9xoXrZ7ipXhB/FREgdGrYQrBeNZTfJu8QYqPJ9DSxzByw==", null, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 7330631755352985600L, 7330631755529146368L });
        }
    }
}
