using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class DatasetRunWithItems
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    [JsonPropertyName("datasetId")]
    public string DatasetId { get; set; } = string.Empty;

    [JsonPropertyName("datasetName")]
    public string DatasetName { get; set; } = string.Empty;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("datasetRunItems")]
    public List<DatasetRunItem> DatasetRunItems { get; set; } = new();
}

public class DatasetRunItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("datasetRunId")]
    public string DatasetRunId { get; set; } = string.Empty;
    
    [JsonPropertyName("datasetRunName")]
    public string DatasetRunName { get; set; } = string.Empty;
    
    [JsonPropertyName("datasetItemId")]
    public string DatasetItemId { get; set; } = string.Empty;

    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;

    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}