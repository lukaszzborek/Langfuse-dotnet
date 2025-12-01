namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Represents a chat message for Gen AI operations following OpenTelemetry semantic conventions.
///     Based on: https://opentelemetry.io/docs/specs/semconv/gen-ai/gen-ai-events/
/// </summary>
public class GenAiMessage
{
    /// <summary>
    ///     The role of the message author (e.g., "system", "user", "assistant", "tool").
    /// </summary>
    public required string Role { get; init; }

    /// <summary>
    ///     The content of the message. Can be a simple string or structured content.
    /// </summary>
    public string? Content { get; init; }

    /// <summary>
    ///     Tool calls requested by the assistant (for assistant messages with function calls).
    /// </summary>
    public List<GenAiToolCall>? ToolCalls { get; init; }

    /// <summary>
    ///     The ID of the tool call this message is responding to (for tool messages).
    /// </summary>
    public string? ToolCallId { get; init; }
}