using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Represents a score configuration in Langfuse that defines the structure, validation rules, and metadata for score
///     types.
///     Score configs standardize how scores are created and interpreted across your project.
/// </summary>
public class ScoreConfig
{
    /// <summary>
    ///     Unique identifier of the score configuration
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the score configuration that will be used to identify score types (e.g., "quality", "relevance").
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Data type defining how scores should be validated and interpreted (NUMERIC, CATEGORICAL, BOOLEAN).
    /// </summary>
    [JsonPropertyName("dataType")]
    public ScoreDataType DataType { get; set; }

    /// <summary>
    ///     Whether this score configuration is archived and no longer available for creating new scores.
    /// </summary>
    [JsonPropertyName("isArchived")]
    public bool IsArchived { get; set; }

    /// <summary>
    ///     Human-readable description explaining what this score configuration measures and how it should be used.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Minimum allowed value for numeric scores, enforced during score creation and validation.
    /// </summary>
    [JsonPropertyName("minValue")]
    public double? MinValue { get; set; }

    /// <summary>
    ///     Maximum allowed value for numeric scores, enforced during score creation and validation.
    /// </summary>
    [JsonPropertyName("maxValue")]
    public double? MaxValue { get; set; }

    /// <summary>
    ///     Predefined categories for categorical scores, each with labels and optional numeric mappings.
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