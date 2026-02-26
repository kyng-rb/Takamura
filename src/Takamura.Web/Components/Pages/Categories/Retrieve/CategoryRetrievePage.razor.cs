using Microsoft.AspNetCore.Components;
using MudBlazor;
using Takamura.Web.Components.Pages.Categories.AddSubCategory;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Components.Pages.Categories.Create;
using Takamura.Web.Services.Category.Retrieve;

namespace Takamura.Web.Components.Pages.Categories.Retrieve;

public partial class CategoryRetrievePage : IDisposable
{
    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    [Inject]
    private CategoryRetrieveService CategoryRetrieveService { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private readonly CancellationTokenSource _requestCts = new();
    private IReadOnlyList<CategoryItem> _categories = [];
    private bool _loading;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        _loading = true;
        await ErrorRedirectExecutor.Run(async () =>
        {
            var output = await CategoryRetrieveService.Handle(_requestCts.Token);
            _categories = output.Categories ?? [];
        }, Nav, () => _loading = false);
    }

    private async Task OpenCreateCategoryDialog()
    {
        var dialog = await DialogService.ShowAsync<CategoryCreateDialog>(
            "Create Category",
            new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true
            });

        var result = await dialog.Result;
        if (result is { Canceled: false })
            await LoadData();
    }

    private async Task OpenAddSubCategoryDialog(int categoryId, string categoryDescription)
    {
        var dialog = await DialogService.ShowAsync<CategoryAddSubCategoryDialog>(
            "Add Subcategory",
            new DialogParameters
            {
                { nameof(CategoryAddSubCategoryDialog.CategoryId), categoryId },
                { nameof(CategoryAddSubCategoryDialog.CategoryDescription), categoryDescription }
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

    public void Dispose()
    {
        _requestCts.Cancel();
        _requestCts.Dispose();
    }
}
