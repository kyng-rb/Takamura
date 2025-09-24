using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Takamura.Application.Database;
using Takamura.Application.Features.Bill.Create;
using Takamura.Application.Features.Bill.Retrieve;
using Takamura.Application.Features.Budget.Create;
using Takamura.Application.Features.Category.AttachSubCategory;
using Takamura.Application.Features.Category.Create;
using Takamura.Application.Features.Category.Retrieve;

namespace Takamura.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<CreateCategoryService>();
        services.AddScoped<RetrieveCategoriesService>();
        services.AddScoped<AttachSubCategoryService>();

        services.AddScoped<CreateBillService>();
        services.AddScoped<RetrieveBillsService>();
        services.AddScoped<CreateBudgetService>();
        return services;
    }

    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DatabaseContext>(
            options => options.UseSqlServer(connectionString,
                sqlOptions => sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)));

        return services;
    }
}