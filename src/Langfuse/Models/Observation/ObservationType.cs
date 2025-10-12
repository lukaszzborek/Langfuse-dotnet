using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Observation;

/// <summary>
///     The type of observation
/// </summary>
[JsonConverter(typeof(UppercaseEnumConverter<ObservationType>))]
public enum ObservationType
{
    /// <summary>
    ///     Span observation type
    /// </summary>
    Span,

    /// <summary>
    ///     Generation observation type
    /// </summary>
    Generation,

    /// <summary>
    ///     Event observation type
    /// </summary>
    Event,

    /// <summary>
    ///     Agent observation type
    /// </summary>
    Agent,

    /// <summary>
    ///     Tool observation type
    /// </summary>
    Tool,

    /// <summary>
    ///     Chain observation type
    /// </summary>
    Chain,

    /// <summary>
    ///     Retriever observation type
    /// </summary>
    Retriever,

    /// <summary>
    ///     Evaluator observation type
    /// </summary>
    Evaluator,

    /// <summary>
    ///     Embedding observation type
    /// </summary>
    Embedding,

    /// <summary>
    ///     Guardrail observation type
    /// </summary>
    Guardrail
}