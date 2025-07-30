using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class CreateDatasetRunItemRequest
{
    [JsonPropertyName("runName")]
    public string RunName { get; set; } = string.Empty;

    [JsonPropertyName("runDescription")]
    public string? RunDescription { get; set; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    [JsonPropertyName("datasetItemId")]
    public required string DatasetItemId { get; set; } 

    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
}