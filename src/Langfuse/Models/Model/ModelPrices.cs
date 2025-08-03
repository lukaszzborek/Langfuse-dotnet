using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Comprehensive pricing structure for different types of model usage, supporting various billing models and advanced
///     features.
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