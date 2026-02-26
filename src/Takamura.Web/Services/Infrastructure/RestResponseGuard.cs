using RestSharp;

namespace Takamura.Web.Services.Infrastructure;

public static class RestResponseGuard
{
    public static T EnsureSuccessAndData<T>(RestResponse<T> response, string resource)
    {
        EnsureSuccess(response, resource);

        if (response.Data is null)
            throw new ApiRequestException(resource, response.StatusCode, "Response body was empty.");

        return response.Data;
    }

    public static void EnsureSuccess(RestResponse response, string resource)
    {
        var statusCode = (int)response.StatusCode;
        var is2xx = statusCode is >= 200 and < 300;

        if (!response.IsSuccessful || !is2xx)
            throw new ApiRequestException(resource, response.StatusCode, response.ErrorMessage);
    }
}
