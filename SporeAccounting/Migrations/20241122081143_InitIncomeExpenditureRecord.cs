using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitIncomeExpenditureRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "223585cd-6aeb-45d7-905b-72b8436f5b20");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "71255818-d425-4434-a738-d2c8ba91936d");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "80089623-fb46-46af-b985-56bcae0107d3");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "8332caf8-3254-4ac6-94ca-c1b087971fd8");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "8685366a-d3dd-4bb9-94a3-8cf39d5c7323");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "9393effb-a8fd-4137-8e16-7d136c9e7efe");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a222ef91-3d14-4ab7-a420-070e45f98630");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "bb7b12c5-f27f-4a9c-a4da-23217fa73d18");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e722bcee-fcfc-42d3-9c86-fc6266332300");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "cfae7fd1-0923-4dab-8456-a15bf0e706b9");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "ca4bff06-8798-4b0e-a2c4-6930653c97f8");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "e43591f8-da98-4e9e-8d65-7d96539e9bee");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7");

            migrationBuilder.CreateTable(
                name: "IncomeExpenditureRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    BeforAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AfterAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncomeExpenditureClassificationId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    CurrencyId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(100)", nullable: true),
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
                    table.PrimaryKey("PK_IncomeExpenditureRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeExpenditureRecord_Currency_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currency",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomeExpenditureRecord_IncomeExpenditureClassification_Inco~",
                        column: x => x.IncomeExpenditureClassificationId,
                        principalTable: "IncomeExpenditureClassification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomeExpenditureRecord_SysUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_IncomeExpenditureRecord_CurrencyId",
                table: "IncomeExpenditureRecord",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeExpenditureRecord_IncomeExpenditureClassificationId",
                table: "IncomeExpenditureRecord",
                column: "IncomeExpenditureClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeExpenditureRecord_UserId",
                table: "IncomeExpenditureRecord",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomeExpenditureRecord");

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

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "223585cd-6aeb-45d7-905b-72b8436f5b20", "GBP", new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8229), "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", null, null, false, "英镑", null, null },
                    { "71255818-d425-4434-a738-d2c8ba91936d", "EUR", new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8199), "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", null, null, false, "欧元", null, null },
                    { "80089623-fb46-46af-b985-56bcae0107d3", "HKD", new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8245), "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", null, null, false, "港元", null, null },
                    { "8332caf8-3254-4ac6-94ca-c1b087971fd8", "USD", new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8192), "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", null, null, false, "美元", null, null },
                    { "8685366a-d3dd-4bb9-94a3-8cf39d5c7323", "TWD", new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8259), "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", null, null, false, "新台币", null, null },
                    { "9393effb-a8fd-4137-8e16-7d136c9e7efe", "MOP", new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8239), "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", null, null, false, "澳门币", null, null },
                    { "a222ef91-3d14-4ab7-a420-070e45f98630", "JPY", new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8223), "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", null, null, false, "日元", null, null },
                    { "bb7b12c5-f27f-4a9c-a4da-23217fa73d18", "CNY", new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8184), "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", null, null, false, "人民币", null, null },
                    { "e722bcee-fcfc-42d3-9c86-fc6266332300", "KRW", new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8252), "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", null, null, false, "韩圆", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "cfae7fd1-0923-4dab-8456-a15bf0e706b9", false, new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(8157), "e43591f8-da98-4e9e-8d65-7d96539e9bee", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", false, new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(7200), "e43591f8-da98-4e9e-8d65-7d96539e9bee", null, null, false, "Administrator", null, null },
                    { "ca4bff06-8798-4b0e-a2c4-6930653c97f8", false, new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(7211), "e43591f8-da98-4e9e-8d65-7d96539e9bee", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "e43591f8-da98-4e9e-8d65-7d96539e9bee", false, new DateTime(2024, 11, 22, 15, 50, 27, 530, DateTimeKind.Local).AddTicks(7257), "e43591f8-da98-4e9e-8d65-7d96539e9bee", null, null, "admin@miaoshu.xyz", false, "dkpcWp/NbEnA5JJVGbGxnchw6MelCuQk+YcRl8UvpFM=", "", "26fa9d1f-4aeb-4e9b-8ba3-dccabc055bf7", "7731f7b158194dce98d617b0ad38d784", null, null, "admin" });
        }
    }
}
