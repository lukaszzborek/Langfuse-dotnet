using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Response from the Langfuse health check endpoint, providing system status and version information for monitoring and diagnostics.
/// </summary>
public class HealthResponse
{
    /// <summary>
    ///     Version of the Langfuse server, useful for compatibility checking and debugging.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>
    ///     Current health status of the Langfuse service, indicating system availability and operational state.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}