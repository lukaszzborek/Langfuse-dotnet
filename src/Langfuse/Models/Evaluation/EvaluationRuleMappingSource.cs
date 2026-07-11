using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Source field used to populate a prompt variable.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<EvaluationRuleMappingSource>))]
public enum EvaluationRuleMappingSource
{
    /// <summary>
    ///     The observation or experiment input payload.
    /// </summary>
    Input,

    /// <summary>
    ///     The observation or experiment output payload.
    /// </summary>
    Output,

    /// <summary>
    ///     The metadata object for the target.
    /// </summary>
    Metadata,

    /// <summary>
    ///     The tool calls recorded on the observation, as an array of {id, name, arguments, type, index} objects in the
    ///     order the model emitted them. Combine with jsonPath (for example $[*].name) to select parts of each call.
    /// </summary>
    Tool_Calls,

    /// <summary>
    ///     The experiment item's expected output. Only valid for target=experiment.
    /// </summary>
    Expected_Output,

    /// <summary>
    ///     The experiment item's metadata. Only valid for target=experiment.
    /// </summary>
    Experiment_Item_Metadata
}