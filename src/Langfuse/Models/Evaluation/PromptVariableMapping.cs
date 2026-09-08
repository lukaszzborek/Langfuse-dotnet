using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Stored prompt variable mapping returned by the API. Modern mappings populate <see cref="Variable" />,
///     <see cref="Source" /> and <see cref="JsonPath" />. Mappings from legacy trace or dataset rules additionally
///     carry <see cref="MappingType" /> = <c>legacy</c>, <see cref="LangfuseObject" /> and <see cref="ObjectName" />.
/// </summary>
public class PromptVariableMapping
{
    /// <summary>
    ///     Prompt variable name without braces.
    /// </summary>
    [JsonPropertyName("variable")]
    public required string Variable { get; init; }

    /// <summary>
    ///     Stored source field populating the variable, or null when the mapping is incomplete.
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; init; }

    /// <summary>
    ///     Optional JSONPath selector applied to the selected source.
    /// </summary>
    [JsonPropertyName("jsonPath")]
    public string? JsonPath { get; init; }

    /// <summary>
    ///     <c>legacy</c> for mappings of legacy trace or dataset rules, otherwise null.
    /// </summary>
    [JsonPropertyName("mappingType")]
    public string? MappingType { get; init; }

    /// <summary>
    ///     Legacy object kind selected as the mapping source. Only set for legacy mappings.
    /// </summary>
    [JsonPropertyName("langfuseObject")]
    public LegacyEvaluationObject? LangfuseObject { get; init; }

    /// <summary>
    ///     Observation name to match for legacy mappings, or null when <see cref="LangfuseObject" /> is
    ///     <see cref="LegacyEvaluationObject.Trace" /> or <see cref="LegacyEvaluationObject.Dataset_Item" />.
    /// </summary>
    [JsonPropertyName("objectName")]
    public string? ObjectName { get; init; }

    /// <summary>
    ///     Whether this mapping originates from a legacy trace or dataset rule.
    /// </summary>
    [JsonIgnore]
    public bool IsLegacy => MappingType == "legacy";
}