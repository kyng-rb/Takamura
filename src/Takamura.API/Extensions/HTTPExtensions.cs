using FluentResults;

namespace Takamura.API.Extensions;

public static class HttpExtensions
{
    private const string ValidationTitle = "The request is not valid.";

    public static async Task<IResult> ToHttp(this Task<Result> task)
    {
        var serviceResult = await task;

        if (serviceResult.IsSuccess)
            return Results.NoContent();

        return ValidationProblem(serviceResult.Errors);
    }

    public static async Task<IResult> ToHttp<T>(this Task<Result<T>> task)
    {
        var serviceResult = await task;

        if (serviceResult.IsSuccess)
            return Results.Ok(serviceResult.Value);

        return ValidationProblem(serviceResult.Errors);
    }

    /// <summary><c>Result&lt;T&gt;</c> from a create service: 201 Created with the body (e.g. <c>{ "id": 1 }</c>) or 400.</summary>
    public static async Task<IResult> ToCreated<T>(this Task<Result<T>> task)
    {
        var serviceResult = await task;

        if (serviceResult.IsSuccess)
            return Results.Created((string?)null, serviceResult.Value);

        return ValidationProblem(serviceResult.Errors);
    }

    /// <summary>
    /// 400 as RFC 7807 problem details, the same shape the framework uses for binding errors:
    /// <c>detail</c> holds every message joined, <c>errors</c> the individual messages.
    /// </summary>
    private static IResult ValidationProblem(IEnumerable<IError> errors)
    {
        var messages = errors.Select(e => e.Message).ToArray();

        return Results.Problem(
            title: ValidationTitle,
            detail: string.Join(" ", messages),
            statusCode: StatusCodes.Status400BadRequest,
            extensions: new Dictionary<string, object?> { ["errors"] = messages });
    }
}
