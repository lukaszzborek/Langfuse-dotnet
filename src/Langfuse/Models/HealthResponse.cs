using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Response from health check endpoint
/// </summary>
public class HealthResponse
{
    /// <summary>
    ///     Langfuse server version
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    ///     Health status
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}