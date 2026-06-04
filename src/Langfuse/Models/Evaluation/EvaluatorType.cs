using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     The evaluator engine type. Currently only LLM-as-a-judge is supported.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<EvaluatorType>))]
public enum EvaluatorType
{
    /// <summary>
    ///     LLM-as-a-judge evaluator.
    /// </summary>
    Llm_As_Judge
}
