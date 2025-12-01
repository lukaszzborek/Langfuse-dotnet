namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Represents a tool/function call in a Gen AI message.
/// </summary>
public class GenAiToolCall
{
    /// <summary>
    ///     The unique identifier for this tool call.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     The type of tool call (typically "function").
    /// </summary>
    public string Type { get; init; } = "function";

    /// <summary>
    ///     The function details for the tool call.
    /// </summary>
    public required GenAiToolCallFunction Function { get; init; }
}