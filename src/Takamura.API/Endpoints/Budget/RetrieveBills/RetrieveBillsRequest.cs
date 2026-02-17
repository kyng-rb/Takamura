using Takamura.Application.Features.Bill.Retrieve;

namespace Takamura.API.Endpoints.Budget.RetrieveBills;

public record RetrieveBillsRequest(
    DateOnly? From,
    DateOnly? To,
    int? CategoryId,
    int? SubCategoryId)
{
    public RetrieveBillsOptions ToServiceInput(int budgetId)
        => new(budgetId, From, To, CategoryId, SubCategoryId);
}
