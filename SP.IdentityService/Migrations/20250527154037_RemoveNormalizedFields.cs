using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SP.IdentityService.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNormalizedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7333152990070788096L);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 7333152989991096320L, 7333152990150479872L });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7333152989991096320L);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 7333152990150479872L);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 7333155173923246080L, null, "Admin", "ADMIN" },
                    { 7333155174015520768L, null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "Email", "EmailConfirmed", "IsDeleted", "LockoutEnabled", "LockoutEnd", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { 7333155174099406848L, "494324190@qq.com", true, false, false, null, "AQAAAAIAAYagAAAAELkxVNU3bTyoz+vBllYqLJT/PXhI4RL1PFa7o2848F32K0MrmB3sYzo83rghpERNvA==", null, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 7333155173923246080L, 7333155174099406848L });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7333155174015520768L);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 7333155173923246080L, 7333155174099406848L });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7333155173923246080L);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 7333155174099406848L);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 7333152989991096320L, null, "Admin", "ADMIN" },
                    { 7333152990070788096L, null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "Email", "EmailConfirmed", "IsDeleted", "LockoutEnabled", "LockoutEnd", "PasswordHash", "PhoneNumber", "UserName" },
                values: new object[] { 7333152990150479872L, "494324190@qq.com", true, false, false, null, "AQAAAAIAAYagAAAAEH/pr/p6ORI9dPZOQNYaRBUQJAckfeZGTDKsFaL4TyHN5u8iFRjc0djVhYtAsBJUEA==", null, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 7333152989991096320L, 7333152990150479872L });
        }
    }
}
