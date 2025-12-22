namespace zborek.Langfuse.Models.Core;

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
    ///     Raw response body from the API
    /// </summary>
    public string? ResponseBody { get; }

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
}