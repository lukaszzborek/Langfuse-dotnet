namespace zborek.Langfuse.Models;

/// <summary>
///     Exception thrown when Langfuse API returns an error response
/// </summary>
public class LangfuseApiException : Exception
{
    /// <summary>
    ///     HTTP status code from the API response
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    ///     Error code from the API response, if available
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    ///     Additional error details from the API response
    /// </summary>
    public IDictionary<string, object>? Details { get; }

    /// <summary>
    ///     Initializes a new instance of the LangfuseApiException class
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="message">Error message</param>
    /// <param name="errorCode">Optional error code</param>
    /// <param name="details">Optional error details</param>
    public LangfuseApiException(int statusCode, string message, string? errorCode = null,
        IDictionary<string, object>? details = null)
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Details = details;
    }

    /// <summary>
    ///     Initializes a new instance of the LangfuseApiException class with an inner exception
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="message">Error message</param>
    /// <param name="innerException">Inner exception</param>
    /// <param name="errorCode">Optional error code</param>
    /// <param name="details">Optional error details</param>
    public LangfuseApiException(int statusCode, string message, Exception innerException, string? errorCode = null,
        IDictionary<string, object>? details = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Details = details;
    }
}