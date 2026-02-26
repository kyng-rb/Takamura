using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Components.Pages.Allocations.Create;
using Takamura.Web.Components.Pages.Allocations.Copy;
using Takamura.Web.Services.Allocation.Retrieve;

namespace Takamura.Web.Components.Pages.Allocations.Retrieve;

public partial class AllocationRetrievePage : IDisposable
{
    [Parameter]
    public int BudgetId { get; set; }

    [Inject]
    private AllocationRetrieveService AllocationRetrieveService { get; set; } = default!;

    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private readonly CancellationTokenSource _requestCts = new();
    private readonly CultureInfo _usdCulture = CultureInfo.GetCultureInfo("en-US");
    private readonly AggregateDefinition<AllocationGridRow> _sumAggregate =
        AggregateDefinition<AllocationGridRow>.SimpleSum("C1", CultureInfo.GetCultureInfo("en-US"));
    private readonly IReadOnlyList<int> _availableYears =
        Enumerable.Range(DateTime.Today.Year - 10, 12).Reverse().ToArray();
    private readonly (int Month, string Label)[] _months = Enumerable.Range(1, 12)
        .Select(month => (month, new DateTime(2000, month, 1).ToString("MMMM", CultureInfo.InvariantCulture)))
        .ToArray();
    private IReadOnlyList<AllocationGridRow> _rows = [];
    private IReadOnlyList<AllocationRetrieveItem> _sourceMonthAllocations = [];
    private int _year = DateTime.Today.Year;
    private int? _month = DateTime.Today.Month;
    private string? _errorMessage;
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

    private async Task OnMonthChanged(int? value)
    {
        _month = value;
        await LoadData();
    }

    private async Task OpenCreateAllocationDialog()
    {
        var dialog = await DialogService.ShowAsync<AllocationCreateDialog>(
            "Create Allocation",
            new DialogParameters
            {
                { nameof(AllocationCreateDialog.BudgetId), BudgetId },
                { nameof(AllocationCreateDialog.Year), _year }
            },
            new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            });

        var result = await dialog.Result;
        if (result is { Canceled: false })
            await LoadData();
    }

    private async Task OpenCopyAllocationDialog()
    {
        if (_month is null)
        {
            _errorMessage = "Select month before using Copy To";
            return;
        }

        var dialog = await DialogService.ShowAsync<AllocationCopyDialog>(
            "Copy Allocations",
            new DialogParameters
            {
                { nameof(AllocationCopyDialog.BudgetId), BudgetId },
                { nameof(AllocationCopyDialog.SourceYear), _year },
                { nameof(AllocationCopyDialog.SourceMonth), _month },
                { nameof(AllocationCopyDialog.SourceAllocations), _sourceMonthAllocations }
            },
            new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Large,
                FullWidth = true
            });

        var result = await dialog.Result;
        if (result is { Canceled: false })
            await LoadData();
    }

    private async Task LoadData()
    {
        _loading = true;
        await ErrorRedirectExecutor.Run(async () =>
        {
            var output = await AllocationRetrieveService.Handle(BudgetId, _year, _requestCts.Token);
            BuildTable(output.Allocations ?? []);
        }, Nav, () => _loading = false);
    }

    private void BuildTable(IReadOnlyCollection<AllocationRetrieveItem> allocations)
    {
        var filtered = allocations
            .Where(x => x.Year == _year)
            .Where(x => _month is null || x.Month == _month.Value)
            .ToArray();

        _sourceMonthAllocations = filtered;

        _rows = filtered
            .Select(x => new AllocationGridRow
            {
                Category = x.Category,
                SubCategory = x.SubCategory,
                MonthGroup = $"{x.Month:00} - {MonthName(x.Month)}",
                Description = x.Description,
                Type = x.Type,
                Amount = x.Amount
            })
            .OrderBy(x => x.Category)
            .ThenBy(x => x.SubCategory)
            .ThenBy(x => x.MonthGroup)
            .ThenBy(x => x.Description)
            .ToArray();
    }

    public void Dispose()
    {
        _requestCts.Cancel();
        _requestCts.Dispose();
    }

    private static string MonthName(int month) =>
        CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);

    public sealed class AllocationGridRow
    {
        public string Category { get; init; } = string.Empty;
        public string SubCategory { get; init; } = string.Empty;
        public string MonthGroup { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
        public decimal Amount { get; init; }
    }
}
