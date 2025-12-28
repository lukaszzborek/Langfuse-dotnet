using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Request to create a new model definition in Langfuse. Model definitions enable automatic cost calculation and usage
///     tracking for AI models.
/// </summary>
public class CreateModelRequest
{
    /// <summary>
    ///     Name of the AI model (e.g., "gpt-4", "claude-3-opus"). This will be matched against model names in observations for
    ///     automatic tracking.
    /// </summary>
    [JsonPropertyName("modelName")]
    public required string ModelName { get; set; }

    /// <summary>
    ///     Pattern to match model names in observations. Supports wildcards and regex for flexible model identification and
    ///     versioning.
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
    public double? InputPrice { get; set; }

    /// <summary>
    ///     Price per unit for output/completion generation in USD. Used for automatic cost calculation in observations.
    /// </summary>
    [JsonPropertyName("outputPrice")]
    public double? OutputPrice { get; set; }

    /// <summary>
    ///     Total price per unit when input and output are billed together. Alternative to separate input/output pricing.
    /// </summary>
    [JsonPropertyName("totalPrice")]
    public double? TotalPrice { get; set; }

    /// <summary>
    ///     Unit of measurement for model usage (tokens, characters, milliseconds, seconds, images, requests).
    /// </summary>
    [JsonPropertyName("unit")]
    public ModelUsageUnit? Unit { get; set; }

    /// <summary>
    ///     Optional ID of the tokenizer to use for counting tokens. Links to specific tokenization logic for accurate usage
    ///     tracking.
    /// </summary>
    [JsonPropertyName("tokenizerId")]
    public string? TokenizerId { get; set; }

    /// <summary>
    ///     Optional configuration settings for the tokenizer, including overhead tokens and model-specific parameters.
    /// </summary>
    [JsonPropertyName("tokenizerConfig")]
    public TokenizerConfig? TokenizerConfig { get; set; }

    /// <summary>
    ///     Pricing tiers for the model. Enables tiered pricing based on usage thresholds.
    ///     When using tiered pricing, the flat price fields (inputPrice, outputPrice, totalPrice) are ignored.
    /// </summary>
    [JsonPropertyName("pricingTiers")]
    public List<PricingTierInput>? PricingTiers { get; set; }
}