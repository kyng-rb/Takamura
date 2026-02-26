using Microsoft.AspNetCore.Components;

namespace Takamura.Web.Components.Infrastructure;

public static class ErrorRedirectExecutor
{
    public static async Task Run(Func<Task> action, NavigationManager navigationManager, Action? onFinally = null)
    {
        try
        {
            await action();
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            var message = Uri.EscapeDataString(ex.Message);
            var details = Uri.EscapeDataString(ex.ToString());
            navigationManager.NavigateTo($"/Error?message={message}&details={details}", forceLoad: true);
        }
        finally
        {
            onFinally?.Invoke();
        }
    }
}
