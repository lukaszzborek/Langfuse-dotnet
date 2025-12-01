namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Represents a tool/function definition for Gen AI operations.
/// </summary>
public class GenAiToolDefinition
{
    /// <summary>
    ///     The name of the tool/function.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The type of tool (typically "function").
    /// </summary>
    public string Type { get; init; } = "function";

    /// <summary>
    ///     A description of what the tool does.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The parameters schema for the tool (JSON Schema format).
    /// </summary>
    public object? Parameters { get; init; }
}