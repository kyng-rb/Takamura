using System.Net;

namespace Takamura.Web.Services.Infrastructure;

public sealed class ApiRequestException : Exception
{
    public ApiRequestException(string resource, HttpStatusCode statusCode, string? responseMessage = null)
        : base($"API request failed for '{resource}' with status {(int)statusCode} ({statusCode}). {responseMessage}".Trim())
    {
        Resource = resource;
        StatusCode = statusCode;
    }

    public string Resource { get; }
    public HttpStatusCode StatusCode { get; }
}
