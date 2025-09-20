using Takamura.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServices();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapGet("/", () => "App is running");

app.Run();