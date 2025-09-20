using Microsoft.Extensions.DependencyInjection;
using Takamura.Application.Features.Bill.Create;
using Takamura.Application.Features.Bill.Retrieve;
using Takamura.Application.Features.Budget.Create;
using Takamura.Application.Features.Category.Create;
using Takamura.Application.Features.SubCategory.Create;

namespace Takamura.Application;

public static class RegisterServices
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<CreateBillService>();
        services.AddScoped<RetrieveBillsService>();
        services.AddScoped<CreateBudgetService>();
        services.AddScoped<CreateCategoryService>();
        services.AddScoped<CreateSubCategoryService>();
        return services;
    }
}