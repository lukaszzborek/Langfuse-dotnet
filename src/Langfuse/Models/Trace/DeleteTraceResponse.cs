using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Response model for trace deletion operations
/// </summary>
public class DeleteTraceResponse
{
    /// <summary>
    ///     Status message about the deletion operation
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}