using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     The ingestion object type that should trigger evaluation runs.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<EvaluationRuleTarget>))]
public enum EvaluationRuleTarget
{
    /// <summary>
    ///     Evaluates live-ingested observations such as generations, spans, and events.
    /// </summary>
    Observation,

    /// <summary>
    ///     Evaluates live experiment executions.
    /// </summary>
    Experiment
}
