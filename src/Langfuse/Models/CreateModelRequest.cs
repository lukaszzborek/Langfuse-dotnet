using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Request to create a new model definition in Langfuse. Model definitions enable automatic cost calculation and usage tracking for AI models.
/// </summary>
public class CreateModelRequest
{
    /// <summary>
    ///     Name of the AI model (e.g., "gpt-4", "claude-3-opus"). This will be matched against model names in observations for automatic tracking.
    /// </summary>
    [JsonPropertyName("modelName")]
    public required string ModelName { get; set; } 

    /// <summary>
    ///     Pattern to match model names in observations. Supports wildcards and regex for flexible model identification and versioning.
    /// </summary>
    [JsonPropertyName("matchPattern")]
    public required string MatchPattern { get; set; } 

    /// <summary>
    ///     Optional start date when this model pricing becomes effective. Useful for handling pricing changes over time.
    /// </summary>
    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    ///     Price per unit for input/prompt processing in USD. Used for automatic cost calculation in observations.
    /// </summary>
    [JsonPropertyName("inputPrice")]
    public decimal? InputPrice { get; set; }

    /// <summary>
    ///     Price per unit for output/completion generation in USD. Used for automatic cost calculation in observations.
    /// </summary>
    [JsonPropertyName("outputPrice")]
    public decimal? OutputPrice { get; set; }

    /// <summary>
    ///     Total price per unit when input and output are billed together. Alternative to separate input/output pricing.
    /// </summary>
    [JsonPropertyName("totalPrice")]
    public decimal? TotalPrice { get; set; }

    /// <summary>
    ///     Unit of measurement for model usage (tokens, characters, milliseconds, seconds, images, requests).
    /// </summary>
    [JsonPropertyName("unit")]
    public ModelUsageUnit? Unit { get; set; }

    /// <summary>
    ///     Optional ID of the tokenizer to use for counting tokens. Links to specific tokenization logic for accurate usage tracking.
    /// </summary>
    [JsonPropertyName("tokenizerId")]
    public string? TokenizerId { get; set; }

    /// <summary>
    ///     Optional configuration settings for the tokenizer, including overhead tokens and model-specific parameters.
    /// </summary>
    [JsonPropertyName("tokenizerConfig")]
    public TokenizerConfig? TokenizerConfig { get; set; }
}

/// <summary>
///     Defines the unit of measurement for model usage tracking and pricing calculations.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ModelUsageUnit
{
    /// <summary>
    ///     Usage measured in characters. Commonly used for text-based models that charge per character count.
    /// </summary>
    [JsonStringEnumMemberName("CHARACTERS")]
    Characters,
    
    /// <summary>
    ///     Usage measured in tokens. Most common unit for language models, accounting for tokenized input/output.
    /// </summary>
    [JsonStringEnumMemberName("TOKENS")]
    Tokens,
    
    /// <summary>
    ///     Usage measured in milliseconds. Used for models that charge based on processing time.
    /// </summary>
    [JsonStringEnumMemberName("MILLISECONDS")]
    Milliseconds,
    
    /// <summary>
    ///     Usage measured in seconds. Used for models that charge based on processing time in second intervals.
    /// </summary>
    [JsonStringEnumMemberName("SECONDS")]
    Seconds,
    
    /// <summary>
    ///     Usage measured in images. Used for vision models that process image inputs.
    /// </summary>
    [JsonStringEnumMemberName("IMAGES")]
    Images,
    
    /// <summary>
    ///     Usage measured in requests. Used for models that charge a flat fee per API request regardless of content size.
    /// </summary>
    [JsonStringEnumMemberName("REQUESTS")]
    Requests,
}