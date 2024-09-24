using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitSysRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "e2ba96c0-e294-4fd5-b05d-f8710ed098a9");

            migrationBuilder.CreateTable(
                name: "SysRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: true),
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
                    table.PrimaryKey("PK_SysRole", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SysRole_SysUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysUser",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysRole");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "08f35c1e-117f-431d-979d-9e51e29b0b7d");

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "e2ba96c0-e294-4fd5-b05d-f8710ed098a9", new DateTime(2024, 9, 24, 23, 45, 49, 13, DateTimeKind.Local).AddTicks(8841), "e2ba96c0-e294-4fd5-b05d-f8710ed098a9", null, null, "admin@miaoshu.xyz", false, "MOlo57MRoO6ARMhWLy+HG+YP7zB+Xdi+9s6c/6GB+7Q=", "", "a36f25484ef64f4db492ce717eebe229", null, null, "admin" });
        }
    }
}
