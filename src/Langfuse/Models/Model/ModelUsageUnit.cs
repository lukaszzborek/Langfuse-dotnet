using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Defines the unit of measurement for model usage tracking and pricing calculations.
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<ModelUsageUnit>))]
public enum ModelUsageUnit
{
    /// <summary>
    ///     Usage measured in characters. Commonly used for text-based models that charge per character count.
    /// </summary>
    Characters,

    /// <summary>
    ///     Usage measured in tokens. Most common unit for language models, accounting for tokenized input/output.
    /// </summary>
    Tokens,

    /// <summary>
    ///     Usage measured in milliseconds. Used for models that charge based on processing time.
    /// </summary>
    Milliseconds,

    /// <summary>
    ///     Usage measured in seconds. Used for models that charge based on processing time in second intervals.
    /// </summary>
    Seconds,

    /// <summary>
    ///     Usage measured in images. Used for vision models that process image inputs.
    /// </summary>
    Images,

    /// <summary>
    ///     Usage measured in requests. Used for models that charge a flat fee per API request regardless of content size.
    /// </summary>
    Requests
}