namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Represents an evaluation/score for Gen AI operations.
/// </summary>
public class GenAiEvaluation
{
    /// <summary>
    ///     The name of the evaluation metric.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     The numeric score value for this evaluation.
    /// </summary>
    public double? ScoreValue { get; init; }

    /// <summary>
    ///     The categorical label for this score.
    /// </summary>
    public string? ScoreLabel { get; init; }

    /// <summary>
    ///     An explanation or reasoning for the score.
    /// </summary>
    public string? Explanation { get; init; }
}