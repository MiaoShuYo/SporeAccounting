using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitAccountBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "2359b739-2a2f-4c29-9350-ecbf10d0f830");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4675b909-a82b-4091-b23a-5907b7b8be3b");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7014806f-9f9b-49d7-8fc0-f7d4b1533308");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "725b313d-e279-4e56-9f59-a9ef8d2e6042");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "98739081-faad-42cb-bc42-ae2c043b035e");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "ac533097-a63f-451d-8fc0-b5938628f20d");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "acdf5bb6-a2ce-4157-94a7-454ffa216b14");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "b3d25cc9-cb0c-4706-b5b1-1e4eb9c8d0fd");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "f725fb69-ad59-484f-bf8d-5835ab949bbe");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "dae1c45b-d478-4fa3-afb7-bc88af674249");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "a59725fd-8381-401c-b606-3f3ce3464b53");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "cf9b392f-fa77-4e2e-9ed7-78eee7646d3d");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "34fe5804-516d-485d-822e-8ba2c56b5227");

            migrationBuilder.CreateTable(
                name: "AccountBook",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
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
                    table.PrimaryKey("PK_AccountBook", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "72e022b5-b40a-4a23-b6a6-2fb7b6ad66f6", "KRW", new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1907), "ee58d560-b44c-4032-b73d-04720a9c2c37", null, null, false, "韩圆", null, null },
                    { "8aa8a00a-df54-4397-9865-5e9791bf74f8", "GBP", new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1882), "ee58d560-b44c-4032-b73d-04720a9c2c37", null, null, false, "英镑", null, null },
                    { "8bcf8596-efcd-464c-8581-d36ce2444afb", "CNY", new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1840), "ee58d560-b44c-4032-b73d-04720a9c2c37", null, null, false, "人民币", null, null },
                    { "8e62a30c-db84-478b-83ba-0995ad2eb284", "EUR", new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1855), "ee58d560-b44c-4032-b73d-04720a9c2c37", null, null, false, "欧元", null, null },
                    { "962cee15-4ed1-4878-b877-08a727009146", "USD", new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1849), "ee58d560-b44c-4032-b73d-04720a9c2c37", null, null, false, "美元", null, null },
                    { "9631399c-0be8-48ed-8a33-02b69e09def3", "HKD", new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1901), "ee58d560-b44c-4032-b73d-04720a9c2c37", null, null, false, "港元", null, null },
                    { "ad45a67d-5c1f-440b-8565-73101f6fd06a", "TWD", new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1913), "ee58d560-b44c-4032-b73d-04720a9c2c37", null, null, false, "新台币", null, null },
                    { "d6ca0145-4e0b-453c-9b28-dac863e0b952", "MOP", new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1894), "ee58d560-b44c-4032-b73d-04720a9c2c37", null, null, false, "澳门币", null, null },
                    { "e646ada2-2aca-4ea6-aa1f-d88e3a4de525", "JPY", new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1875), "ee58d560-b44c-4032-b73d-04720a9c2c37", null, null, false, "日元", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "405a9d0f-0291-4e6a-bee4-ba511f1ed630", false, new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(1808), "2da32ea8-71aa-4060-a968-15fbe21d8974", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "dd62a92c-2354-4ec3-9618-22f13ad686d6", false, new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(564), "2da32ea8-71aa-4060-a968-15fbe21d8974", null, null, false, "Consumer", null, null },
                    { "ee58d560-b44c-4032-b73d-04720a9c2c37", false, new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(542), "2da32ea8-71aa-4060-a968-15fbe21d8974", null, null, false, "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "2da32ea8-71aa-4060-a968-15fbe21d8974", false, new DateTime(2024, 11, 23, 11, 27, 0, 171, DateTimeKind.Local).AddTicks(662), "2da32ea8-71aa-4060-a968-15fbe21d8974", null, null, "admin@miaoshu.xyz", false, "soq3PK8e/Cpdi9GBfbSkh0X/zomlqE6aMNLsgspjaBk=", "", "ee58d560-b44c-4032-b73d-04720a9c2c37", "74375f176b084f209f27943c421838fe", null, null, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBook");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "72e022b5-b40a-4a23-b6a6-2fb7b6ad66f6");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "8aa8a00a-df54-4397-9865-5e9791bf74f8");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "8bcf8596-efcd-464c-8581-d36ce2444afb");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "8e62a30c-db84-478b-83ba-0995ad2eb284");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "962cee15-4ed1-4878-b877-08a727009146");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "9631399c-0be8-48ed-8a33-02b69e09def3");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "ad45a67d-5c1f-440b-8565-73101f6fd06a");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "d6ca0145-4e0b-453c-9b28-dac863e0b952");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e646ada2-2aca-4ea6-aa1f-d88e3a4de525");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "405a9d0f-0291-4e6a-bee4-ba511f1ed630");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "dd62a92c-2354-4ec3-9618-22f13ad686d6");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "2da32ea8-71aa-4060-a968-15fbe21d8974");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "ee58d560-b44c-4032-b73d-04720a9c2c37");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "2359b739-2a2f-4c29-9350-ecbf10d0f830", "KRW", new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7883), "34fe5804-516d-485d-822e-8ba2c56b5227", null, null, false, "韩圆", null, null },
                    { "4675b909-a82b-4091-b23a-5907b7b8be3b", "MOP", new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7866), "34fe5804-516d-485d-822e-8ba2c56b5227", null, null, false, "澳门币", null, null },
                    { "7014806f-9f9b-49d7-8fc0-f7d4b1533308", "TWD", new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7890), "34fe5804-516d-485d-822e-8ba2c56b5227", null, null, false, "新台币", null, null },
                    { "725b313d-e279-4e56-9f59-a9ef8d2e6042", "JPY", new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7850), "34fe5804-516d-485d-822e-8ba2c56b5227", null, null, false, "日元", null, null },
                    { "98739081-faad-42cb-bc42-ae2c043b035e", "GBP", new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7856), "34fe5804-516d-485d-822e-8ba2c56b5227", null, null, false, "英镑", null, null },
                    { "ac533097-a63f-451d-8fc0-b5938628f20d", "EUR", new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7842), "34fe5804-516d-485d-822e-8ba2c56b5227", null, null, false, "欧元", null, null },
                    { "acdf5bb6-a2ce-4157-94a7-454ffa216b14", "HKD", new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7874), "34fe5804-516d-485d-822e-8ba2c56b5227", null, null, false, "港元", null, null },
                    { "b3d25cc9-cb0c-4706-b5b1-1e4eb9c8d0fd", "CNY", new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7810), "34fe5804-516d-485d-822e-8ba2c56b5227", null, null, false, "人民币", null, null },
                    { "f725fb69-ad59-484f-bf8d-5835ab949bbe", "USD", new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7835), "34fe5804-516d-485d-822e-8ba2c56b5227", null, null, false, "美元", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "dae1c45b-d478-4fa3-afb7-bc88af674249", false, new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(7759), "cf9b392f-fa77-4e2e-9ed7-78eee7646d3d", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "34fe5804-516d-485d-822e-8ba2c56b5227", false, new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(6872), "cf9b392f-fa77-4e2e-9ed7-78eee7646d3d", null, null, false, "Administrator", null, null },
                    { "a59725fd-8381-401c-b606-3f3ce3464b53", false, new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(6890), "cf9b392f-fa77-4e2e-9ed7-78eee7646d3d", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "cf9b392f-fa77-4e2e-9ed7-78eee7646d3d", false, new DateTime(2024, 11, 22, 16, 11, 42, 846, DateTimeKind.Local).AddTicks(6958), "cf9b392f-fa77-4e2e-9ed7-78eee7646d3d", null, null, "admin@miaoshu.xyz", false, "4Y/kt5bcQqHPaisIxHlLoc8OuhRrsKRikacIArvhZuo=", "", "34fe5804-516d-485d-822e-8ba2c56b5227", "4fff1bb8e85d4ea5a1fc73b318837ced", null, null, "admin" });
        }
    }
}
