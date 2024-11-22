using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConfigAddFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "0382fc54-9a3b-49be-8ac4-7a1307cd2481");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "2abdc350-2b4e-462e-a8fd-3490dc391db2");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "3e6726b7-5583-4011-95f0-5c8851eb13eb");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "4225e640-f0f4-4151-bc9d-ee156b2f1fb4");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "61bc551c-ecb5-4120-8b7d-fa35bc1307b0");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "a1c46451-e57f-418b-9e8c-b22587832dea");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "b23014ed-08bd-4db3-aa4a-5432db0176fa");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e288fab2-3239-4be0-9682-dcb53e8c8cd2");

            migrationBuilder.DeleteData(
                table: "Currency",
                keyColumn: "Id",
                keyValue: "e37cd628-00e0-4036-b952-6f44038a549a");

            migrationBuilder.DeleteData(
                table: "IncomeExpenditureClassification",
                keyColumn: "Id",
                keyValue: "e3e389f7-8e56-4492-acf9-28220459b044");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "1c9bb153-ba4c-458b-8f24-67de6d09ba6e");

            migrationBuilder.DeleteData(
                table: "SysUser",
                keyColumn: "Id",
                keyValue: "c8873ec9-cf40-49d1-b8e5-4507e9d25b53");

            migrationBuilder.DeleteData(
                table: "SysRole",
                keyColumn: "Id",
                keyValue: "484488fe-9930-4d4f-bf1a-59a4fedc7529");

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

            migrationBuilder.CreateIndex(
                name: "IX_Config_UserId",
                table: "Config",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Config_SysUser_UserId",
                table: "Config",
                column: "UserId",
                principalTable: "SysUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Config_SysUser_UserId",
                table: "Config");

            migrationBuilder.DropIndex(
                name: "IX_Config_UserId",
                table: "Config");

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

            migrationBuilder.InsertData(
                table: "Currency",
                columns: new[] { "Id", "Abbreviation", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "0382fc54-9a3b-49be-8ac4-7a1307cd2481", "CNY", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(886), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "人民币", null, null },
                    { "2abdc350-2b4e-462e-a8fd-3490dc391db2", "GBP", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(912), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "英镑", null, null },
                    { "3e6726b7-5583-4011-95f0-5c8851eb13eb", "USD", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(893), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "美元", null, null },
                    { "4225e640-f0f4-4151-bc9d-ee156b2f1fb4", "KRW", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(935), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "韩圆", null, null },
                    { "61bc551c-ecb5-4120-8b7d-fa35bc1307b0", "EUR", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(900), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "欧元", null, null },
                    { "a1c46451-e57f-418b-9e8c-b22587832dea", "HKD", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(927), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "港元", null, null },
                    { "b23014ed-08bd-4db3-aa4a-5432db0176fa", "TWD", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(941), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "新台币", null, null },
                    { "e288fab2-3239-4be0-9682-dcb53e8c8cd2", "JPY", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(906), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "日元", null, null },
                    { "e37cd628-00e0-4036-b952-6f44038a549a", "MOP", new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(920), "484488fe-9930-4d4f-bf1a-59a4fedc7529", null, null, false, "澳门币", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentClassificationId", "ParentId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "e3e389f7-8e56-4492-acf9-28220459b044", false, new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(832), "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", null, null, false, "其他", null, null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "1c9bb153-ba4c-458b-8f24-67de6d09ba6e", false, new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(28), "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", null, null, false, "Consumer", null, null },
                    { "484488fe-9930-4d4f-bf1a-59a4fedc7529", false, new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(19), "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", null, null, false, "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", false, new DateTime(2024, 11, 21, 17, 3, 51, 550, DateTimeKind.Local).AddTicks(76), "c8873ec9-cf40-49d1-b8e5-4507e9d25b53", null, null, "admin@miaoshu.xyz", false, "9rVToUu4HHPeOTJuvQXlJ8ovy5kkwXqARFzBZjp3fFY=", "", "484488fe-9930-4d4f-bf1a-59a4fedc7529", "637768579c8e4d9091dc9d132ce3d9de", null, null, "admin" });
        }
    }
}
