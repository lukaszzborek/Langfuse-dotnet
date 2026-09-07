using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Source field used to populate an evaluator prompt variable from live data.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<PromptVariableMappingSource>))]
public enum PromptVariableMappingSource
{
    /// <summary>
    ///     The observation input payload.
    /// </summary>
    Input,

    /// <summary>
    ///     The observation output payload.
    /// </summary>
    Output,

    /// <summary>
    ///     The observation metadata object. Combine with a JSONPath to select one nested field.
    /// </summary>
    Metadata,

    /// <summary>
    ///     Tool calls recorded on the observation, as an array of <c>{id, name, arguments, type, index}</c>
    ///     objects in emission order. Combine with a JSONPath (for example <c>$[*].name</c>) to select parts.
    /// </summary>
    Tool_Calls,

    /// <summary>
    ///     The experiment item's expected output when the observation belongs to an experiment.
    /// </summary>
    Expected_Output,

    /// <summary>
    ///     The experiment item's metadata when the observation belongs to an experiment.
    /// </summary>
    Experiment_Item_Metadata
}