using Takamura.Application.Features.Bill.Create;

namespace Takamura.API.Endpoints.Budget.CreateBill;

public record CreateBillRequest(DateOnly Date, decimal Amount, string Description, int SubCategoryId)
{
    public CreateBillServiceInput ToServiceInput(int budgetId)
        => new(Date, Amount, Description, budgetId, SubCategoryId);
}
