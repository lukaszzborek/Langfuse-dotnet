namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Represents the function details within a tool call.
/// </summary>
public class GenAiToolCallFunction
{
    /// <summary>
    ///     The name of the function to call.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The arguments to pass to the function, typically as a JSON string.
    /// </summary>
    public string? Arguments { get; init; }
}