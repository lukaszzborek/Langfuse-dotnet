using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Role of an evaluator prompt message.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<EvaluatorChatMessageRole>))]
public enum EvaluatorChatMessageRole
{
    /// <summary>
    ///     System message. Only allowed as the first message.
    /// </summary>
    System,

    /// <summary>
    ///     User message.
    /// </summary>
    User,

    /// <summary>
    ///     Assistant message.
    /// </summary>
    Assistant
}