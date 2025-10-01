using FluentResults;

namespace Takamura.API.Extensions;

public static class HTTPExtensions
{
    public static async Task<IResult> ToHttp<T>(this Task<Result<T>> task)
    {
        var serviceResult = await task;

        if (serviceResult.IsSuccess)
        {
            return Results.Ok(serviceResult.Value);
        }

        return Results.BadRequest(serviceResult.Errors[0].Message);
    }
}