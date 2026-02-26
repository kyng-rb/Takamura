using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Services.Allocation.Create;
using Takamura.Web.Services.Category.Retrieve;

namespace Takamura.Web.Components.Pages.Allocations.Create;

public class AllocationCreateModel
{
    public int? SubCategoryId { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal? Amount { get; set; }
    public string Type { get; set; } = string.Empty;

    public AllocationCreateInput ToInput(int budgetId)
    {
        return new AllocationCreateInput(
            budgetId,
            SubCategoryId!.Value,
            Month!.Value,
            Year!.Value,
            Description,
            Amount!.Value,
            Type);
    }
}

public partial class AllocationCreateDialog : IDisposable
{
    [Parameter]
    public int BudgetId { get; set; }

    [Parameter]
    public int Year { get; set; }

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    private readonly AllocationCreateService _allocationCreateService;
    private readonly CategoryRetrieveService _categoryRetrieveService;
    private readonly Func<decimal?, string?> _amountValidation = ValidateAmount;
    private readonly AllocationCreateModel _model = new();
    private readonly CultureInfo _usdCulture = CultureInfo.GetCultureInfo("en-US");
    private readonly CancellationTokenSource _requestCts = new();
    private readonly IReadOnlyList<int> _availableYears =
        Enumerable.Range(DateTime.Today.Year - 10, 12).Reverse().ToArray();
    private readonly (int Month, string Label)[] _months = Enumerable.Range(1, 12)
        .Select(month => (month, new DateTime(2000, month, 1).ToString("MMMM", CultureInfo.InvariantCulture)))
        .ToArray();

    private IReadOnlyCollection<CategoryItem> _categories = [];
    private IReadOnlyCollection<SubCategoryRetrieveItem> _subCategories = [];
    private int? _selectedCategoryId;
    private MudForm _form = new();
    private bool _loading;

    public AllocationCreateDialog(
        CategoryRetrieveService categoryRetrieveService,
        AllocationCreateService allocationCreateService)
    {
        _categoryRetrieveService = categoryRetrieveService;
        _allocationCreateService = allocationCreateService;
    }

    protected override async Task OnInitializedAsync()
    {
        _model.Year = Year > 0 ? Year : DateTime.Today.Year;
        _model.Month = DateTime.Today.Month;
        _model.Type = "Outcome";

        _loading = true;
        await ErrorRedirectExecutor.Run(async () =>
        {
            var categoryOutput = await _categoryRetrieveService.Handle(_requestCts.Token);
            _categories = categoryOutput.Categories ?? [];
        }, Nav, () => _loading = false);
    }

    private Task OnYearChanged(int year)
    {
        _model.Year = year;
        return Task.CompletedTask;
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

            var input = _model.ToInput(BudgetId);
            await _allocationCreateService.Handle(input, _requestCts.Token);
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
