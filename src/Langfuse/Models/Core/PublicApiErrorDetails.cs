using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Additional, context-specific details attached to a <see cref="PublicApiError" />.
/// </summary>
public class PublicApiErrorDetails
{
    /// <summary>
    ///     Validation issues that caused the request to be rejected.
    /// </summary>
    [JsonPropertyName("issues")]
    public List<PublicApiValidationIssue>? Issues { get; set; }

    /// <summary>
    ///     The maximum number of requests allowed within the current rate-limit window.
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    /// <summary>
    ///     The number of requests remaining within the current rate-limit window.
    /// </summary>
    [JsonPropertyName("remaining")]
    public int? Remaining { get; set; }

    /// <summary>
    ///     The date and time at which the current rate-limit window resets.
    /// </summary>
    [JsonPropertyName("resetAt")]
    public DateTimeOffset? ResetAt { get; set; }

    /// <summary>
    ///     The number of seconds the client should wait before retrying the request.
    /// </summary>
    [JsonPropertyName("retryAfterSeconds")]
    public int? RetryAfterSeconds { get; set; }
}