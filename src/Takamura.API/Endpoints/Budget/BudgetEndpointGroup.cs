namespace Takamura.API.Endpoints.Budget;

using AttachPeriodAllocation;
using CreateBill;
using CreateBudget;
using RetrieveBills;
using RetrieveBudgets;
using RetrieveMonthlySubCategoryBalance;
using RetrievePeriodAllocations;

public static class BudgetEndpointGroup
{
    private const string Section = "api/budget";

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup(Section);

        RetrieveBudgetsEndpoint.Map(group);
        CreateBudgetEndpoint.Map(group);
        AttachPeriodAllocationEndpoint.Map(group);
        RetrievePeriodAllocationsEndpoint.Map(group);
        RetrieveMonthlySubCategoryBalanceEndpoint.Map(group);
        RetrieveBillsEndpoint.Map(group);
        CreateBillEndpoint.Map(group);
    }
}
