using ClosedXML.Excel;

namespace Takamura.Application.Features.Budget.Summary;

public static class ReportExtensions
{
    const string currencyFormat = "$#,##0";

    public static void FormatCategoryBudgetSheet(this IXLWorksheet worksheet)
    {
        worksheet.SetBaseStyle();
        worksheet.GetColumnNamesRange().HighlightRange(XLColor.Cerulean);
        worksheet.GetCategoriesRange().HighlightRange(XLColor.AmberSaeEce);

        foreach (var column in worksheet.ColumnsUsed())
        {
            if (column.IsNotAnAmountColumn())
                continue;

            var dataRange = worksheet.Range(
                column.Cell(2).Address,
                column.Cell(worksheet.LastRowUsed()!.RowNumber()).Address
            );

            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        worksheet.AddTotalRow();
    }

    public static void FormatSubCategoryBudgetSheet(this IXLWorksheet worksheet)
    {
        worksheet.SetBaseStyle();
        worksheet.GetColumnNamesRange().HighlightRange(XLColor.Cerulean);
        worksheet.GetCategoriesRange().HighlightRange(XLColor.AmberSaeEce);
        worksheet.GetSubCategoriesRange().HighlightRange(XLColor.GoldenPoppy);

        foreach (var column in worksheet.ColumnsUsed())
        {
            if (column.IsNotAnAmountColumn())
                continue;

            var dataRange = worksheet.Range(
                column.Cell(2).Address,
                column.Cell(worksheet.LastRowUsed()!.RowNumber()).Address
            );

            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        worksheet.AddTotalRow();
    }


    public static void FormatSubCategoryBalanceSheet(this IXLWorksheet worksheet)
    {
        worksheet.SetBaseStyle();
        worksheet.GetColumnNamesRange().HighlightRange(XLColor.Cerulean);
        worksheet.GetCategoriesRange().HighlightRange(XLColor.AmberSaeEce);
        worksheet.GetSubCategoriesRange().HighlightRange(XLColor.GoldenPoppy);

        foreach (var column in worksheet.ColumnsUsed())
        {
            if (column.IsNotAnAmountColumn())
                continue;

            var dataRange = worksheet.Range(
                column.Cell(2).Address,
                column.Cell(worksheet.LastRowUsed()!.RowNumber()).Address
            );

            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataRange.AddPositiveAmountConditionalFormat();
            dataRange.AddNegativeAmountConditionalFormat();
        }

        worksheet.AddTotalRow();
    }

    public static void FormatCategoryBalanceSheet(this IXLWorksheet worksheet)
    {
        worksheet.SetBaseStyle();
        worksheet.GetColumnNamesRange().HighlightRange(XLColor.Cerulean);
        worksheet.GetCategoriesRange().HighlightRange(XLColor.AmberSaeEce);

        foreach (var column in worksheet.ColumnsUsed())
        {
            if (column.IsNotAnAmountColumn())
                continue;

            var dataRange = worksheet.Range(
                column.Cell(2).Address,
                column.Cell(worksheet.LastRowUsed()!.RowNumber()).Address
            );

            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataRange.AddPositiveAmountConditionalFormat();
            dataRange.AddNegativeAmountConditionalFormat();
        }

        worksheet.AddTotalRow();
    }

    public static void FormatYearToDateBalanceSheet(this IXLWorksheet worksheet)
    {
        worksheet.SetBaseStyle();
        worksheet.GetColumnNamesRange().HighlightRange(XLColor.Cerulean);
        worksheet.GetCategoriesRange().HighlightRange(XLColor.AmberSaeEce);
        worksheet.GetSubCategoriesRange().HighlightRange(XLColor.GoldenPoppy);

        foreach (var column in worksheet.ColumnsUsed())
        {
            if (column.IsNotAnAmountColumn())
                continue;

            var dataRange = worksheet.Range(
                column.Cell(2).Address,
                column.Cell(worksheet.LastRowUsed()!.RowNumber()).Address
            );

            dataRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            dataRange.AddPositiveAmountConditionalFormat();
            dataRange.AddNegativeAmountConditionalFormat();
        }

        worksheet.AddTotalRow();
    }

    private static void AddTotalRow(this IXLWorksheet sheet)
    {
        var table = sheet.Table(0);
        table.ShowTotalsRow = true;
        table.Field(0).TotalsRowLabel = "Total";

        foreach (var column in sheet.ColumnsUsed())
        {
            if (column.IsNotAnAmountColumn())
                continue;

            table.Field(column.ColumnNumber() - 1).TotalsRowFunction = XLTotalsRowFunction.Sum;
        }
    }

    private static bool IsNotAnAmountColumn(this IXLColumn column)
    {
        var secondUsedCell = column.Cell(column.FirstCellUsed()!.Address.RowNumber + 1);
        var isAmount = decimal.TryParse(secondUsedCell.GetValue<string>(), out _);
        return !isAmount;
    }

    private static void AddNegativeAmountConditionalFormat(this IXLRange dataRange)
    {
        dataRange.AddConditionalFormat()
            .WhenLessThan(0)
            .Fill.SetBackgroundColor(XLColor.Cinnabar)
            .Font.SetFontColor(XLColor.White);
    }

    private static void AddPositiveAmountConditionalFormat(this IXLRange dataRange)
    {
        dataRange.AddConditionalFormat()
            .WhenGreaterThan(0)
            .Fill.SetBackgroundColor(XLColor.Pistachio)
            .Font.SetFontColor(XLColor.White);
    }

    private static void SetBaseStyle(this IXLWorksheet sheet)
    {
        var range = sheet.RangeUsed()!;
        range.Style.NumberFormat.Format = currencyFormat;
        range.Style.Fill.BackgroundColor = XLColor.White;
        range.Style.Font.FontColor = XLColor.Black;

        foreach (var column in sheet.ColumnsUsed())
        {
            column.AdjustToContents();
            column.Width += 3;
        }
    }

    private static void HighlightRange(this IXLRange range, XLColor color)
    {
        range.Style.Fill.BackgroundColor = color;
        range.Style.Font.FontColor = XLColor.White;
        range.Style.Font.SetBold();
    }

    private static IXLRange GetColumnNamesRange(this IXLWorksheet sheet)
    {
        return sheet.Range(
            sheet.Cell(1, 1),
            sheet.Cell(1, sheet.RangeUsed()!.LastColumn().ColumnNumber())
        );
    }

    private static IXLRange GetCategoriesRange(this IXLWorksheet sheet)
    {
        return sheet.Range(
            sheet.Cell(2, 1),
            sheet.Cell(sheet.LastRowUsed()!.RowNumber(), 1)
        );
    }

    private static IXLRange GetSubCategoriesRange(this IXLWorksheet sheet)
    {
        return sheet.Range(
            sheet.Cell(2, 2),
            sheet.Cell(sheet.LastRowUsed()!.RowNumber(), 2)
        );
    }
}