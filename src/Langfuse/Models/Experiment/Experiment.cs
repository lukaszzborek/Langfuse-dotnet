using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Models.Experiment;

/// <summary>
///     An experiment aggregating experiment items within the requested time range.
/// </summary>
public class Experiment
{
    /// <summary>
    ///     Experiment identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Experiment name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    ///     Experiment description, or null when not set.
    /// </summary>
    [JsonPropertyName("description")]
    public required string? Description { get; init; }

    /// <summary>
    ///     Start of the experiment, i.e. the earliest event within the requested time range.
    ///     Clipped to <c>fromStartTime</c> when the experiment started before the requested range.
    /// </summary>
    [JsonPropertyName("startTime")]
    public required DateTime StartTime { get; init; }

    /// <summary>
    ///     End of the experiment, i.e. the latest event end within the requested time range.
    /// </summary>
    [JsonPropertyName("endTime")]
    public required DateTime EndTime { get; init; }

    /// <summary>
    ///     Number of experiment items within the requested time range.
    /// </summary>
    [JsonPropertyName("itemCount")]
    public required int ItemCount { get; init; }

    /// <summary>
    ///     Dataset identifier. Null when the experiment is not associated with a dataset.
    /// </summary>
    [JsonPropertyName("datasetId")]
    public required string? DatasetId { get; init; }

    /// <summary>
    ///     Experiment metadata. Included only when <c>fields=metadata</c> is requested.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; init; }

    /// <summary>
    ///     Scores directly attached to the experiment. Included only when <c>fields=scores</c> is requested.
    /// </summary>
    [JsonPropertyName("scores")]
    public ScoreV3[]? Scores { get; init; }
}