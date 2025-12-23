using Microsoft.AspNetCore.Mvc;
using Takamura.API.Extensions;
using Takamura.Application.Features.Bill.Create;
using Takamura.Application.Features.Bill.Retrieve;
using Takamura.Application.Features.Budget.AttachPeriod;
using Takamura.Application.Features.Budget.Create;
using Takamura.Application.Features.Budget.Retrieve;

namespace Takamura.API.Endpoints;

public static class BudgetEndpoints
{
    private const string Section = "api/budget";

    public static void Map(WebApplication app)
    {
        var group = app.MapGroup(Section);

        group.MapGet("/", async ([FromServices] RetrieveBudgetsService service)
            => await service.Handle().ToHttp())
            .Produces<RetrieveBudgetsOutput>();

        group.MapPost("/", async ([FromBody] CreateBudgetServiceInput input, [FromServices] CreateBudgetService service)
            => await service.Handle(input).ToHttp());

        group.MapPost("/{id}/allocation", async ([FromRoute] int id, [FromBody] AttachPeriodAllocationRequest input, [FromServices] AttachPeriodAllocationService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp());

        group.MapGet("/{id}/allocation", async ([FromRoute] int id, [AsParameters] RetrievePeriodAllocationRequest input, [FromServices] RetrievePeriodAllocationsService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp())
            .Produces<RetrievePeriodAllocationsOutput>();

        group.MapGet("/{id:int}/bill", async ([FromRoute] int id, [AsParameters] RetrieveBillsRequest input, [FromServices] RetrieveBillsService service)
            => await service.Handle(input.ToServiceInput(id)).ToHttp())
            .Produces<RetrieveBillsOutput>();

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

    public record CreateBillRequest(DateOnly Date, decimal Amount, string Description, int SubCategoryId)
    {
        public CreateBillServiceInput ToServiceInput(int budgetId)
                  => new(Date, Amount, Description, budgetId, SubCategoryId);
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

    public record AttachPeriodAllocationRequest(int SubCategoryId, int Month, int Year, decimal Amount, string Type)
    {
        public AttachPeriodAllocationServiceInput ToServiceInput(int budgetId)
            => new(budgetId, SubCategoryId, Month, Year, Amount, Type);
    }
}