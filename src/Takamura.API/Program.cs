using Takamura.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();
var app = builder.ConfigureAPI();

app.Run();