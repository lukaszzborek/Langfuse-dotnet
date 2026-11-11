using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents an evaluator observation (assessing the relevance or quality of LLM output).
/// </summary>
public class OtelEvaluator : OtelObservation
{
    internal OtelEvaluator(Activity? activity) : base(activity)
    {
    }

    /// <summary>
    ///     Records the evaluation result.
    /// </summary>
    public void SetEvaluation(GenAiEvaluation evaluation)
    {
        GenAiActivityHelper.RecordEvaluation(Activity, evaluation);
    }

    /// <summary>
    ///     Records the evaluation result.
    /// </summary>
    public void SetEvaluation(string name, double score, string? label = null, string? explanation = null)
    {
        GenAiActivityHelper.RecordEvaluation(Activity, name, score, label, explanation);
    }
}
