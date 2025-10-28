using System.Data;
using System.Globalization;
using ClosedXML.Excel;
using Takamura.Application.Database.Entities.Summary;

namespace Takamura.Application.Features.Budget.Summary;

public static class SummaryExtensions
{

    public static DataTable AsBalanceDataTable(this List<MonthlyCategoryBalance> records)
    {
        var table = new DataTable();
        table.Columns.Add("Category", typeof(string));

        for (int month = 1; month <= 12; month++)
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            var availableColumn = new DataColumn($"{monthName}", typeof(decimal))
            {
                DefaultValue = 0
            };

            table.Columns.Add(availableColumn);
        }

        var categories = records
            .Select(r => r.Category)
            .Distinct();

        foreach (var category in categories)
        {
            var row = table.NewRow();
            row["Category"] = category;

            for (int month = 1; month <= 12; month++)
            {
                var record = records.FirstOrDefault(r => r.Category == category && r.Month == month);

                var column = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}";

                if (record is null)
                    continue;

                row[column] = record.AvailableAmount;
            }

            table.Rows.Add(row);
        }

        return table;
    }

    public static DataTable AsBudgetDataTable(this List<MonthlyCategoryBalance> records)
    {
        var table = new DataTable();
        table.Columns.Add("Category", typeof(string));

        for (int month = 1; month <= 12; month++)
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            var availableColumn = new DataColumn($"{monthName}", typeof(decimal))
            {
                DefaultValue = 0
            };

            table.Columns.Add(availableColumn);
        }

        var categories = records
            .Select(r => r.Category)
            .Distinct();

        foreach (var category in categories)
        {
            var row = table.NewRow();
            row["Category"] = category;

            for (int month = 1; month <= 12; month++)
            {
                var record = records.FirstOrDefault(r => r.Category == category && r.Month == month);

                var column = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}";

                if (record is null)
                    continue;

                row[column] = record.PeriodBudget;
            }

            table.Rows.Add(row);
        }

        return table;
    }

    public static DataTable AsBudgetDataTable(this List<MonthlySubCategoryBalance> records)
    {
        var table = new DataTable();
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("SubCategory", typeof(string));

        for (int month = 1; month <= 12; month++)
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            var availableColumn = new DataColumn($"{monthName}", typeof(decimal))
            {
                DefaultValue = 0
            };

            table.Columns.Add(availableColumn);
        }

        var subcategories = records
            .OrderBy(x => x.Category)
            .Select(x => new
            {
                x.Category,
                x.SubCategory
            })
            .Distinct();

        foreach (var subcategory in subcategories)
        {
            var row = table.NewRow();
            row["Category"] = subcategory.Category;
            row["SubCategory"] = subcategory.SubCategory;

            for (int month = 1; month <= 12; month++)
            {
                var record = records.FirstOrDefault(r =>
                    r.Category == subcategory.Category &&
                    r.SubCategory == subcategory.SubCategory &&
                    r.Month == month);

                var column = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}";

                if (record is null)
                    continue;

                row[column] = record.PeriodBudget;
            }

            table.Rows.Add(row);
        }

        return table;
    }


    public static DataTable AsBalanceDataTable(this List<MonthlySubCategoryBalance> records)
    {
        var table = new DataTable();
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("SubCategory", typeof(string));

        for (int month = 1; month <= 12; month++)
        {
            var monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);

            var availableColumn = new DataColumn($"{monthName}", typeof(decimal))
            {
                DefaultValue = 0
            };

            table.Columns.Add(availableColumn);
        }

        var subcategories = records
            .OrderBy(x => x.Category)
            .Select(x => new
            {
                x.Category,
                x.SubCategory
            })
            .Distinct();

        foreach (var subcategory in subcategories)
        {
            var row = table.NewRow();
            row["Category"] = subcategory.Category;
            row["SubCategory"] = subcategory.SubCategory;

            for (int month = 1; month <= 12; month++)
            {
                var record = records.FirstOrDefault(r =>
                    r.Category == subcategory.Category &&
                    r.SubCategory == subcategory.SubCategory &&
                    r.Month == month);

                var column = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)}";

                if (record is null)
                    continue;

                row[column] = record.AvailableAmount;
            }

            table.Rows.Add(row);
        }

        return table;
    }


    public static DataTable AsDataTable(this List<YearToDateBalance> records)
    {
        var table = new DataTable();
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("SubCategory", typeof(string));
        table.Columns.Add("Balance", typeof(decimal));
        table.Columns.Add("Budget", typeof(decimal));

        foreach (var item in records)
        {
            var row = table.NewRow();
            row["Category"] = item.Category;
            row["SubCategory"] = item.SubCategory;
            row["Balance"] = item.AvailableAmount;
            row["Budget"] = item.PeriodBudget;

            table.Rows.Add(row);
        }

        return table;
    }



}