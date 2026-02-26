using Microsoft.AspNetCore.Components;
using MudBlazor;
using Takamura.Web.Components.Infrastructure;
using Takamura.Web.Services.Category.Create;

namespace Takamura.Web.Components.Pages.Categories.Create;

public class CategoryCreateDialogModel
{
    public sealed class SubCategoryModel
    {
        public string Description { get; set; } = string.Empty;
    }

    public string Description { get; set; } = string.Empty;
    public List<SubCategoryModel> SubCategories { get; } =
    [
        new()
    ];
}

public partial class CategoryCreateDialog : IDisposable
{
    [Inject]
    private NavigationManager Nav { get; set; } = default!;

    [Inject]
    private CategoryCreateService CategoryCreateService { get; set; } = default!;

    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    private readonly CancellationTokenSource _requestCts = new();
    private readonly CategoryCreateDialogModel _model = new();
    private MudForm _form = new();
    private bool _loading;

    private void AddSubCategory()
    {
        _model.SubCategories.Add(new CategoryCreateDialogModel.SubCategoryModel());
    }

    private void RemoveSubCategory(int index)
    {
        if (_model.SubCategories.Count == 1)
            return;

        _model.SubCategories.RemoveAt(index);
    }

    private async Task Submit()
    {
        await ErrorRedirectExecutor.Run(async () =>
        {
            await _form.Validate();
            if (!_form.IsValid)
                return;

            var description = _model.Description.Trim();
            var subCategories = _model.SubCategories
                .Select(x => x.Description.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.InvariantCultureIgnoreCase)
                .Select(x => new CategoryCreateSubCategoryInput(x))
                .ToArray();

            if (string.IsNullOrWhiteSpace(description) || subCategories.Length == 0)
                return;

            _loading = true;
            await CategoryCreateService.Handle(
                new CategoryCreateInput(description, subCategories),
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
