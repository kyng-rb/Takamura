using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Takamura.Application.Database.Migrations
{
    /// <inheritdoc />
    public partial class initial_setup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "varchar(100)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Budget", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(100)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyBudget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyBudget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyBudget_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(100)", nullable: false),
                    MovementType = table.Column<string>(type: "varchar(20)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bill",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "varchar(100)", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    MonthlyBudgetId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bill_MonthlyBudget_MonthlyBudgetId",
                        column: x => x.MonthlyBudgetId,
                        principalTable: "MonthlyBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bill_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubCategoryBudget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthFrom = table.Column<int>(type: "int", nullable: false),
                    YearFrom = table.Column<int>(type: "int", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    MonthlyBudgetId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategoryBudget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategoryBudget_MonthlyBudget_MonthlyBudgetId",
                        column: x => x.MonthlyBudgetId,
                        principalTable: "MonthlyBudget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubCategoryBudget_SubCategory_SubCategoryId",
                        column: x => x.SubCategoryId,
                        principalTable: "SubCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "CreatedAt", "Description", "LastUpdateAt", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Comida", null, "Created" },
                    { 2, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hipoteca", null, "Created" },
                    { 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mantenimiento de la casita", null, "Created" },
                    { 4, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Animalijos", null, "Created" },
                    { 5, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Entretenimiento personal", null, "Created" },
                    { 6, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ahorro", null, "Created" },
                    { 7, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Inesperados", null, "Created" },
                    { 8, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vehiculos", null, "Created" }
                });

            migrationBuilder.InsertData(
                table: "SubCategory",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "LastUpdateAt", "MovementType", "Status" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Comida", null, "Expense", "Created" },
                    { 2, 2, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hipoteca mensual", null, "Expense", "Created" },
                    { 3, 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Electricidad", null, "Expense", "Created" },
                    { 4, 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mantenimiento", null, "Expense", "Created" },
                    { 5, 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Limpieza", null, "Expense", "Created" },
                    { 6, 4, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Animalijos", null, "Expense", "Created" },
                    { 7, 8, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gasolina", null, "Expense", "Created" },
                    { 8, 8, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lavado", null, "Expense", "Created" },
                    { 9, 5, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reynaldo", null, "Expense", "Created" },
                    { 10, 5, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jenifer", null, "Expense", "Created" },
                    { 11, 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Internet residencial", null, "Expense", "Created" },
                    { 12, 6, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ahorro principal", null, "Reserve", "Created" },
                    { 13, 6, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ayuda a los viejos", null, "Reserve", "Created" },
                    { 14, 2, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hipoteca adelantada", null, "Reserve", "Created" },
                    { 15, 7, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Inesperado", null, "Expense", "Created" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bill_MonthlyBudgetId",
                table: "Bill",
                column: "MonthlyBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_SubCategoryId",
                table: "Bill",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyBudget_BudgetId",
                table: "MonthlyBudget",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryBudget_MonthlyBudgetId",
                table: "SubCategoryBudget",
                column: "MonthlyBudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoryBudget_SubCategoryId",
                table: "SubCategoryBudget",
                column: "SubCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bill");

            migrationBuilder.DropTable(
                name: "SubCategoryBudget");

            migrationBuilder.DropTable(
                name: "MonthlyBudget");

            migrationBuilder.DropTable(
                name: "SubCategory");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
