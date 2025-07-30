using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a score configuration in Langfuse
/// </summary>
public class ScoreConfig
{
    /// <summary>
    ///     Unique identifier of the score configuration
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the score configuration
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Data type of the score (NUMERIC, CATEGORICAL, BOOLEAN)
    /// </summary>
    [JsonPropertyName("dataType")]
    public ScoreDataType DataType { get; set; }

    /// <summary>
    ///     Whether this score configuration is archived
    /// </summary>
    [JsonPropertyName("isArchived")]
    public bool IsArchived { get; set; }

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

    /// <summary>
    ///     Creation timestamp
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    ///     Last update timestamp
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    ///     Project ID this score configuration belongs to
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;
}