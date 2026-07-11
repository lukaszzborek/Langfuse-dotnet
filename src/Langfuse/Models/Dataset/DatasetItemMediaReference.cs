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
    public DatasetItemMediaReferenceField Field { get; set; }

    /// <summary>
    ///     The Langfuse media reference string, e.g. `@@@langfuseMedia:type=image/png|id=...|source=bytes@@@`.
    /// </summary>
    [JsonPropertyName("referenceString")]
    public string ReferenceString { get; set; } = string.Empty;

    /// <summary>
    ///     JSONPath of the string holding the reference within the field, e.g. `$['image']`.
    /// </summary>
    [JsonPropertyName("jsonPath")]
    public string JsonPath { get; set; } = string.Empty;

    /// <summary>
    ///     The resolved media record.
    /// </summary>
    [JsonPropertyName("media")]
    public DatasetItemMediaReferenceMedia Media { get; set; } = new();
}