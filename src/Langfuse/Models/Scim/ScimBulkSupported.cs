using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     SCIM bulk operations support configuration.
/// </summary>
public class ScimBulkSupported : ScimSupported
{
    /// <summary>
    ///     Maximum number of operations in a single bulk request.
    /// </summary>
    [JsonPropertyName("maxOperations")]
    public int MaxOperations { get; set; }

    /// <summary>
    ///     Maximum payload size in bytes for a bulk request.
    /// </summary>
    [JsonPropertyName("maxPayloadSize")]
    public int MaxPayloadSize { get; set; }
}