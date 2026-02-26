using System.Globalization;
using Microsoft.AspNetCore.Components;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Services.Balances.RetrieveSubCategoryBalance;

namespace Takamura.Web.Components.Pages.Balances.Retrieve;

public partial class BalanceRetrievePage : IDisposable
{
    private enum BalanceViewLevel
    {
        SubCategory = 1,
        Category = 2
    }

    private enum BalanceTimeframe
    {
        Monthly = 1,
        YearToDate = 2
    }

    [Parameter]
    public int BudgetId { get; set; }

    [Inject]
    private RetrieveSubCategoryBalanceService RetrieveSubCategoryBalanceService { get; set; } = default!;

    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    private readonly CancellationTokenSource _requestCts = new();
    private readonly CultureInfo _usdCulture = CultureInfo.GetCultureInfo("en-US");
    private readonly IReadOnlyList<int> _availableYears =
        Enumerable.Range(DateTime.Today.Year - 10, 12).Reverse().ToArray();
    private readonly (BalanceViewLevel Value, string Label)[] _viewLevels =
    [
        (BalanceViewLevel.SubCategory, "Subcategory"),
        (BalanceViewLevel.Category, "Category")
    ];
    private readonly (BalanceTimeframe Value, string Label)[] _timeframes =
    [
        (BalanceTimeframe.Monthly, "Monthly"),
        (BalanceTimeframe.YearToDate, "Year To Date")
    ];
    private IReadOnlyList<BalanceMatrixRow> _rows = [];
    private IReadOnlyCollection<RetrieveSubCategoryBalanceItem> _sourceItems = [];
    private int _year = DateTime.Today.Year;
    private BalanceViewLevel _viewLevel = BalanceViewLevel.SubCategory;
    private BalanceTimeframe _timeframe = BalanceTimeframe.Monthly;
    private bool _loading;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task OnYearChanged(int value)
    {
        _year = value;
        await LoadData();
    }

    private Task OnViewLevelChanged(BalanceViewLevel value)
    {
        _viewLevel = value;
        BuildMatrix(_sourceItems);
        return Task.CompletedTask;
    }

    private Task OnTimeframeChanged(BalanceTimeframe value)
    {
        _timeframe = value;
        BuildMatrix(_sourceItems);
        return Task.CompletedTask;
    }

    private async Task LoadData()
    {
        _loading = true;
        await ErrorRedirectExecutor.Run(async () =>
        {
            var output = await RetrieveSubCategoryBalanceService.Handle(BudgetId, _year, null, _requestCts.Token);
            _sourceItems = output.Balances ?? [];
            BuildMatrix(_sourceItems);
        }, Nav, () => _loading = false);
    }

    private void BuildMatrix(IReadOnlyCollection<RetrieveSubCategoryBalanceItem> items)
    {
        var filtered = items.Where(x => x.Year == _year);

        if (_viewLevel == BalanceViewLevel.Category)
        {
            _rows = filtered
                .GroupBy(x => x.Category)
                .Select(group => BuildRow(group.Key, "-", group, DateTime.Today.Month))
                .OrderBy(x => x.Category)
                .ToArray();
            return;
        }

        _rows = filtered
            .GroupBy(x => new { x.Category, x.SubCategory })
            .Select(group => BuildRow(group.Key.Category, group.Key.SubCategory, group, DateTime.Today.Month))
            .OrderBy(x => x.Category)
            .ThenBy(x => x.SubCategory)
            .ToArray();
    }

    private static BalanceMatrixRow BuildRow(
        string category,
        string subCategory,
        IEnumerable<RetrieveSubCategoryBalanceItem> entries,
        int ytdMonthLimit)
    {
        var byMonth = entries
            .GroupBy(x => x.Month)
            .ToDictionary(
                g => g.Key,
                g => new MonthBalanceCell(
                    g.Sum(x => x.PeriodBudget),
                    g.Sum(x => x.AvailableAmount)));

        return new BalanceMatrixRow
        {
            Category = category,
            SubCategory = subCategory,
            January = GetMonthValue(byMonth, 1),
            February = GetMonthValue(byMonth, 2),
            March = GetMonthValue(byMonth, 3),
            April = GetMonthValue(byMonth, 4),
            May = GetMonthValue(byMonth, 5),
            June = GetMonthValue(byMonth, 6),
            July = GetMonthValue(byMonth, 7),
            August = GetMonthValue(byMonth, 8),
            September = GetMonthValue(byMonth, 9),
            October = GetMonthValue(byMonth, 10),
            November = GetMonthValue(byMonth, 11),
            December = GetMonthValue(byMonth, 12),
            YearToDate = SumYtdFromDictionary(byMonth, ytdMonthLimit)
        };
    }

    private static MonthBalanceCell GetMonthValue(IReadOnlyDictionary<int, MonthBalanceCell> byMonth, int month) =>
        byMonth.TryGetValue(month, out var value) ? value : new MonthBalanceCell(0m, 0m);

