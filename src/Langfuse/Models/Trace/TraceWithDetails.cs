using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Observation;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Represents a trace with detailed nested observations from the Langfuse API
/// </summary>
public class TraceWithDetails : TraceModel
{
    /// <summary>
    ///     Observations (generations, spans, events) associated with this trace
    /// </summary>
    [JsonPropertyName("observations")]
    public ObservationModel[]? Observations { get; set; }

    /// <summary>
    ///     Scores associated with this trace
    /// </summary>
    [JsonPropertyName("scores")]
    public object[]? Scores { get; set; }

    /// <summary>
    ///     HTML representation of the trace (if available)
    /// </summary>
    [JsonPropertyName("htmlPath")]
    public string? HtmlPath { get; set; }

    /// <summary>
    ///     Cost information for the trace
    /// </summary>
    [JsonPropertyName("cost")]
    public decimal? Cost { get; set; }

    /// <summary>
    ///     Token usage information
    /// </summary>
    [JsonPropertyName("usage")]
    public TraceUsage? Usage { get; set; }
}

/// <summary>
///     Token usage information for a trace
/// </summary>
public class TraceUsage
{
    /// <summary>
    ///     Total number of prompt tokens used
    /// </summary>
    [JsonPropertyName("promptTokens")]
    public int? PromptTokens { get; set; }

    /// <summary>
    ///     Total number of completion tokens used
    /// </summary>
    [JsonPropertyName("completionTokens")]
    public int? CompletionTokens { get; set; }

    /// <summary>
    ///     Total number of tokens used
    /// </summary>
    [JsonPropertyName("totalTokens")]
    public int? TotalTokens { get; set; }
}