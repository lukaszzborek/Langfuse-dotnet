using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class IngestionRequest
{
    [JsonPropertyName("batch")]
    public object[] Batch { get; set; }
}