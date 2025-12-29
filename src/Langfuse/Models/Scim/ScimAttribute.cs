using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Represents a SCIM schema attribute definition.
/// </summary>
public class ScimAttribute
{
    /// <summary>
    ///     Name of the attribute.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Data type of the attribute.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     Indicates if the attribute can have multiple values.
    /// </summary>
    [JsonPropertyName("multiValued")]
    public bool MultiValued { get; set; }

    /// <summary>
    ///     Human-readable description of the attribute.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Indicates if the attribute is required.
    /// </summary>
    [JsonPropertyName("required")]
    public bool Required { get; set; }

    /// <summary>
    ///     Indicates if string matching is case-sensitive.
    /// </summary>
    [JsonPropertyName("caseExact")]
    public bool CaseExact { get; set; }

    /// <summary>
    ///     Mutability of the attribute (readOnly, readWrite, immutable, writeOnly).
    /// </summary>
    [JsonPropertyName("mutability")]
    public string Mutability { get; set; } = string.Empty;

    /// <summary>
    ///     When the attribute is returned in a response (always, never, default, request).
    /// </summary>
    [JsonPropertyName("returned")]
    public string Returned { get; set; } = string.Empty;

    /// <summary>
    ///     Uniqueness constraint (none, server, global).
    /// </summary>
    [JsonPropertyName("uniqueness")]
    public string Uniqueness { get; set; } = string.Empty;

    /// <summary>
    ///     Sub-attributes for complex attributes.
    /// </summary>
    [JsonPropertyName("subAttributes")]
    public List<ScimAttribute>? SubAttributes { get; set; }
}