using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Langfuse log level
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<LangfuseLogLevel>))]
public enum LangfuseLogLevel
{
    /// <summary>
    ///     Debug
    /// </summary>
    Debug = -1,

    /// <summary>
    ///     Default
    /// </summary>
    Default = 0,

    /// <summary>
    ///     Warning
    /// </summary>
    Warning = 1,

    /// <summary>
    ///     Error
    /// </summary>
    Error = 2
}