using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Connects one prompt variable to data from an observation or experiment. Manual mappings are used for
///     LLM-as-a-judge evaluators; code evaluators use a fixed runtime mapping managed by Langfuse.
/// </summary>
/// <remarks>
///     Read the evaluator <c>variables</c> array and add exactly one mapping per variable, using the variable
///     name exactly as returned (without braces). Invalid, missing, or duplicate mappings return a validation
///     error, as do malformed JSONPath expressions.
/// </remarks>
public class PromptVariableMappingInput
{
    /// <summary>
    ///     Prompt variable name without braces. For <c>Judge {{input}} against {{output}}</c> use
    ///     <c>input</c> and <c>output</c>.
    /// </summary>
    [JsonPropertyName("variable")]
    public required string Variable { get; init; }

    /// <summary>
    ///     Source field that should populate the prompt variable.
    /// </summary>
    [JsonPropertyName("source")]
    public required PromptVariableMappingSource Source { get; init; }

    /// <summary>
    ///     Optional JSONPath selector applied to the selected source before it is passed to the prompt. Must
    ///     start with <c>$</c> and be a valid JSONPath expression. Most useful with <see cref="Source" /> =
    ///     <see cref="PromptVariableMappingSource.Metadata" />.
    /// </summary>
    [JsonPropertyName("jsonPath")]
    public string? JsonPath { get; init; }
}