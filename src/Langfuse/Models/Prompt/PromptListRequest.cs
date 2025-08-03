using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

public class PromptListRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("tag")]
    public string? Tag { get; set; }

    [JsonPropertyName("page")]
    public int? Page { get; set; }

    [JsonPropertyName("limit")]
    public int? Limit { get; set; }

    [JsonPropertyName("fromUpdatedAt")]
    public DateTime? FromUpdatedAt { get; set; }

    [JsonPropertyName("toUpdatedAt")]
    public DateTime? ToUpdatedAt { get; set; }
}