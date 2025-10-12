using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Type of filter condition for trace filtering
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<TraceFilterType>))]
public enum TraceFilterType
{
    /// <summary>
    ///     DateTime filter with operators: &gt;, &lt;, &gt;=, &lt;=
    /// </summary>
    [JsonPropertyName("datetime")]
    DateTime,

    /// <summary>
    ///     String filter with operators: =, contains, does not contain, starts with, ends with
    /// </summary>
    [JsonPropertyName("string")]
    String,

    /// <summary>
    ///     Number filter with operators: =, &gt;, &lt;, &gt;=, &lt;=
    /// </summary>
    [JsonPropertyName("number")]
    Number,

    /// <summary>
    ///     String options filter with operators: any of, none of
    /// </summary>
    [JsonPropertyName("stringOptions")]
    StringOptions,

    /// <summary>
    ///     Category options filter with operators: any of, none of
    /// </summary>
    [JsonPropertyName("categoryOptions")]
    CategoryOptions,

    /// <summary>
    ///     Array options filter with operators: any of, none of, all of
    /// </summary>
    [JsonPropertyName("arrayOptions")]
    ArrayOptions,

    /// <summary>
    ///     String object filter with operators: =, contains, does not contain, starts with, ends with
    /// </summary>
    [JsonPropertyName("stringObject")]
    StringObject,

    /// <summary>
    ///     Number object filter with operators: =, &gt;, &lt;, &gt;=, &lt;=
    /// </summary>
    [JsonPropertyName("numberObject")]
    NumberObject,

    /// <summary>
    ///     Boolean filter with operators: =, &lt;&gt;
    /// </summary>
    [JsonPropertyName("boolean")]
    Boolean,

    /// <summary>
    ///     Null filter with operators: is null, is not null
    /// </summary>
    [JsonPropertyName("null")]
    Null
}
