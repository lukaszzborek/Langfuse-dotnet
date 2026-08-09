using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Feedback;

/// <summary>
///     Category of the thing the feedback is about.
/// </summary>
[JsonConverter(typeof(KebabCaseLowerEnumConverter<FeedbackTargetType>))]
public enum FeedbackTargetType
{
    /// <summary>
    ///     A Langfuse skill
    /// </summary>
    Skill,

    /// <summary>
    ///     An MCP tool
    /// </summary>
    McpTool,

    /// <summary>
    ///     The Langfuse CLI
    /// </summary>
    Cli,

    /// <summary>
    ///     Documentation pages
    /// </summary>
    Docs,

    /// <summary>
    ///     The public API
    /// </summary>
    PublicApi,

    /// <summary>
    ///     Anything else
    /// </summary>
    Other
}
