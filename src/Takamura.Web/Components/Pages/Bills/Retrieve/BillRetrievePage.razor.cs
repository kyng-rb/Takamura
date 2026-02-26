using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Components.Pages.Bills.Create;
using Takamura.Web.Services.Bills.Retrieve;

namespace Takamura.Web.Components.Pages.Bills.Retrieve;

public partial class BillRetrievePage : IDisposable
{
    [Parameter]
    public int BudgetId { get; set; }

    [Inject]
    private NavigationManager Nav { get; set; } = default!;
    
    private readonly BillsRetrieveService _billsRetrieveService;
    private readonly IDialogService _dialogService;
    private readonly CancellationTokenSource _requestCts = new();
    private readonly CultureInfo _usdCulture = CultureInfo.GetCultureInfo("en-US");
    private IReadOnlyList<BillsRetrieveItem> _bills = [];
    private bool _loading;
    private string _search = string.Empty;

    public BillRetrievePage(BillsRetrieveService billsRetrieveService, IDialogService dialogService)
    {
        _billsRetrieveService = billsRetrieveService;
        _dialogService = dialogService;
    }

    protected override async Task OnInitializedAsync()
    {
        await LoadBills();
    }

    private bool QuickFilter(BillsRetrieveItem item)
    {
        if (string.IsNullOrWhiteSpace(_search))
            return true;

        var search = _search.Trim();

        return item.Category.Contains(search, StringComparison.OrdinalIgnoreCase)
               || item.SubCategory.Contains(search, StringComparison.OrdinalIgnoreCase)
               || item.Description.Contains(search, StringComparison.OrdinalIgnoreCase)
               || item.Amount.ToString(CultureInfo.InvariantCulture).Contains(search, StringComparison.OrdinalIgnoreCase)
               || item.Amount.ToString("C1", _usdCulture).Contains(search, StringComparison.OrdinalIgnoreCase)
               || item.Date.ToString("yyyy/MM/dd").Contains(search, StringComparison.OrdinalIgnoreCase)
               || item.Date.ToString("MM/dd/yyyy").Contains(search, StringComparison.OrdinalIgnoreCase)
               || item.Date.ToString("dd/MM/yyyy").Contains(search, StringComparison.OrdinalIgnoreCase)
               || item.Date.ToString("MMMM yyyy", CultureInfo.InvariantCulture).Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    private async Task OpenCreateBillDialog()
    {
        var dialog = await _dialogService.ShowAsync<BillCreatePage>(
            "Create Bill",
            new DialogParameters
            {
                { nameof(BillCreatePage.BudgetId), BudgetId }
            },
            new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            });

        var result = await dialog.Result;
        if (result is { Canceled: false })
            await LoadBills();
    }

    private async Task LoadBills()
    {
        _loading = true;
        await ErrorRedirectExecutor.Run(async () =>
        {
            var serviceOutput = await _billsRetrieveService.Handle(BudgetId, cancellationToken: _requestCts.Token);
            _bills = serviceOutput.Bills ?? [];
        }, Nav, () => _loading = false);
    }

    public void Dispose()
    {
        _requestCts.Cancel();
        _requestCts.Dispose();
    }
}
