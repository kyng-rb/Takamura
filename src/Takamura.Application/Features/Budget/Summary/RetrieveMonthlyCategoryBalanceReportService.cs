using ClosedXML.Excel;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Takamura.Application.Database;

namespace Takamura.Application.Features.Budget.Summary;

public class RetrieveMonthlyCategoryBalanceReportService(DatabaseContext dbContext)
{
    private readonly DatabaseContext _dbContext = dbContext;

    public async Task<Result<RetrieveMonthlyBalanceReportServiceOutput>> Handle(RetrieveMonthlyCategoryBalanceReportServiceInput input)
    {
        if (input.BudgetId <= 0)
            return Result.Fail(new Error("Invalid BudgetId"));

        if (input.Year < 2025)
            return Result.Fail(new Error("Invalid Year"));

        var categoryRecords = await _dbContext.GetMonthlyCategoryBalance(input.BudgetId, input.Year, default)
            .ToListAsync()
            .ConfigureAwait(false);

        var subCategoryRecords = await _dbContext.GetMonthlySubCategoryBalance(input.BudgetId, input.Year, default)
            .ToListAsync()
            .ConfigureAwait(false);

        var yearToDateBalance = await _dbContext.GetYearToDateBalance(input.BudgetId)
            .ToListAsync()
            .ConfigureAwait(false);

        var categoryBalance = categoryRecords.AsBalanceDataTable();
        var categoryBudget = categoryRecords.AsBudgetDataTable();
        var subcategoryBalance = subCategoryRecords.AsBalanceDataTable();
        var subcategoryBudget = subCategoryRecords.AsBudgetDataTable();
        var yearToDateRecords = yearToDateBalance.AsDataTable();

        using var excelFile = new XLWorkbook();

        excelFile.Worksheets.Add(yearToDateRecords, "Year To Date Balance").FormatYearToDateBalanceSheet();
        excelFile.Worksheets.Add(subcategoryBalance, "Sub Category Balance").FormatSubCategoryBalanceSheet();
        excelFile.Worksheets.Add(categoryBalance, "Category Balance").FormatCategoryBalanceSheet();
        excelFile.Worksheets.Add(categoryBudget, "Category Budget").FormatCategoryBudgetSheet();
        excelFile.Worksheets.Add(subcategoryBudget, "Sub Category Budget").FormatSubCategoryBudgetSheet();

        var path = Path.Combine("/Users/il_mostro/Desktop/reports", $"report.xlsx");
        excelFile.SaveAs(path);

        var output = new RetrieveMonthlyBalanceServiceOutput(categoryRecords);

        return Result.Ok(new RetrieveMonthlyBalanceReportServiceOutput(path));
    }
}

public record RetrieveMonthlyBalanceReportServiceOutput(string Location);

public record RetrieveMonthlyCategoryBalanceReportServiceInput(int BudgetId, int Year);