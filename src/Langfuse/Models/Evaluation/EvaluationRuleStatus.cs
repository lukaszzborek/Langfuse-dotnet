using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Effective runtime status of the evaluation rule.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<EvaluationRuleStatus>))]
public enum EvaluationRuleStatus
{
    /// <summary>
    ///     Enabled and currently runnable.
    /// </summary>
    Active,

    /// <summary>
    ///     Disabled by configuration.
    /// </summary>
    Inactive,

    /// <summary>
    ///     Enabled, but Langfuse has blocked execution until the underlying issue is resolved.
    /// </summary>
    Paused
}
