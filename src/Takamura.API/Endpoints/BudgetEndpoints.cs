using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Bill.Create;
using Takamura.Application.Features.Bill.Retrieve;
using Takamura.Application.Features.Budget.AttachPeriod;
using Takamura.Application.Features.Budget.Create;
using Takamura.Application.Features.Budget.Retrieve;
using Takamura.Application.Features.Budget.Summary;

namespace Takamura.API.Endpoints;

public static class BudgetEndpoints
{
    private const string Section = "api/budget";

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup(Section);

        group.MapGet("/", async ([FromServices] RetrieveBudgetsService service)
            => await service.Handle().ToHttp());

        group.MapPost("/", async ([FromBody] CreateBudgetServiceInput input, [FromServices] CreateBudgetService service)
            => await service.Handle(input).ToHttp());

        group.MapGet("/{id:int}/subcategorybalance", async ([FromRoute] int id, [FromQuery] int year, [FromQuery] int? month, [FromServices] RetrieveMonthlySubCategoryBalanceService service)
            => await service.Handle(new RetrieveMonthlySubCategoryBalanceServiceInput(id, year, month)).ToHttp());

        group.MapGet("/{id:int}/categorybalance", async ([FromRoute] int id, [FromQuery] int year, [FromQuery] int? month, [FromServices] RetrieveMonthlyCategoryBalanceService service)
            => await service.Handle(new RetrieveMonthlyCategoryBalanceServiceInput(id, year, month)).ToHttp());

        group.MapGet("/{id:int}/report", async ([FromRoute] int id, [FromQuery] int year, [FromServices] RetrieveMonthlyCategoryBalanceReportService service)
            => await service.Handle(new RetrieveMonthlyCategoryBalanceReportServiceInput(id, year)).ToHttp());

        group.MapPost("/{id}/allocation", async ([FromRoute] int id, [FromBody] AttachPeriodAllocationRequest input, [FromServices] AttachPeriodAllocationService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp());

        group.MapGet("/{id}/allocation", async ([FromRoute] int id, [AsParameters] RetrievePeriodAllocationRequest input, [FromServices] RetrievePeriodAllocationsService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp());

        group.MapGet("/{id:int}/bill", async ([FromRoute] int id, [AsParameters] RetrieveBillsRequest input, [FromServices] RetrieveBillsService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp());

        group.MapPost("/{id}/bill", async ([FromRoute] int id, [FromBody] CreateBillRequest input, [FromServices] CreateBillService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp());
    }

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

    public record CreateBillRequest(DateTime Date, decimal Amount, string Description, int SubCategoryId)
    {
        public CreateBillServiceInput ToServiceInput(int budgetId)
                  => new(DateOnly.FromDateTime(Date), Amount, Description, budgetId, SubCategoryId);
    }

    public record RetrieveBillsRequest(
        DateOnly? From,
        DateOnly? To,
        int? CategoryId,
        int? SubCategoryId)
    {
        public RetrieveBillsOptions ToServiceInput(int budgetId)
            => new(budgetId, From, To, CategoryId, SubCategoryId);
    }

    public record AttachPeriodAllocationRequest(int SubCategoryId, int Month, int Year, string Description, decimal Amount, string Type)
    {
        public AttachPeriodAllocationServiceInput ToServiceInput(int budgetId)
            => new(budgetId, SubCategoryId, Month, Year, Description, Amount, Type);
    }
}