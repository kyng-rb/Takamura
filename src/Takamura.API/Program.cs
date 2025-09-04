var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.UseHttpsRedirection();

app.MapGet("/", () => "App is running");

app.Run();