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
    ///     Deprecated, use <see cref="PricingTiers" /> instead. Price (USD) per input unit. Creates a default tier if
    ///     pricing tiers are not provided.
    /// </summary>
    [JsonPropertyName("inputPrice")]
    public double? InputPrice { get; set; }

    /// <summary>
    ///     Deprecated, use <see cref="PricingTiers" /> instead. Price (USD) per output unit. Creates a default tier if
    ///     pricing tiers are not provided.
    /// </summary>
    [JsonPropertyName("outputPrice")]
    public double? OutputPrice { get; set; }

    /// <summary>
    ///     Deprecated, use <see cref="PricingTiers" /> instead. Price (USD) per total unit. Cannot be set together with
    ///     input or output price. Creates a default tier if pricing tiers are not provided.
    /// </summary>
    [JsonPropertyName("totalPrice")]
    public double? TotalPrice { get; set; }

    /// <summary>
    ///     Unit of measurement for model usage (tokens, characters, milliseconds, seconds, images, requests). Required.
    /// </summary>
    [JsonPropertyName("unit")]
    public required ModelUsageUnit Unit { get; set; }

    /// <summary>
    ///     Optional tokenizer applied to observations which match this model (<see cref="ModelTokenizerId.Openai" /> or
    ///     <see cref="ModelTokenizerId.Claude" />).
    /// </summary>
    [JsonPropertyName("tokenizerId")]
    public ModelTokenizerId? TokenizerId { get; set; }

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