using System.Net;

namespace Takamura.Web.Services.Infrastructure;

/// <summary>Thrown when the API returns a non-success status code.</summary>
public class ApiRequestException : Exception
{
    public ApiRequestException(string resource, HttpStatusCode statusCode, string? responseMessage = null)
        : base(BuildMessage(resource, statusCode, responseMessage))
    {
        Resource = resource;
        StatusCode = statusCode;
        ResponseMessage = responseMessage;
    }

    public string Resource { get; }
    public HttpStatusCode StatusCode { get; }

    /// <summary>The message returned by the API (e.g. the validation error), if any.</summary>
    public string? ResponseMessage { get; }

    private static string BuildMessage(string resource, HttpStatusCode statusCode, string? responseMessage)
        => $"API request failed for '{resource}' with status {(int)statusCode} ({statusCode}). {responseMessage}".Trim();
}

/// <summary>
/// Thrown when the API rejects the request with 400 Bad Request (business/validation rule).
/// <see cref="Exception.Message"/> is the user-facing message returned by the API.
/// </summary>
public sealed class ApiValidationException : ApiRequestException
{
    public ApiValidationException(string resource, string validationMessage)
        : base(resource, HttpStatusCode.BadRequest, validationMessage)
    {
        ValidationMessage = validationMessage;
    }

    public string ValidationMessage { get; }

    public override string Message => ValidationMessage;
}
