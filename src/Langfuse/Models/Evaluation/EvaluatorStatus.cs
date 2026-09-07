using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Effective evaluator runtime status.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<EvaluatorStatus>))]
public enum EvaluatorStatus
{
    /// <summary>
    ///     The evaluator can run.
    /// </summary>
    Active,

    /// <summary>
    ///     Langfuse paused execution until the underlying issue is resolved.
    /// </summary>
    Paused
}