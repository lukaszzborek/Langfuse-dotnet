using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class Model
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("modelName")]
    public string ModelName { get; set; } = string.Empty;

    [JsonPropertyName("matchPattern")]
    public string MatchPattern { get; set; } = string.Empty;

    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }
    
    [JsonPropertyName("unit")]
    public ModelUsageUnit? Unit { get; set; }
    
    [JsonPropertyName("inputPrice")]
    public decimal? InputPrice { get; set; }

    [JsonPropertyName("outputPrice")]
    public decimal? OutputPrice { get; set; }

    [JsonPropertyName("totalPrice")]
    public decimal? TotalPrice { get; set; }

    [JsonPropertyName("tokenizerId")]
    public string? TokenizerId { get; set; }

    [JsonPropertyName("tokenizerConfig")]
    public TokenizerConfig? TokenizerConfig { get; set; }
    
    [JsonPropertyName("isLangfuseManaged")]
    public bool IsLangfuseManaged { get; set; }
    
    [JsonPropertyName("prices")]
    public ModelPrices? Prices { get; set; }
}

public class TokenizerConfig
{
    [JsonPropertyName("tokensPerName")]
    public int? TokensPerName { get; set; }
    
    [JsonPropertyName("tokenizerModel")]
    public string? TokenizerModel { get; set; }
    
    [JsonPropertyName("tokensPerMessage")]
    public int? TokensPerMessage { get; set; }
}

public class ModelPrices
{
    [JsonPropertyName("input")]
    public ModelPrice? Input { get; set; }
    
    [JsonPropertyName("output")]
    public ModelPrice? Output { get; set; }
    
    [JsonPropertyName("output_tokens")]
    public ModelPrice? OutputTokens { get; set; }
    
    [JsonPropertyName("cache_creation_input_tokens")]
    public ModelPrice? CacheCreationInputTokens { get; set; }
    
    [JsonPropertyName("input_cache_creation")]
    public ModelPrice? InputCacheCreation { get; set; }
    
    [JsonPropertyName("cache_read_input_tokens")]
    public ModelPrice? CacheReadInputTokens { get; set; }
    
    [JsonPropertyName("input_cache_read")]
    public ModelPrice? InputCacheRead { get; set; }
    
    [JsonPropertyName("input_tokens")]
    public ModelPrice? InputTokens { get; set; }
    
    [JsonPropertyName("prompt_token_count")]
    public ModelPrice? PromptTokenCount { get; set; }
    
    [JsonPropertyName("candidates_token_count")]
    public ModelPrice? CandidatesTokenCount { get; set; }
   
    [JsonPropertyName("thoughts_token_count")]
    public ModelPrice? ThoughtsTokenCount { get; set; }
    
    [JsonPropertyName("output_reasoning")]
    public ModelPrice? OutputReasoning { get; set; }
    
    [JsonPropertyName("input_audio_tokens")]
    public ModelPrice? InputAudioTokens { get; set; }
}

public class ModelPrice
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}