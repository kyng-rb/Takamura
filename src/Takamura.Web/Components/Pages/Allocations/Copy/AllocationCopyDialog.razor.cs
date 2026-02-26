using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Services.Allocation.Create;
using Takamura.Web.Services.Allocation.Retrieve;

namespace Takamura.Web.Components.Pages.Allocations.Copy;

public partial class AllocationCopyDialog : IDisposable
{
    [Parameter]
    public int BudgetId { get; set; }

    [Parameter]
    public int SourceYear { get; set; }

    [Parameter]
    public int? SourceMonth { get; set; }

    [Parameter]
    public IReadOnlyList<AllocationRetrieveItem>? SourceAllocations { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    private readonly AllocationCreateService _allocationCreateService;
    private readonly CancellationTokenSource _requestCts = new();
    private readonly CultureInfo _usdCulture = CultureInfo.GetCultureInfo("en-US");
    private readonly IReadOnlyList<int> _availableYears = Enumerable.Range(DateTime.Today.Year - 10, 12).Reverse().ToArray();
    private readonly (int Month, string Label)[] _months = Enumerable.Range(1, 12)
        .Select(month => (month, new DateTime(2000, month, 1).ToString("MMMM", CultureInfo.InvariantCulture)))
        .ToArray();

    private readonly List<AllocationRetrieveItem> _items = [];
    private int _sourceYear;
    private int? _sourceMonth;
    private int _targetYear;
    private int? _targetMonth;
    private string? _errorMessage;
    private bool _loading;

    public AllocationCopyDialog(AllocationCreateService allocationCreateService)
    {
        _allocationCreateService = allocationCreateService;
    }

    protected override void OnInitialized()
    {
        _sourceYear = SourceYear;
        _sourceMonth = SourceMonth;
        _targetYear = SourceYear > 0 ? SourceYear : DateTime.Today.Year;
        _targetMonth = SourceMonth;

        if (SourceAllocations is not null)
            _items.AddRange(SourceAllocations);
    }

    private Task OnYearChanged(int year)
    {
        _targetYear = year;
        return Task.CompletedTask;
    }

    private Task OnMonthChanged(int? month)
    {
        _targetMonth = month;
        return Task.CompletedTask;
    }

    private void RemoveItem(int allocationId)
    {
        _items.RemoveAll(x => x.Id == allocationId);
    }

    private async Task Submit()
    {
        _errorMessage = null;

        if (_targetMonth is null)
        {
            _errorMessage = "Target month is required";
            return;
        }

        if (_items.Count == 0)
        {
            _errorMessage = "There are no allocations to copy";
            return;
        }

        _loading = true;
        await ErrorRedirectExecutor.Run(async () =>
        {
            foreach (var item in _items)
            {
                var input = new AllocationCreateInput(
                    BudgetId,
                    item.SubCategoryId,
                    _targetMonth.Value,
                    _targetYear,
                    item.Description,
                    item.Amount,
                    item.Type);

                await _allocationCreateService.Handle(input, _requestCts.Token);
            }

            MudDialog.Close(DialogResult.Ok(true));
        }, Nav, () => _loading = false);
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private static string MonthName(int month) =>
        CultureInfo.InvariantCulture.DateTimeFormat.GetMonthName(month);

    public void Dispose()
    {
        _requestCts.Cancel();
        _requestCts.Dispose();
    }
}
