using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class ApiKeyList
{
    [JsonPropertyName("apiKeys")]
    public List<ApiKeySummary> ApiKeys { get; set; } = new();
}