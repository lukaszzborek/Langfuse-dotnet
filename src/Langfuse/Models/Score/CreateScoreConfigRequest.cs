using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Request model for creating a new score configuration
/// </summary>
public class CreateScoreConfigRequest
{
    /// <summary>
    ///     Name of the score configuration
    /// </summary>
    [Required]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    ///     Data type of the score (NUMERIC, CATEGORICAL, BOOLEAN)
    /// </summary>
    [Required]
    [JsonPropertyName("dataType")]
    public required ScoreConfigDataType DataType { get; set; }

    /// <summary>
    ///     Description of the score configuration
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Minimum value for numeric scores
    /// </summary>
    [JsonPropertyName("minValue")]
    public double? MinValue { get; set; }

    /// <summary>
    ///     Maximum value for numeric scores
    /// </summary>
    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; set; }

    /// <summary>
    ///     Allowed categories for categorical scores
    /// </summary>
    [JsonPropertyName("categories")]
    public ConfigCategory[]? Categories { get; set; }
}