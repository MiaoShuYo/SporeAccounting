using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SporeAccounting.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", nullable: false),
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
                    table.PrimaryKey("PK_Currency", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExchangeRate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ConvertCurrency = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
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
                    table.PrimaryKey("PK_ExchangeRate", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "IncomeExpenditureClassification",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CanDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ParentIncomeExpenditureClassificationId = table.Column<string>(type: "nvarchar(36)", nullable: true),
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
                    table.PrimaryKey("PK_IncomeExpenditureClassification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeExpenditureClassification_IncomeExpenditureClassificat~",
                        column: x => x.ParentIncomeExpenditureClassificationId,
                        principalTable: "IncomeExpenditureClassification",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SysRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    CanDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SysUrl",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    RequestMethod = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    CanDelete = table.Column<string>(type: "nvarchar(200)", nullable: false),
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
                    table.PrimaryKey("PK_SysUrl", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SysUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(11)", nullable: false),
                    CanDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(36)", nullable: false),
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
                    table.PrimaryKey("PK_SysUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SysUser_SysRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SysRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SysRoleUrl",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    UrlId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    CanDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_SysRoleUrl_SysRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SysRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SysRoleUrl_SysUrl_UrlId",
                        column: x => x.UrlId,
                        principalTable: "SysUrl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountBook",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_AccountBook_SysUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    IncomeExpenditureClassificationId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Period = table.Column<int>(type: "int", nullable: false),
                    Remaining = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remark = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: false),
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
                    table.PrimaryKey("PK_Budget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budget_IncomeExpenditureClassification_IncomeExpenditureClas~",
                        column: x => x.IncomeExpenditureClassificationId,
                        principalTable: "IncomeExpenditureClassification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Budget_SysUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ConfigType = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", nullable: false),
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
                    table.PrimaryKey("PK_Config", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Config_SysUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: true),
                    Quarter = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ClassificationId = table.Column<string>(type: "nvarchar(36)", nullable: false),
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
                    table.PrimaryKey("PK_Report", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Report_IncomeExpenditureClassification_ClassificationId",
                        column: x => x.ClassificationId,
                        principalTable: "IncomeExpenditureClassification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Report_SysUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReportLog",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    ReportId = table.Column<string>(type: "nvarchar(36)", nullable: false),
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
                    table.PrimaryKey("PK_ReportLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportLog_SysUser_UserId",
                        column: x => x.UserId,
                        principalTable: "SysUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "IncomeExpenditureRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    BeforAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AfterAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IncomeExpenditureClassificationId = table.Column<string>(type: "nvarchar(36)", nullable: false),
                    RecordDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AccountBookId = table.Column<string>(type: "nvarchar(36)", nullable: false),
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
                        name: "FK_IncomeExpenditureRecord_AccountBook_AccountBookId",
                        column: x => x.AccountBookId,
                        principalTable: "AccountBook",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    { "098e71cf-4630-467b-a530-cea4b30e9070", "HKD", new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1339), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "港元", null, null },
                    { "409f7f1d-3430-4f82-9180-520ac1dadbc9", "EUR", new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1309), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "欧元", null, null },
                    { "422a920d-12e9-4263-a1b6-9d6e4e3366ea", "JPY", new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1316), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "日元", null, null },
                    { "4b6a9c6f-d77f-4087-af5d-2d4f85375bda", "GBP", new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1322), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "英镑", null, null },
                    { "551b2b37-dfd8-49df-bfc5-c78f068b2d01", "MOP", new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1332), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "澳门币", null, null },
                    { "7b01d6fa-e673-4bfd-8112-3e988971d91c", "KRW", new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1346), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "韩圆", null, null },
                    { "a374bbfa-99bd-4f14-9f11-49260528d7a4", "TWD", new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1352), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "新台币", null, null },
                    { "e25b4885-cf61-4249-b86f-0130defd1d57", "USD", new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1302), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "美元", null, null },
                    { "e7b3e54d-dbf3-432e-b6fb-b251ffa844b6", "CNY", new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1294), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "人民币", null, null }
                });

            migrationBuilder.InsertData(
                table: "IncomeExpenditureClassification",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "Name", "ParentIncomeExpenditureClassificationId", "Type", "UpdateDateTime", "UpdateUserId" },
                values: new object[] { "10ce6d08-3de2-466e-a9bb-e15cb4eec56f", false, new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(1183), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "其他", null, -1, null, null });

            migrationBuilder.InsertData(
                table: "SysRole",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "IsDeleted", "RoleName", "UpdateDateTime", "UpdateUserId" },
                values: new object[,]
                {
                    { "10389aa0-b6f2-4241-9a77-ca8020656bb6", false, new DateTime(2025, 2, 6, 1, 11, 46, 442, DateTimeKind.Local).AddTicks(9968), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "Consumer", null, null },
                    { "cef80881-fe89-4b1f-85ad-83184777d61b", false, new DateTime(2025, 2, 6, 1, 11, 46, 442, DateTimeKind.Local).AddTicks(9957), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, false, "Administrator", null, null }
                });

            migrationBuilder.InsertData(
                table: "SysUser",
                columns: new[] { "Id", "CanDelete", "CreateDateTime", "CreateUserId", "DeleteDateTime", "DeleteUserId", "Email", "IsDeleted", "Password", "PhoneNumber", "RoleId", "Salt", "UpdateDateTime", "UpdateUserId", "UserName" },
                values: new object[] { "b47637e2-603f-4df0-abe9-88d70fa870ee", false, new DateTime(2025, 2, 6, 1, 11, 46, 443, DateTimeKind.Local).AddTicks(24), "b47637e2-603f-4df0-abe9-88d70fa870ee", null, null, "admin@miaoshu.xyz", false, "M9dA4EKZ3DtaVqNsrHwLa5vBggrwy27eoz/2sRsPtSI=", "", "cef80881-fe89-4b1f-85ad-83184777d61b", "ef612ae74f0b467c9db7cb213947699f", null, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountBook_UserId",
                table: "AccountBook",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_IncomeExpenditureClassificationId",
                table: "Budget",
                column: "IncomeExpenditureClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_UserId",
                table: "Budget",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Config_UserId",
                table: "Config",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeExpenditureClassification_ParentIncomeExpenditureClass~",
                table: "IncomeExpenditureClassification",
                column: "ParentIncomeExpenditureClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeExpenditureRecord_AccountBookId",
                table: "IncomeExpenditureRecord",
                column: "AccountBookId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Report_ClassificationId",
                table: "Report",
                column: "ClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_UserId",
                table: "Report",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportLog_UserId",
                table: "ReportLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRoleUrl_RoleId",
                table: "SysRoleUrl",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRoleUrl_UrlId",
                table: "SysRoleUrl",
                column: "UrlId");

            migrationBuilder.CreateIndex(
                name: "IX_SysUser_RoleId",
                table: "SysUser",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "Config");

            migrationBuilder.DropTable(
                name: "ExchangeRate");

            migrationBuilder.DropTable(
                name: "IncomeExpenditureRecord");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "ReportLog");

            migrationBuilder.DropTable(
                name: "SysRoleUrl");

            migrationBuilder.DropTable(
                name: "AccountBook");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "IncomeExpenditureClassification");

            migrationBuilder.DropTable(
                name: "SysUrl");

            migrationBuilder.DropTable(
                name: "SysUser");

            migrationBuilder.DropTable(
                name: "SysRole");
        }
    }
}
