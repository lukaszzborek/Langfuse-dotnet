using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Standard error response for the stable evaluators and evaluation-rules API.
/// </summary>
public class PublicApiError
{
    /// <summary>
    ///     Human-readable description of the error.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; set; }

    /// <summary>
    ///     Machine-readable error code.
    /// </summary>
    [JsonPropertyName("code")]
    public required PublicApiErrorCode Code { get; set; }

    /// <summary>
    ///     Additional, context-specific details about the error.
    /// </summary>
    [JsonPropertyName("details")]
    public PublicApiErrorDetails? Details { get; set; }
}