using Microsoft.AspNetCore.Components;
using MudBlazor;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Services.Category.AddSubCategory;

namespace Takamura.Web.Components.Pages.Categories.AddSubCategory;

public partial class CategoryAddSubCategoryDialog : IDisposable
{
    [Parameter]
    public int CategoryId { get; set; }

    [Parameter]
    public string CategoryDescription { get; set; } = string.Empty;

    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    [Inject]
    private CategoryAddSubCategoryService CategoryAddSubCategoryService { get; set; } = default!;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly CancellationTokenSource _requestCts = new();
    private MudForm _form = new();
    private string _description = string.Empty;
    private string _categoryDescription = string.Empty;
    private bool _loading;

    protected override void OnInitialized()
    {
        _categoryDescription = CategoryDescription;
    }

    private async Task Submit()
    {
        await ErrorRedirectExecutor.Run(async () =>
        {
            await _form.Validate();
            if (!_form.IsValid)
                return;

            var description = _description.Trim();
            if (string.IsNullOrWhiteSpace(description))
                return;

            _loading = true;
            await CategoryAddSubCategoryService.Handle(
                new CategoryAddSubCategoryInput(CategoryId, description),
                _requestCts.Token);
            MudDialog.Close(DialogResult.Ok(true));
        }, Nav, () => _loading = false);
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    public void Dispose()
    {
        _requestCts.Cancel();
        _requestCts.Dispose();
    }
}
