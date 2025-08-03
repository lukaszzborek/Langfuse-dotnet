using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

public class CreateDatasetItemRequest
{
    [JsonPropertyName("datasetName")]
    public required string DatasetName { get; set; }

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

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("status")]
    public DatasetStatus? Status { get; set; }
}