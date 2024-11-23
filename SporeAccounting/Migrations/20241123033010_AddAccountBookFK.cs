using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountBookFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_AccountBook_UserId",
                table: "AccountBook",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountBook_SysUser_UserId",
                table: "AccountBook",
                column: "UserId",
                principalTable: "SysUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountBook_SysUser_UserId",
                table: "AccountBook");

            migrationBuilder.DropIndex(
                name: "IX_AccountBook_UserId",
                table: "AccountBook");

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
    }
}
