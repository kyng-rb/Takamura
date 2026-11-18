using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Takamura.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_PeriodAllocation_BudgetId",
                table: "PeriodAllocation");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId_Description",
                table: "SubCategory",
                columns: new[] { "CategoryId", "Description" },
                unique: true,
                filter: "[Status] <> 'Deleted'");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodAllocation_BudgetId_SubCategoryId_YearFrom_MonthFrom",
                table: "PeriodAllocation",
                columns: new[] { "BudgetId", "SubCategoryId", "YearFrom", "MonthFrom" },
                unique: true,
                filter: "[Status] <> 'Deleted'");

            migrationBuilder.CreateIndex(
                name: "IX_Category_Description",
                table: "Category",
                column: "Description",
                unique: true,
                filter: "[Status] <> 'Deleted'");

            migrationBuilder.CreateIndex(
                name: "IX_Budget_Title",
                table: "Budget",
                column: "Title",
                unique: true,
                filter: "[Status] <> 'Deleted'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubCategory_CategoryId_Description",
                table: "SubCategory");

            migrationBuilder.DropIndex(
                name: "IX_PeriodAllocation_BudgetId_SubCategoryId_YearFrom_MonthFrom",
                table: "PeriodAllocation");

            migrationBuilder.DropIndex(
                name: "IX_Category_Description",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Budget_Title",
                table: "Budget");

            migrationBuilder.CreateIndex(
                name: "IX_SubCategory_CategoryId",
                table: "SubCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodAllocation_BudgetId",
                table: "PeriodAllocation",
                column: "BudgetId");
        }
    }
}
