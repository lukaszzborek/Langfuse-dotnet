using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Models.Experiment;

/// <summary>
///     A single experiment item with its trace context, optional dataset linkage, inputs/outputs, metadata, and scores.
/// </summary>
public class ExperimentItem
{
    /// <summary>
    ///     Experiment item identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    ///     Identifier of the trace produced by this experiment item.
    /// </summary>
    [JsonPropertyName("traceId")]
    public required string TraceId { get; init; }

    /// <summary>
    ///     Start time of the experiment item.
    /// </summary>
    [JsonPropertyName("startTime")]
    public required DateTime StartTime { get; init; }

    /// <summary>
    ///     End time of the experiment item, or null when not available.
    /// </summary>
    [JsonPropertyName("endTime")]
    public required DateTime? EndTime { get; init; }

    /// <summary>
    ///     Level of the experiment item indicating importance or severity.
    /// </summary>
    [JsonPropertyName("level")]
    public required LangfuseLogLevel Level { get; init; }

    /// <summary>
    ///     Environment the experiment item was recorded in.
    /// </summary>
    [JsonPropertyName("environment")]
    public required string Environment { get; init; }

    /// <summary>
    ///     Identifier of the experiment this item belongs to.
    /// </summary>
    [JsonPropertyName("experimentId")]
    public required string ExperimentId { get; init; }

    /// <summary>
    ///     Name of the experiment this item belongs to.
    /// </summary>
    [JsonPropertyName("experimentName")]
    public required string ExperimentName { get; init; }

    /// <summary>
    ///     Identifier of the experiment item within the experiment.
    /// </summary>
    [JsonPropertyName("experimentItemId")]
    public required string ExperimentItemId { get; init; }

    /// <summary>
    ///     Dataset identifier of the experiment. Included when <c>fields=dataset</c> is requested.
    /// </summary>
    [JsonPropertyName("experimentDatasetId")]
    public string? ExperimentDatasetId { get; init; }

    /// <summary>
    ///     Version of the experiment item. Included when <c>fields=dataset</c> is requested.
    /// </summary>
    [JsonPropertyName("experimentItemVersion")]
    public DateTime? ExperimentItemVersion { get; init; }

    /// <summary>
    ///     Input of the experiment item. Included when <c>fields=io</c> is requested.
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; init; }

    /// <summary>
    ///     Output of the experiment item. Included when <c>fields=io</c> is requested.
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; init; }

    /// <summary>
    ///     Expected output of the experiment item. Included when <c>fields=io</c> is requested.
    /// </summary>
    [JsonPropertyName("expectedOutput")]
    public object? ExpectedOutput { get; init; }

    /// <summary>
    ///     Metadata of the experiment item trace. Included when <c>fields=metadata</c> is requested.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; init; }

    /// <summary>
    ///     Metadata of the experiment item. Included when <c>fields=itemMetadata</c> is requested.
    /// </summary>
    [JsonPropertyName("experimentItemMetadata")]
    public object? ExperimentItemMetadata { get; init; }

    /// <summary>
    ///     Metadata of the experiment. Included when <c>fields=experimentMetadata</c> is requested.
    /// </summary>
    [JsonPropertyName("experimentMetadata")]
    public object? ExperimentMetadata { get; init; }

    /// <summary>
    ///     Description of the experiment. Included when <c>fields=experimentMetadata</c> is requested.
    /// </summary>
    [JsonPropertyName("experimentDescription")]
    public string? ExperimentDescription { get; init; }

    /// <summary>
    ///     Item and trace scores of the experiment item. Included only when <c>fields=scores</c> is requested.
    ///     Experiment-level scores are returned by the experiments endpoint.
    /// </summary>
    [JsonPropertyName("scores")]
    public ScoreV3[]? Scores { get; init; }
}