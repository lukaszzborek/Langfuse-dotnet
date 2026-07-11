using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Experiment;

/// <summary>
///     Response containing experiment items with cursor-based pagination.
/// </summary>
public class ExperimentItemsResponse
{
    /// <summary>
    ///     Experiment items in the current page.
    /// </summary>
    [JsonPropertyName("data")]
    public required ExperimentItem[] Data { get; init; }

    /// <summary>
    ///     Metadata for cursor-based pagination.
    /// </summary>
    [JsonPropertyName("meta")]
    public required ExperimentsResponseMeta Meta { get; init; }
}