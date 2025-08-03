using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Model;

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