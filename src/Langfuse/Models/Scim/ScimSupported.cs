using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Base class for SCIM feature support configuration.
/// </summary>
public class ScimSupported
{
    /// <summary>
    ///     Indicates whether this feature is supported.
    /// </summary>
    [JsonPropertyName("supported")]
    public bool Supported { get; set; }
}