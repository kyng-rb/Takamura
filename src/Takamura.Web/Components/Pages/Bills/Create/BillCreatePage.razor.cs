using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Globalization;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Services.Bills.Create;
using Takamura.Web.Services.Category.Retrieve;

namespace Takamura.Web.Components.Pages.Bills.Create;

public class BillCreateInput
{
    public DateTime? Date { get; set; } = DateTime.Today;

    public int? SubCategoryId { get; set; }

    public string Description { get; set; } = string.Empty;

    public decimal? Amount { get; set; }


    public BillsCreateServiceInput ToServiceInput(int budgetId)
    {
        return new BillsCreateServiceInput(budgetId, DateOnly.FromDateTime(Date!.Value), Description, Amount!.Value,
            SubCategoryId!.Value);
    }
}

public partial class BillCreatePage : IDisposable
{
    [Parameter]
    public int BudgetId { get; set; }

    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;
    
    private readonly BillsCreateService _billsCreateService;
    private readonly CategoryRetrieveService _categoryRetrieveService;
    private readonly Func<decimal?, string?> _amountValidation = ValidateAmount;
    private readonly BillCreateInput _model = new();
    private readonly CultureInfo _usdCulture = CultureInfo.GetCultureInfo("en-US");
    private readonly CancellationTokenSource _requestCts = new();
    private IReadOnlyCollection<CategoryItem> _categories = [];
    private IReadOnlyCollection<SubCategoryRetrieveItem> _subCategories = [];
    private int? _selectedCategoryId;
    private MudForm _form = new();
    private bool _loading;

    public BillCreatePage(
        CategoryRetrieveService categoryRetrieveService,
        BillsCreateService billsCreateService)
    {
        _categoryRetrieveService = categoryRetrieveService;
        _billsCreateService = billsCreateService;
    }

    protected override async Task OnInitializedAsync()
    {
        _loading = true;
        await ErrorRedirectExecutor.Run(async () =>
        {
            var categoryOutput = await _categoryRetrieveService.Handle(_requestCts.Token);
            _categories = categoryOutput.Categories ?? [];
        }, Nav, () => _loading = false);
    }

    private void OnCategoryChanged(int? categoryId)
    {
        _selectedCategoryId = categoryId;
        _model.SubCategoryId = null;
        _subCategories = _categories
            .FirstOrDefault(category => category.Id == categoryId)?
            .SubCategories ?? [];
    }

    private async Task Submit()
    {
        await ErrorRedirectExecutor.Run(async () =>
        {
            await _form.Validate();
            if (!_form.IsValid)
                return;

            var input = _model.ToServiceInput(BudgetId);
            await _billsCreateService.Handle(input, _requestCts.Token);
            MudDialog.Close(DialogResult.Ok(true));
        }, Nav);
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private void OnAmountChanged(decimal? amount)
    {
        _model.Amount = amount is null ? null : Math.Round(amount.Value, 1, MidpointRounding.AwayFromZero);
    }

    private static string? ValidateAmount(decimal? amount)
    {
        if (amount is null)
            return "Amount is required";

        var scaled = amount.Value * 10m;
        return scaled != decimal.Truncate(scaled) ? "Only one decimal place is allowed" : null;
    }

    public void Dispose()
    {
        _requestCts.Cancel();
        _requestCts.Dispose();
    }
}
