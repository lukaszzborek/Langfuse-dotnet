using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Request parameters for deleting multiple traces by their IDs
/// </summary>
public class DeleteTraceManyRequest
{
    /// <summary>
    ///     Array of trace IDs to delete
    /// </summary>
    [JsonPropertyName("traceIds")]
    public required string[] TraceIds { get; set; }
}
