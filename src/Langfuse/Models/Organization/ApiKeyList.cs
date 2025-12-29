using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     List of API keys for a project.
/// </summary>
public class ApiKeyList
{
    /// <summary>
    ///     Collection of API key summaries.
    /// </summary>
    [JsonPropertyName("apiKeys")]
    public List<ApiKeySummary> ApiKeys { get; set; } = new();
}