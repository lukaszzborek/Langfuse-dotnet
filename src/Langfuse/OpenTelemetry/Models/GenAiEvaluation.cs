namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Represents an evaluation/score for Gen AI operations.
/// </summary>
public class GenAiEvaluation
{
    public required string Name { get; init; }
    public double? ScoreValue { get; init; }
    public string? ScoreLabel { get; init; }
    public string? Explanation { get; init; }
}