using Microsoft.AspNetCore.Components;
using Takamura.Web.Services.Infrastructure;

namespace Takamura.Web.Components.Infrastructure;

public static class ErrorRedirectExecutor
{
    // Keeps the redirect URL well below Kestrel's 8 KB request-line limit.
    private const int MaxDetailsLength = 2000;

    /// <summary>
    /// Runs <paramref name="action"/>. API validation errors (400) are passed to
    /// <paramref name="onValidationError"/> when provided, so the caller can show them inline;
    /// any other error navigates to the Error page.
    /// </summary>
    public static async Task Run(
        Func<Task> action,
        NavigationManager navigationManager,
        Action? onFinally = null,
        Action<string>? onValidationError = null)
    {
        try
        {
            await action();
        }
        catch (OperationCanceledException)
        {
        }
        catch (ApiValidationException ex) when (onValidationError is not null)
        {
            onValidationError(ex.ValidationMessage);
        }
        catch (Exception ex)
        {
            var message = Uri.EscapeDataString(ex.Message);
            var fullDetails = ex.ToString();
            var details = Uri.EscapeDataString(fullDetails.Length > MaxDetailsLength
                ? fullDetails[..MaxDetailsLength] + "…"
                : fullDetails);
            navigationManager.NavigateTo($"/Error?message={message}&details={details}", forceLoad: true);
        }
        finally
        {
            onFinally?.Invoke();
        }
    }
}
