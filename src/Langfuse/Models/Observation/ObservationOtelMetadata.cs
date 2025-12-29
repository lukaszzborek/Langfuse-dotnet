using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.OpenTelemetry;

namespace zborek.Langfuse.Models.Observation;

/// <summary>
///     Represents OpenTelemetry-structured metadata from an observation.
///     Contains attributes, resource attributes, and scope information.
/// </summary>
public class ObservationOtelMetadata
{
    private const string GenAiPrefix = "gen_ai.";

    /// <summary>
    ///     OpenTelemetry span attributes containing telemetry data.
    ///     Keys include langfuse-specific attributes (langfuse.*) and
    ///     semantic conventions (gen_ai.*).
    /// </summary>
    [JsonPropertyName("attributes")]
    public Dictionary<string, string>? Attributes { get; set; }

    /// <summary>
    ///     OpenTelemetry resource attributes describing the source of the telemetry.
    ///     Typically includes service.name, service.version, etc.
    /// </summary>
    [JsonPropertyName("resourceAttributes")]
    public Dictionary<string, string>? ResourceAttributes { get; set; }

    /// <summary>
    ///     OpenTelemetry instrumentation scope information.
    /// </summary>
    [JsonPropertyName("scope")]
    public OtelScopeInfo? Scope { get; set; }

    /// <summary>
    ///     Gets the Langfuse user ID from attributes.
    /// </summary>
    [JsonIgnore]
    public string? UserId => GetAttribute(LangfuseAttributes.UserId);

    /// <summary>
    ///     Gets the Langfuse session ID from attributes.
    /// </summary>
    [JsonIgnore]
    public string? SessionId => GetAttribute(LangfuseAttributes.SessionId);

    /// <summary>
    ///     Gets the Langfuse trace tags parsed from JSON array.
    /// </summary>
    [JsonIgnore]
    public List<string>? Tags => ParseTags();

    /// <summary>
    ///     Gets all custom user metadata (attributes with langfuse.observation.metadata.* prefix).
    ///     Keys are returned without the prefix.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, string> CustomMetadata =>
        GetPrefixedAttributes(LangfuseAttributes.ObservationMetadataPrefix);

    /// <summary>
    ///     Gets all gen_ai.* semantic convention attributes.
    ///     Keys are returned without the prefix.
    /// </summary>
    [JsonIgnore]
    public Dictionary<string, string> GenAiAttributes => GetPrefixedAttributes(GenAiPrefix);

    /// <summary>
    ///     Gets an attribute value by key.
    /// </summary>
    /// <param name="key">The attribute key.</param>
    /// <returns>The attribute value, or null if not found.</returns>
    public string? GetAttribute(string key)
    {
        if (Attributes is null)
        {
            return null;
        }

        return Attributes.GetValueOrDefault(key);
    }

    /// <summary>
    ///     Gets a resource attribute value by key.
    /// </summary>
    /// <param name="key">The resource attribute key.</param>
    /// <returns>The resource attribute value, or null if not found.</returns>
    public string? GetResourceAttribute(string key)
    {
        if (ResourceAttributes is null)
        {
            return null;
        }

        return ResourceAttributes.GetValueOrDefault(key);
    }

    private List<string>? ParseTags()
    {
        var tagsJson = GetAttribute(LangfuseAttributes.TraceTags);
        if (string.IsNullOrEmpty(tagsJson))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<List<string>>(tagsJson);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private Dictionary<string, string> GetPrefixedAttributes(string prefix)
    {
        var result = new Dictionary<string, string>();

        if (Attributes is null)
        {
            return result;
        }

        foreach (KeyValuePair<string, string> kvp in Attributes)
        {
            if (kvp.Key.StartsWith(prefix, StringComparison.Ordinal))
            {
                var keyWithoutPrefix = kvp.Key[prefix.Length..];
                result[keyWithoutPrefix] = kvp.Value;
            }
        }

        return result;
    }
}

/// <summary>
///     OpenTelemetry instrumentation scope information.
/// </summary>
public class OtelScopeInfo
{
    /// <summary>
    ///     Name of the instrumentation scope (e.g., "Langfuse").
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Attributes associated with the instrumentation scope.
    /// </summary>
    [JsonPropertyName("attributes")]
    public Dictionary<string, string>? Attributes { get; set; }
}