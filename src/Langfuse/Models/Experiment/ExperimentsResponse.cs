using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Experiment;

/// <summary>
///     Response containing experiments with cursor-based pagination.
/// </summary>
public class ExperimentsResponse
{
    /// <summary>
    ///     Experiments in the current page.
    /// </summary>
    [JsonPropertyName("data")]
    public required Experiment[] Data { get; init; }

    /// <summary>
    ///     Metadata for cursor-based pagination.
    /// </summary>
    [JsonPropertyName("meta")]
    public required ExperimentsResponseMeta Meta { get; init; }
}