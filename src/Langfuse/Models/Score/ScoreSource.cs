using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Source of the score
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScoreSource
{
    /// <summary>
    ///     Manual annotation
    /// </summary>
    [JsonPropertyName("ANNOTATION")] ANNOTATION,

    /// <summary>
    ///     API-created score
    /// </summary>
    [JsonPropertyName("API")] API,

    /// <summary>
    ///     Evaluation-generated score
    /// </summary>
    [JsonPropertyName("EVAL")] EVAL
}