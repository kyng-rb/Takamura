using Takamura.Application.Features.Budget.AttachPeriod;

namespace Takamura.API.Endpoints.Budget.AttachPeriodAllocation;

public record AttachPeriodAllocationRequest(int SubCategoryId, string Description, int Month, int Year, decimal Amount, string Type)
{
    public AttachPeriodAllocationServiceInput ToServiceInput(int budgetId)
        => new(budgetId, Description, SubCategoryId, Month, Year, Amount, Type);
}
