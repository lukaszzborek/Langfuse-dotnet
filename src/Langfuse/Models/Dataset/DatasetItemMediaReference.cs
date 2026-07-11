using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     A resolved Langfuse media reference found in a dataset item's input, expected output, or metadata.
/// </summary>
public class DatasetItemMediaReference
{
    /// <summary>
    ///     The dataset item field containing the reference.
    /// </summary>
    [JsonPropertyName("field")]
    public required DatasetItemMediaReferenceField Field { get; init; }

    /// <summary>
    ///     The Langfuse media reference string, e.g. `@@@langfuseMedia:type=image/png|id=...|source=bytes@@@`.
    /// </summary>
    [JsonPropertyName("referenceString")]
    public required string ReferenceString { get; init; }

    /// <summary>
    ///     JSONPath of the string holding the reference within the field, e.g. `$['image']`.
    /// </summary>
    [JsonPropertyName("jsonPath")]
    public required string JsonPath { get; init; }

    /// <summary>
    ///     The resolved media record.
    /// </summary>
    [JsonPropertyName("media")]
    public required DatasetItemMediaReferenceMedia Media { get; init; }
}