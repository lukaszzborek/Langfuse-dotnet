using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     SCIM filter operations support configuration.
/// </summary>
public class ScimFilterSupported : ScimSupported
{
    /// <summary>
    ///     Maximum number of results returned when filtering.
    /// </summary>
    [JsonPropertyName("maxResults")]
    public int? MaxResults { get; set; }
}