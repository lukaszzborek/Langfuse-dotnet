using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Deprecated: legacy Langfuse object kind used by trace and dataset evaluation rules.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<LegacyEvaluationObject>))]
public enum LegacyEvaluationObject
{
    /// <summary>Trace.</summary>
    Trace,

    /// <summary>Span observation.</summary>
    Span,

    /// <summary>Generation observation.</summary>
    Generation,

    /// <summary>Event observation.</summary>
    Event,

    /// <summary>Agent observation.</summary>
    Agent,

    /// <summary>Tool observation.</summary>
    Tool,

    /// <summary>Chain observation.</summary>
    Chain,

    /// <summary>Retriever observation.</summary>
    Retriever,

    /// <summary>Evaluator observation.</summary>
    Evaluator,

    /// <summary>Embedding observation.</summary>
    Embedding,

    /// <summary>Guardrail observation.</summary>
    Guardrail,

    /// <summary>Dataset item.</summary>
    Dataset_Item
}