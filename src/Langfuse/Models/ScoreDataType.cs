using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Score data type
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScoreDataType
{
    /// <summary>
    ///     Numeric
    /// </summary>
    [JsonPropertyName("NUMERIC")] Numeric,

    /// <summary>
    ///     Boolean
    /// </summary>
    [JsonPropertyName("BOOLEAN")] Boolean,

    /// <summary>
    ///     Categorical
    /// </summary>
    [JsonPropertyName("CATEGORICAL")] Categorical
}