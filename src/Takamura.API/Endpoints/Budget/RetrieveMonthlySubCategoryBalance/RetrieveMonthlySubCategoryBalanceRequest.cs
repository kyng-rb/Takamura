using Takamura.Application.Features.Budget.RetrieveMonthlySubCategoryBalance;

namespace Takamura.API.Endpoints.Budget.RetrieveMonthlySubCategoryBalance;

public record RetrieveMonthlySubCategoryBalanceRequest(int Year, int? Month)
{
    public RetrieveMonthlySubCategoryBalanceInput ToServiceInput(int budgetId)
        => new(budgetId, Year, Month);
}
