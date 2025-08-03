using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a model definition in Langfuse - configuration for how to track usage, pricing, and tokenization for AI models.
///     Models enable automatic cost calculation and usage tracking when traces reference specific AI model names.
/// </summary>
public class Model
{
    /// <summary>
    ///     Unique identifier of the model definition.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the AI model (e.g., "gpt-4", "claude-3-opus"). Used to identify which model was used in observations.
    /// </summary>
    [JsonPropertyName("modelName")]
    public string ModelName { get; set; } = string.Empty;

    /// <summary>
    ///     Pattern to match model names in observations. Supports wildcards and regex for flexible model identification.
    /// </summary>
    [JsonPropertyName("matchPattern")]
    public string MatchPattern { get; set; } = string.Empty;

    /// <summary>
    ///     Optional start date when this model configuration becomes effective. Used for time-based pricing changes.
    /// </summary>
    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }
    
    /// <summary>
    ///     Unit of measurement for model usage (tokens, characters, milliseconds, seconds, images, requests).
    /// </summary>
    [JsonPropertyName("unit")]
    public ModelUsageUnit? Unit { get; set; }
    
    /// <summary>
    ///     Price per unit for input/prompt processing in USD. Used for automatic cost calculation.
    /// </summary>
    [JsonPropertyName("inputPrice")]
    public decimal? InputPrice { get; set; }

    /// <summary>
    ///     Price per unit for output/completion generation in USD. Used for automatic cost calculation.
    /// </summary>
    [JsonPropertyName("outputPrice")]
    public decimal? OutputPrice { get; set; }

    /// <summary>
    ///     Total price per unit when input and output are billed together. Alternative to separate input/output pricing.
    /// </summary>
    [JsonPropertyName("totalPrice")]
    public decimal? TotalPrice { get; set; }

    /// <summary>
    ///     ID of the tokenizer to use for counting tokens. Links to specific tokenization logic for accurate usage tracking.
    /// </summary>
    [JsonPropertyName("tokenizerId")]
    public string? TokenizerId { get; set; }

    /// <summary>
    ///     Configuration settings for the tokenizer, including overhead tokens and model-specific parameters.
    /// </summary>
    [JsonPropertyName("tokenizerConfig")]
    public TokenizerConfig? TokenizerConfig { get; set; }
    
    /// <summary>
    ///     Whether this model is managed by Langfuse (predefined) or custom-defined by the user.
    /// </summary>
    [JsonPropertyName("isLangfuseManaged")]
    public bool IsLangfuseManaged { get; set; }
    
    /// <summary>
    ///     Detailed pricing structure for different types of usage (input, output, caching, reasoning, etc.).
    /// </summary>
    [JsonPropertyName("prices")]
    public ModelPrices? Prices { get; set; }
}

/// <summary>
///     Configuration settings for tokenization behavior, including overhead costs and model-specific parameters.
/// </summary>
public class TokenizerConfig
{
    /// <summary>
    ///     Additional tokens to add per function/tool name. Accounts for overhead in function calling scenarios.
    /// </summary>
    [JsonPropertyName("tokensPerName")]
    public int? TokensPerName { get; set; }
    
    /// <summary>
    ///     Name of the specific tokenizer model to use for counting tokens (e.g., "cl100k_base", "r50k_base").
    /// </summary>
    [JsonPropertyName("tokenizerModel")]
    public string? TokenizerModel { get; set; }
    
    /// <summary>
    ///     Additional tokens to add per message in chat-based scenarios. Accounts for message formatting overhead.
    /// </summary>
    [JsonPropertyName("tokensPerMessage")]
    public int? TokensPerMessage { get; set; }
}

/// <summary>
///     Comprehensive pricing structure for different types of model usage, supporting various billing models and advanced features.
/// </summary>
public class ModelPrices
{
    /// <summary>
    ///     Standard input processing price per unit.
    /// </summary>
    [JsonPropertyName("input")]
    public ModelPrice? Input { get; set; }
    
    /// <summary>
    ///     Standard output generation price per unit.
    /// </summary>
    [JsonPropertyName("output")]
    public ModelPrice? Output { get; set; }
    
    /// <summary>
    ///     Price for output tokens (alternative naming for output pricing).
    /// </summary>
    [JsonPropertyName("output_tokens")]
    public ModelPrice? OutputTokens { get; set; }
    
    /// <summary>
    ///     Price for creating cache from input tokens. Used in caching-enabled models for initial cache population.
    /// </summary>
    [JsonPropertyName("cache_creation_input_tokens")]
    public ModelPrice? CacheCreationInputTokens { get; set; }
    
    /// <summary>
    ///     Price for input cache creation operations (alternative naming for cache creation pricing).
    /// </summary>
    [JsonPropertyName("input_cache_creation")]
    public ModelPrice? InputCacheCreation { get; set; }
    
    /// <summary>
    ///     Price for reading from cached input tokens. Typically lower than standard input pricing due to cache efficiency.
    /// </summary>
    [JsonPropertyName("cache_read_input_tokens")]
    public ModelPrice? CacheReadInputTokens { get; set; }
    
    /// <summary>
    ///     Price for input cache read operations (alternative naming for cache read pricing).
    /// </summary>
    [JsonPropertyName("input_cache_read")]
    public ModelPrice? InputCacheRead { get; set; }
    
    /// <summary>
    ///     Price for input tokens (alternative naming for input pricing).
    /// </summary>
    [JsonPropertyName("input_tokens")]
    public ModelPrice? InputTokens { get; set; }
    
    /// <summary>
    ///     Price based on prompt token count. Used in models with prompt-specific pricing structures.
    /// </summary>
    [JsonPropertyName("prompt_token_count")]
    public ModelPrice? PromptTokenCount { get; set; }
    
    /// <summary>
    ///     Price based on candidate token count. Used in models that generate multiple response candidates.
    /// </summary>
    [JsonPropertyName("candidates_token_count")]
    public ModelPrice? CandidatesTokenCount { get; set; }
   
    /// <summary>
    ///     Price for reasoning/thinking tokens. Used in models with visible reasoning processes.
    /// </summary>
    [JsonPropertyName("thoughts_token_count")]
    public ModelPrice? ThoughtsTokenCount { get; set; }
    
    /// <summary>
    ///     Price for output reasoning tokens (alternative naming for reasoning token pricing).
    /// </summary>
    [JsonPropertyName("output_reasoning")]
    public ModelPrice? OutputReasoning { get; set; }
    
    /// <summary>
    ///     Price for processing input audio tokens. Used in multimodal models that accept audio input.
    /// </summary>
    [JsonPropertyName("input_audio_tokens")]
    public ModelPrice? InputAudioTokens { get; set; }
}

/// <summary>
///     Represents a price value for a specific type of model usage.
/// </summary>
public class ModelPrice
{
    /// <summary>
    ///     The price amount in USD for this usage type.
    /// </summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}