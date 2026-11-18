using Takamura.Application.Features.Budget.Create;

namespace Takamura.API.Endpoints.Budget.CreateBudget;

public record CreateBudgetAllocationRequest(
    string Description,
    int SubCategoryId,
    int MonthFrom,
    int YearFrom,
    decimal Amount,
    string Type)
{
    public CreateBudgetPeriodAllocationInput ToServiceInput()
        => new(Description, SubCategoryId, MonthFrom, YearFrom, Amount, Type);
}

public record CreateBudgetRequest(string Title, CreateBudgetAllocationRequest[]? Allocations)
{
    public CreateBudgetServiceInput ToServiceInput()
        => new(Title, Allocations?.Select(x => x.ToServiceInput()).ToArray() ?? []);
}
