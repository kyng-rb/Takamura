using System.Net;
using System.Text.Json;
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

        if (response.IsSuccessful && is2xx)
            return;

        // No HTTP response at all (API down, DNS, timeout...).
        if (response.StatusCode == 0)
            throw new ApiRequestException(resource, response.StatusCode,
                response.ErrorMessage ?? response.ErrorException?.Message ?? "The API could not be reached.");

        var apiMessage = ReadApiMessage(response.Content);

        if (response.StatusCode == HttpStatusCode.BadRequest)
            throw new ApiValidationException(resource, apiMessage ?? "The request is not valid.");

        throw new ApiRequestException(resource, response.StatusCode, apiMessage ?? response.ErrorMessage);
    }

    /// <summary>
    /// Extracts a readable message from the API body. Supports the plain JSON string returned by
    /// <c>Results.BadRequest(string)</c> and RFC 7807 problem details (<c>detail</c>, <c>title</c>, <c>errors</c>).
    /// </summary>
    private static string? ReadApiMessage(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return null;

        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;

            switch (root.ValueKind)
            {
                case JsonValueKind.String:
                    return root.GetString();

                case JsonValueKind.Object:
                {
                    var messages = new List<string>();

                    if (TryGetString(root, "detail", out var detail))
                        messages.Add(detail);
                    else if (TryGetString(root, "title", out var title))
                        messages.Add(title);

                    if (TryGetProperty(root, "errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var field in errors.EnumerateObject())
                        {
                            if (field.Value.ValueKind != JsonValueKind.Array)
                                continue;

                            messages.AddRange(field.Value.EnumerateArray()
                                .Where(x => x.ValueKind == JsonValueKind.String)
                                .Select(x => x.GetString()!));
                        }
                    }

                    return messages.Count > 0 ? string.Join(" ", messages) : null;
                }

                default:
                    return content.Trim();
            }
        }
        catch (JsonException)
        {
            return content.Trim();
        }
    }

    private static bool TryGetString(JsonElement element, string name, out string value)
    {
        value = string.Empty;
        if (!TryGetProperty(element, name, out var property) || property.ValueKind != JsonValueKind.String)
            return false;

        value = property.GetString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(value);
    }

    private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }
}
