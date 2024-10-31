using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitSysRoleUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SysRole_SysUser_UserId",
                table: "SysRole");

            migrationBuilder.DropIndex(
                name: "IX_SysRole_UserId",
                table: "SysRole");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "1b78e284-2174-4a3f-8254-a8b22cd82b68");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "b7e6ae8c-3f62-4d11-93af-bbf7e306be39");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "08f35c1e-117f-431d-979d-9e51e29b0b7d");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SysRole");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "SysUser",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SysRoleUrl",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(100)", nullable: false),
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
                    table.PrimaryKey("PK_SysRoleUrl", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SysRoleSysRoleUrl",
                columns: table => new
                {
                    RoleUrlsId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    RolesId = table.Column<string>(type: "nvarchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysRoleSysRoleUrl", x => new { x.RoleUrlsId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_SysRoleSysRoleUrl_SysRoleUrl_RoleUrlsId",
                        column: x => x.RoleUrlsId,
                        principalTable: "SysRoleUrl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SysRoleSysRoleUrl_SysRole_RolesId",
                        column: x => x.RolesId,
                        principalTable: "SysRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "05847d63-9df7-4083-a588-0feef57b0648", new DateTime(2024, 10, 31, 23, 31, 16, 85, DateTimeKind.Local).AddTicks(7223), "26539465-d4e3-4224-9684-e3692d0772bd", null, null, false, "Administrator", null, null },
                    { "299e2358-c62c-43e4-8487-0eddc307c306", new DateTime(2024, 10, 31, 23, 31, 16, 85, DateTimeKind.Local).AddTicks(7233), "26539465-d4e3-4224-9684-e3692d0772bd", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "26539465-d4e3-4224-9684-e3692d0772bd", new DateTime(2024, 10, 31, 23, 31, 16, 85, DateTimeKind.Local).AddTicks(5734), "26539465-d4e3-4224-9684-e3692d0772bd", null, null, "admin@miaoshu.xyz", false, "6Xvz+X1e8ucTUwJsU7sRb1KJjd7WdPfnbqWiZTl9krw=", "", "05847d63-9df7-4083-a588-0feef57b0648", "554935227d2f4a3ea69d90195efce0a0", null, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_SysUser_RoleId",
                table: "SysUser",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRoleSysRoleUrl_RolesId",
                table: "SysRoleSysRoleUrl",
                column: "RolesId");

            migrationBuilder.AddForeignKey(
                name: "FK_SysUser_SysRole_RoleId",
                table: "SysUser",
                column: "RoleId",
                principalTable: "SysRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SysUser_SysRole_RoleId",
                table: "SysUser");

            migrationBuilder.DropTable(
                name: "SysRoleSysRoleUrl");

            migrationBuilder.DropTable(
                name: "SysRoleUrl");

            migrationBuilder.DropIndex(
                name: "IX_SysUser_RoleId",
                table: "SysUser");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "299e2358-c62c-43e4-8487-0eddc307c306");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "26539465-d4e3-4224-9684-e3692d0772bd");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "05847d63-9df7-4083-a588-0feef57b0648");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "SysUser");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "SysRole",
                type: "nvarchar(36)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId", "UserId" },
                values: new object[,]
                {
                    { "1b78e284-2174-4a3f-8254-a8b22cd82b68", new DateTime(2024, 9, 24, 23, 48, 7, 529, DateTimeKind.Local).AddTicks(6925), "08f35c1e-117f-431d-979d-9e51e29b0b7d", null, null, false, "Consumer", null, null, null },
                    { "b7e6ae8c-3f62-4d11-93af-bbf7e306be39", new DateTime(2024, 9, 24, 23, 48, 7, 529, DateTimeKind.Local).AddTicks(6918), "08f35c1e-117f-431d-979d-9e51e29b0b7d", null, null, false, "Administrator", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "08f35c1e-117f-431d-979d-9e51e29b0b7d", new DateTime(2024, 9, 24, 23, 48, 7, 529, DateTimeKind.Local).AddTicks(5886), "08f35c1e-117f-431d-979d-9e51e29b0b7d", null, null, "admin@miaoshu.xyz", false, "Xwizw3C+xEtFkmO4KIGskDc4Mc3K2hYKY/f+FEikf4A=", "", "13b5ff1a02534532930665f6b545eedb", null, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_SysRole_UserId",
                table: "SysRole",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SysRole_SysUser_UserId",
                table: "SysRole",
                column: "UserId",
                principalTable: "SysUser",
                principalColumn: "Id");
        }
    }
}
