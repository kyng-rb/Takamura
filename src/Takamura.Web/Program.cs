using MudBlazor.Services;
using Takamura.Web.Components;
using Takamura.Web.Services;
using Takamura.Web.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.AddApplicationServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler("/Error", true);

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();