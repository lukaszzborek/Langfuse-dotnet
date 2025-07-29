using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class DatasetItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("input")]
    public object? Input { get; set; }

    [JsonPropertyName("expectedOutput")]
    public object? ExpectedOutput { get; set; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    [JsonPropertyName("sourceTraceId")]
    public string? SourceTraceId { get; set; }

    [JsonPropertyName("sourceObservationId")]
    public string? SourceObservationId { get; set; }

    [JsonPropertyName("datasetId")]
    public string DatasetId { get; set; } = string.Empty;

    [JsonPropertyName("datasetName")]
    public string DatasetName { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}