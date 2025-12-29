namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Configuration for Gen AI agent operations.
/// </summary>
public class GenAiAgentConfig
{
    /// <summary>
    ///     The unique identifier for the agent.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    ///     The display name of the agent.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     An optional description of the agent's purpose or capabilities.
    /// </summary>
    public string? Description { get; init; }
}