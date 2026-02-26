using Takamura.Web.Services.Bills.Create;
using Takamura.Web.Services.Bills.Retrieve;
using Takamura.Web.Services.Budget.Retrieve;
using Takamura.Web.Services.Allocation.Create;
using Takamura.Web.Services.Allocation.Retrieve;
using Takamura.Web.Services.Balances.RetrieveSubCategoryBalance;
using Takamura.Web.Services.Category.AddSubCategory;
using Takamura.Web.Services.Category.Create;
using Takamura.Web.Services.Category.Retrieve;

namespace Takamura.Web.Services;

public static class ServicesDependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<BudgetRetrieveService>();
        services.AddScoped<AllocationRetrieveService>();
        services.AddScoped<AllocationCreateService>();
        services.AddScoped<RetrieveSubCategoryBalanceService>();
        services.AddScoped<BillsRetrieveService>();
        services.AddScoped<BillsCreateService>();
        services.AddScoped<CategoryRetrieveService>();
        services.AddScoped<CategoryCreateService>();
        services.AddScoped<CategoryAddSubCategoryService>();
        return services;
    }
}
