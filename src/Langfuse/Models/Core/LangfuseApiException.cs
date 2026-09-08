using System.Text.Json;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Exception thrown when Langfuse API returns an error response
/// </summary>
public class LangfuseApiException : Exception
{
    private static readonly JsonSerializerOptions ErrorParsingOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    ///     HTTP status code from the API response
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    ///     Raw response body from the API
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    ///     Structured error envelope parsed from <see cref="ResponseBody" />, when the API returned the
    ///     standard <see cref="Models.Core.PublicApiError" /> shape. Null when the response body was empty,
    ///     not JSON, or did not match that shape.
    /// </summary>
    public PublicApiError? Error { get; }

    /// <summary>
    ///     Initializes a new instance of the LangfuseApiException class
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="message">Error message</param>
    /// <param name="responseBody">Raw response body</param>
    public LangfuseApiException(int statusCode, string message, string? responseBody = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        Error = TryParseError(responseBody);
    }

    /// <summary>
    ///     Initializes a new instance of the LangfuseApiException class with an inner exception
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    public LangfuseApiException(int statusCode, string message, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    ///     Attempts to parse the given response body as a <see cref="PublicApiError" /> envelope.
    ///     Best-effort only: any failure (empty body, non-JSON body, or a JSON shape that does not
    ///     match the envelope) is swallowed and null is returned.
    /// </summary>
    /// <param name="responseBody">Raw response body to parse</param>
    /// <returns>The parsed error envelope, or null if it could not be parsed</returns>
    private static PublicApiError? TryParseError(string? responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<PublicApiError>(responseBody, ErrorParsingOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}