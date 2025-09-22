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
                name: "SubCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(100)", nullable: false),
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
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bill", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bill_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
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
                name: "PeriodBudget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MonthFrom = table.Column<int>(type: "int", nullable: false),
                    YearFrom = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(20)", nullable: false),
                    SubCategoryId = table.Column<int>(type: "int", nullable: false),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    LastUpdateAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodBudget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeriodBudget_Budget_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeriodBudget_SubCategory_SubCategoryId",
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
                columns: new[] { "Id", "CategoryId", "CreatedAt", "Description", "LastUpdateAt", "Status" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Comida", null, "Created" },
                    { 2, 2, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hipoteca mensual", null, "Created" },
                    { 3, 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Electricidad", null, "Created" },
                    { 4, 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mantenimiento", null, "Created" },
                    { 5, 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Limpieza", null, "Created" },
                    { 6, 4, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Animalijos", null, "Created" },
                    { 7, 8, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Gasolina", null, "Created" },
                    { 8, 8, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lavado", null, "Created" },
                    { 9, 5, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Reynaldo", null, "Created" },
                    { 10, 5, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Jenifer", null, "Created" },
                    { 11, 3, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Internet residencial", null, "Created" },
                    { 12, 6, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ahorro principal", null, "Created" },
                    { 13, 6, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ayuda a los viejos", null, "Created" },
                    { 14, 2, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hipoteca adelantada", null, "Created" },
                    { 15, 7, new DateTime(2025, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Inesperado", null, "Created" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bill_BudgetId",
                table: "Bill",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Bill_SubCategoryId",
                table: "Bill",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodBudget_BudgetId",
                table: "PeriodBudget",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodBudget_SubCategoryId",
                table: "PeriodBudget",
                column: "SubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bill");

            migrationBuilder.DropTable(
                name: "PeriodBudget");

            migrationBuilder.DropTable(
                name: "Budget");

            migrationBuilder.DropTable(
                name: "SubCategory");

            migrationBuilder.DropTable(
                name: "Category");
        }
    }
}
