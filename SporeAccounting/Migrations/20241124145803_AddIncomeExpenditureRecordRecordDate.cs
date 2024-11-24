using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class AddIncomeExpenditureRecordRecordDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "35acf108-2aff-4196-a33e-117143aef6ea");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4a773a81-d408-4535-a9f5-f387f3fc9233");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "5ec27600-fe3b-4d56-883e-dad11fbe4873");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7255a6eb-ae5b-4e07-8edc-cb0d1e29618b");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "b2e81836-edc5-4762-b2b5-6a8777b5ccfc");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "b69bb731-8f62-4cf1-bdc3-f45c3f90be8d");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "b95a010f-55b2-4d99-b862-b796a2edfcc1");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "ba76a4b6-c7bc-48fb-9854-18e432163ee9");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "ee9af98c-3dba-4ead-8e8a-9f429dc96ade");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "f41c2386-e0ec-4baa-9539-4e2b0f7d624a");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "f895454a-3a2e-4bd2-a2b1-43dc9d972539");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "87bd9e76-9ad0-4744-9e46-3203a40bc86a");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "ebcef031-0158-4f8b-9b73-f448f7038aab");

            migrationBuilder.AddColumn<DateTime>(
                name: "RecordDate",
                table: "IncomeExpenditureRecord",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "079a780e-67ab-42a1-8a99-58aa812bfd77", "HKD", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6549), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "港元", null, null },
                    { "1a30cc31-f847-4247-a243-248a3ad9d13a", "EUR", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6517), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "欧元", null, null },
                    { "36b3cccc-cbd6-4e9d-81d6-a0e616ba4ed7", "TWD", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6561), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "新台币", null, null },
                    { "7e2ed102-4d0f-49fa-83d0-a51469fc39c8", "GBP", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6531), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "英镑", null, null },
                    { "88424068-b069-474c-b3a8-6b77657e0312", "JPY", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6524), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "日元", null, null },
                    { "a4d8cf89-3180-4291-8869-9205538d8960", "CNY", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6501), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "人民币", null, null },
                    { "b17ff54a-2cdd-40c6-9192-b552268497cc", "KRW", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6555), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "韩圆", null, null },
                    { "c8b1bc60-6efc-4cd6-8f5f-cf44c03e94ce", "USD", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6510), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "美元", null, null },
                    { "f8cc5da3-5daf-4e77-b55a-3468748d7a92", "MOP", new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6542), "b8cebe50-d7bd-4a98-b567-af5effb300f1", null, null, false, "澳门币", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "5bbe8aae-f4b2-4495-b985-cccd3fc6d7f6", false, new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(6471), "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "65d52218-417d-4b55-a41e-af18f4aa67d9", false, new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(5295), "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", null, null, false, "Consumer", null, null },
                    { "b8cebe50-d7bd-4a98-b567-af5effb300f1", false, new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(5282), "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", null, null, false, "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", false, new DateTime(2024, 11, 24, 22, 58, 2, 905, DateTimeKind.Local).AddTicks(5355), "7d50a013-0a3e-443c-9c80-8366fa2b8ac2", null, null, "admin@miaoshu.xyz", false, "eFACbEM7k2qm411QbWSRemzWozm3ZZ1mrpxVYC06tkU=", "", "b8cebe50-d7bd-4a98-b567-af5effb300f1", "2dd0533d86df4322b072f0be77838019", null, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Config_UserId",
                table: "Config",
                column: "UserId");
            
            migrationBuilder.AddForeignKey(
                name: "FK_Config_SysUser",
                table: "Config",
                column: "UserId",
                principalTable: "SysUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Config_UserId",
                table: "Config");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "079a780e-67ab-42a1-8a99-58aa812bfd77");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "1a30cc31-f847-4247-a243-248a3ad9d13a");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "36b3cccc-cbd6-4e9d-81d6-a0e616ba4ed7");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "7e2ed102-4d0f-49fa-83d0-a51469fc39c8");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "88424068-b069-474c-b3a8-6b77657e0312");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a4d8cf89-3180-4291-8869-9205538d8960");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "b17ff54a-2cdd-40c6-9192-b552268497cc");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "c8b1bc60-6efc-4cd6-8f5f-cf44c03e94ce");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "f8cc5da3-5daf-4e77-b55a-3468748d7a92");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "5bbe8aae-f4b2-4495-b985-cccd3fc6d7f6");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "65d52218-417d-4b55-a41e-af18f4aa67d9");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "7d50a013-0a3e-443c-9c80-8366fa2b8ac2");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "b8cebe50-d7bd-4a98-b567-af5effb300f1");

            migrationBuilder.DropColumn(
                name: "RecordDate",
                table: "IncomeExpenditureRecord");

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "35acf108-2aff-4196-a33e-117143aef6ea", "JPY", new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9921), "ebcef031-0158-4f8b-9b73-f448f7038aab", null, null, false, "日元", null, null },
                    { "4a773a81-d408-4535-a9f5-f387f3fc9233", "KRW", new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9952), "ebcef031-0158-4f8b-9b73-f448f7038aab", null, null, false, "韩圆", null, null },
                    { "5ec27600-fe3b-4d56-883e-dad11fbe4873", "MOP", new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9939), "ebcef031-0158-4f8b-9b73-f448f7038aab", null, null, false, "澳门币", null, null },
                    { "7255a6eb-ae5b-4e07-8edc-cb0d1e29618b", "CNY", new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9900), "ebcef031-0158-4f8b-9b73-f448f7038aab", null, null, false, "人民币", null, null },
                    { "b2e81836-edc5-4762-b2b5-6a8777b5ccfc", "USD", new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9908), "ebcef031-0158-4f8b-9b73-f448f7038aab", null, null, false, "美元", null, null },
                    { "b69bb731-8f62-4cf1-bdc3-f45c3f90be8d", "EUR", new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9915), "ebcef031-0158-4f8b-9b73-f448f7038aab", null, null, false, "欧元", null, null },
                    { "b95a010f-55b2-4d99-b862-b796a2edfcc1", "GBP", new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9928), "ebcef031-0158-4f8b-9b73-f448f7038aab", null, null, false, "英镑", null, null },
                    { "ba76a4b6-c7bc-48fb-9854-18e432163ee9", "HKD", new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9946), "ebcef031-0158-4f8b-9b73-f448f7038aab", null, null, false, "港元", null, null },
                    { "ee9af98c-3dba-4ead-8e8a-9f429dc96ade", "TWD", new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9959), "ebcef031-0158-4f8b-9b73-f448f7038aab", null, null, false, "新台币", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "f41c2386-e0ec-4baa-9539-4e2b0f7d624a", false, new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(9869), "87bd9e76-9ad0-4744-9e46-3203a40bc86a", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "ebcef031-0158-4f8b-9b73-f448f7038aab", false, new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(8708), "87bd9e76-9ad0-4744-9e46-3203a40bc86a", null, null, false, "Administrator", null, null },
                    { "f895454a-3a2e-4bd2-a2b1-43dc9d972539", false, new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(8720), "87bd9e76-9ad0-4744-9e46-3203a40bc86a", null, null, false, "Consumer", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "87bd9e76-9ad0-4744-9e46-3203a40bc86a", false, new DateTime(2024, 11, 23, 11, 30, 9, 512, DateTimeKind.Local).AddTicks(8778), "87bd9e76-9ad0-4744-9e46-3203a40bc86a", null, null, "admin@miaoshu.xyz", false, "eGl4J/dfpUJ8rhwvLejRFfACYsmtizXYB8XP2msXARU=", "", "ebcef031-0158-4f8b-9b73-f448f7038aab", "4951cb15942a4d48886c29bb496f30e5", null, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Config_UserId",
                table: "Config",
                column: "UserId",
                unique: true);
        }
    }
}
