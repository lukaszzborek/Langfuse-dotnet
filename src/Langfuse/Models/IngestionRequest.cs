using System.Text.Json.Serialization;

namespace Langfuse.Models;

public class IngestionRequest
{
    [JsonPropertyName("batch")]
    public object[] Batch { get; set; }
}