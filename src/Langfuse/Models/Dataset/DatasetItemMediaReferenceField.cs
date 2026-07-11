using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     The dataset item field a media reference was found in.
/// </summary>
[JsonConverter(typeof(CamelCaseEnumConverter<DatasetItemMediaReferenceField>))]
public enum DatasetItemMediaReferenceField
{
    /// <summary>
    ///     The dataset item input field.
    /// </summary>
    Input,

    /// <summary>
    ///     The dataset item expected output field.
    /// </summary>
    ExpectedOutput,

    /// <summary>
    ///     The dataset item metadata field.
    /// </summary>
    Metadata
}