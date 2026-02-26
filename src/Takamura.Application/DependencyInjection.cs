using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Takamura.Application.Database;
using Takamura.Application.Features.Bill.Create;
using Takamura.Application.Features.Bill.Retrieve;
using Takamura.Application.Features.Budget.AttachPeriod;
using Takamura.Application.Features.Budget.Create;
using Takamura.Application.Features.Budget.Retrieve;
using Takamura.Application.Features.Budget.RetrieveMonthlySubCategoryBalance;
using Takamura.Application.Features.Category.AttachSubCategory;
using Takamura.Application.Features.Category.Create;
using Takamura.Application.Features.Category.Retrieve;

namespace Takamura.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CreateCategoryService>();
        services.AddScoped<RetrieveCategoriesService>();
        services.AddScoped<AttachSubCategoryService>();

        services.AddScoped<CreateBudgetService>();
        services.AddScoped<RetrieveBudgetsService>();
        services.AddScoped<AttachPeriodAllocationService>();
        services.AddScoped<RetrievePeriodAllocationsService>();
        services.AddScoped<RetrieveMonthlySubCategoryBalanceService>();

        services.AddScoped<CreateBillService>();
        services.AddScoped<RetrieveBillsService>();
        return services;
    }

    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, string connectionString, bool isDevelopment)
    {
        services.AddDbContext<DatabaseContext>(
            options =>
            {
                options.UseSqlServer(connectionString,
                    sqlOptions => sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null));

                if (isDevelopment)
                {
                    options.EnableSensitiveDataLogging();
                    options.EnableDetailedErrors();
                    options.LogTo(Console.WriteLine, LogLevel.Information);
                }
            });

        return services;
    }
}
