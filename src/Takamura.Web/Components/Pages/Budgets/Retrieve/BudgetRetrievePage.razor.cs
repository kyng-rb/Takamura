using Microsoft.AspNetCore.Components;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Services.Budget.Retrieve;

namespace Takamura.Web.Components.Pages.Budgets.Retrieve;

public partial class BudgetRetrievePage : IDisposable
{
    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    private readonly BudgetRetrieveService _budgetRetrieveService;
    private readonly CancellationTokenSource _requestCts = new();
    private IReadOnlyList<BudgetRetrieveItem> _budgets = [];
    private bool _loading;

    public BudgetRetrievePage(BudgetRetrieveService budgetRetrieveService)
    {
        _budgetRetrieveService = budgetRetrieveService;
    }

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await ErrorRedirectExecutor.Run(async () =>
        {
            var response = await _budgetRetrieveService.Handle(_requestCts.Token);
            _budgets = response.Budgets ?? [];
        }, Nav, () => _loading = false);
    }

    public void Dispose()
    {
        _requestCts.Cancel();
        _requestCts.Dispose();
    }
}
