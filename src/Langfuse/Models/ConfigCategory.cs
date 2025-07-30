using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a category configuration for categorical scores
/// </summary>
public class ConfigCategory
{
    /// <summary>
    ///     Numeric value associated with this category
    /// </summary>
    [JsonPropertyName("value")]
    public double Value { get; set; }

    /// <summary>
    ///     Human-readable label for this category
    /// </summary>
    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;
}