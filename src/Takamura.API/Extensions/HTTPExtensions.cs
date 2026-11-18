using FluentResults;

namespace Takamura.API.Extensions;

public static class HttpExtensions
{
    public static async Task<IResult> ToHttp(this Task<Result> task)
    {
        var serviceResult = await task;

        if (serviceResult.IsSuccess)
            return Results.NoContent();

        return Results.BadRequest(ErrorMessage(serviceResult.Errors));
    }

    private static string ErrorMessage(IEnumerable<IError> errors)
        => string.Join(" ", errors.Select(e => e.Message));

    public static async Task<IResult> ToHttp<T>(this Task<Result<T>> task)
    {
        var serviceResult = await task;

        if (serviceResult.IsSuccess)
            return Results.Ok(serviceResult.Value);

        return Results.BadRequest(ErrorMessage(serviceResult.Errors));
    }

    /// <summary><c>Result&lt;T&gt;</c> from a create service: 201 Created with the body (e.g. <c>{ "id": 1 }</c>) or 400.</summary>
    public static async Task<IResult> ToCreated<T>(this Task<Result<T>> task)
    {
        var serviceResult = await task;

        if (serviceResult.IsSuccess)
            return Results.Created((string?)null, serviceResult.Value);

        return Results.BadRequest(ErrorMessage(serviceResult.Errors));
    }
}
