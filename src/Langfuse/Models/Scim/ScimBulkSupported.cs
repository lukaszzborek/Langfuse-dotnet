using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimBulkSupported : ScimSupported
{
    [JsonPropertyName("maxOperations")]
    public int MaxOperations { get; set; }

    [JsonPropertyName("maxPayloadSize")]
    public int MaxPayloadSize { get; set; }
}