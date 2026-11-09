using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Model definition used for calculating USD cost from usage submitted with observations.
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
    public double? InputPrice { get; set; }

    /// <summary>
    ///     Price per unit for output/completion generation in USD. Used for automatic cost calculation.
    /// </summary>
    [JsonPropertyName("outputPrice")]
    public double? OutputPrice { get; set; }

    /// <summary>
    ///     Total price per unit when input and output are billed together. Alternative to separate input/output pricing.
    /// </summary>
    [JsonPropertyName("totalPrice")]
    public double? TotalPrice { get; set; }

    /// <summary>
    ///     Legacy tokenizer identifier kept for API compatibility. Ingestion no longer uses it to infer observation usage.
    /// </summary>
    [JsonPropertyName("tokenizerId")]
    public string? TokenizerId { get; set; }

    /// <summary>
    ///     Legacy tokenizer configuration kept for API compatibility. Ingestion no longer uses it to infer observation usage.
    /// </summary>
    [JsonPropertyName("tokenizerConfig")]
    public TokenizerConfig TokenizerConfig { get; set; } = new();

    /// <summary>
    ///     Whether this model is managed by Langfuse (predefined) or custom-defined by the user.
    /// </summary>
    [JsonPropertyName("isLangfuseManaged")]
    public bool IsLangfuseManaged { get; set; }

    /// <summary>
    ///     Timestamp when the model was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Detailed pricing structure for different types of usage (input, output, caching, reasoning, etc.).
    ///     Keys are usage types (e.g., "input", "output"), values are ModelPrice objects.
    /// </summary>
    [JsonPropertyName("prices")]
    public Dictionary<string, ModelPrice> Prices { get; set; } = new();

    /// <summary>
    ///     Pricing tiers for the model. Enables tiered pricing based on usage thresholds.
    ///     When using tiered pricing, the flat price fields (inputPrice, outputPrice, totalPrice) are ignored.
    /// </summary>
    [JsonPropertyName("pricingTiers")]
    public List<PricingTier> PricingTiers { get; set; } = [];
}