    private static string MonthName(int month) =>
        CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);

    private static MonthBalanceCell GetMonthCell(BalanceMatrixRow row, int month) =>
        month switch
        {
            1 => row.January,
            2 => row.February,
            3 => row.March,
            4 => row.April,
            5 => row.May,
            6 => row.June,
            7 => row.July,
            8 => row.August,
            9 => row.September,
            10 => row.October,
            11 => row.November,
            12 => row.December,
            _ => new MonthBalanceCell(0m, 0m)
        };

    private static string GetAvailableCellStyle(decimal availableAmount)
    {
        var background = availableAmount switch
        {
            > 0m => "#dcfce7",
            < 0m => "#fee2e2",
            _ => "#dbeafe"
        };

        return $"background-color:{background};";
    }

    private static MonthBalanceCell SumMonthFromRows(IEnumerable<BalanceMatrixRow> rows, int month)
    {
        var monthValues = rows.Select(row => GetMonthCell(row, month));
        return new MonthBalanceCell(
            monthValues.Sum(x => x.PeriodBudget),
            monthValues.Sum(x => x.AvailableAmount));
    }

    private static MonthBalanceCell SumYtdFromRows(IEnumerable<BalanceMatrixRow> rows)
    {
        var monthValues = rows.Select(row => row.YearToDate);
        return new MonthBalanceCell(
            monthValues.Sum(x => x.PeriodBudget),
            monthValues.Sum(x => x.AvailableAmount));
    }

    private static MonthBalanceCell SumYtdFromDictionary(
        IReadOnlyDictionary<int, MonthBalanceCell> byMonth,
        int monthLimit)
    {
        var values = Enumerable.Range(1, Math.Clamp(monthLimit, 1, 12))
            .Select(month => GetMonthValue(byMonth, month));

        return new MonthBalanceCell(
            values.Sum(x => x.PeriodBudget),
            values.Sum(x => x.AvailableAmount));
    }

    public void Dispose()
    {
        _requestCts.Cancel();
        _requestCts.Dispose();
    }

    public sealed class BalanceMatrixRow
    {
        public string Category { get; init; } = string.Empty;
        public string SubCategory { get; init; } = string.Empty;
        public MonthBalanceCell January { get; init; } = new(0m, 0m);
        public MonthBalanceCell February { get; init; } = new(0m, 0m);
        public MonthBalanceCell March { get; init; } = new(0m, 0m);
        public MonthBalanceCell April { get; init; } = new(0m, 0m);
        public MonthBalanceCell May { get; init; } = new(0m, 0m);
        public MonthBalanceCell June { get; init; } = new(0m, 0m);
        public MonthBalanceCell July { get; init; } = new(0m, 0m);
        public MonthBalanceCell August { get; init; } = new(0m, 0m);
        public MonthBalanceCell September { get; init; } = new(0m, 0m);
        public MonthBalanceCell October { get; init; } = new(0m, 0m);
        public MonthBalanceCell November { get; init; } = new(0m, 0m);
        public MonthBalanceCell December { get; init; } = new(0m, 0m);
        public MonthBalanceCell YearToDate { get; init; } = new(0m, 0m);

        public decimal JanuaryPeriodBudget => January.PeriodBudget;
        public decimal JanuaryAvailable => January.AvailableAmount;
        public decimal FebruaryPeriodBudget => February.PeriodBudget;
        public decimal FebruaryAvailable => February.AvailableAmount;
        public decimal MarchPeriodBudget => March.PeriodBudget;
        public decimal MarchAvailable => March.AvailableAmount;
        public decimal AprilPeriodBudget => April.PeriodBudget;
        public decimal AprilAvailable => April.AvailableAmount;
        public decimal MayPeriodBudget => May.PeriodBudget;
        public decimal MayAvailable => May.AvailableAmount;
        public decimal JunePeriodBudget => June.PeriodBudget;
        public decimal JuneAvailable => June.AvailableAmount;
        public decimal JulyPeriodBudget => July.PeriodBudget;
        public decimal JulyAvailable => July.AvailableAmount;
        public decimal AugustPeriodBudget => August.PeriodBudget;
        public decimal AugustAvailable => August.AvailableAmount;
        public decimal SeptemberPeriodBudget => September.PeriodBudget;
        public decimal SeptemberAvailable => September.AvailableAmount;
        public decimal OctoberPeriodBudget => October.PeriodBudget;
        public decimal OctoberAvailable => October.AvailableAmount;
        public decimal NovemberPeriodBudget => November.PeriodBudget;
        public decimal NovemberAvailable => November.AvailableAmount;
        public decimal DecemberPeriodBudget => December.PeriodBudget;
        public decimal DecemberAvailable => December.AvailableAmount;
    }

    public sealed record MonthBalanceCell(decimal PeriodBudget, decimal AvailableAmount);
}
