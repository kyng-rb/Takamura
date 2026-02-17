using FluentResults;

namespace Takamura.API.Extensions;

public static class HttpExtensions
{
    public static async Task<IResult> ToHttp(this Task<Result> task)
    {
        var serviceResult = await task;

        if (serviceResult.IsSuccess)
            return Results.NoContent();

        return Results.BadRequest(serviceResult.Errors[0].Message);
    }

    public static async Task<IResult> ToHttp<T>(this Task<Result<T>> task)
    {
        var serviceResult = await task;

        if (serviceResult.IsSuccess)
            return Results.Ok(serviceResult.Value);

        return Results.BadRequest(serviceResult.Errors[0].Message);
    }
}
