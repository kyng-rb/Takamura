using Takamura.Application.Features.Budget.Retrieve;

namespace Takamura.API.Endpoints.Budget.RetrievePeriodAllocations;

public record RetrievePeriodAllocationRequest(
    int? Year,
    int? Month,
    int? SubCategoryId,
    int? CategoryId,
    string? Type)
{
    public RetrievePeriodAllocationInput ToServiceInput(int budgetId)
        => new(budgetId, Year, Month, SubCategoryId, CategoryId, Type);
}
