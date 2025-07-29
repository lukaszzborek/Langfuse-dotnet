using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class CreateModelRequest
{
    [JsonPropertyName("modelName")]
    public required string ModelName { get; set; } 

    [JsonPropertyName("matchPattern")]
    public required string MatchPattern { get; set; } 

    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }

    [JsonPropertyName("inputPrice")]
    public decimal? InputPrice { get; set; }

    [JsonPropertyName("outputPrice")]
    public decimal? OutputPrice { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal? TotalPrice { get; set; }

    [JsonPropertyName("unit")]
    public ModelUsageUnit? Unit { get; set; }

    [JsonPropertyName("tokenizerId")]
    public string? TokenizerId { get; set; }

    [JsonPropertyName("tokenizerConfig")]
    public TokenizerConfig? TokenizerConfig { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ModelUsageUnit
{
    [JsonStringEnumMemberName("CHARACTERS")]
    Characters,
    
    [JsonStringEnumMemberName("TOKENS")]
    Tokens,
    
    [JsonStringEnumMemberName("MILLISECONDS")]
    Milliseconds,
    
    [JsonStringEnumMemberName("SECONDS")]
    Seconds,
    
    [JsonStringEnumMemberName("IMAGES")]
    Images,
    
    [JsonStringEnumMemberName("REQUESTS")]
    Requests,
}