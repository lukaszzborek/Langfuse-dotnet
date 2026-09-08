using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Evaluation;

/// <summary>
///     Evaluator prompt input: either a user prompt string shortcut or an ordered list of chat messages.
///     A system message is only allowed as the first message. Implicitly convertible from
///     <see cref="string" /> and <see cref="EvaluatorChatMessage" /> arrays.
/// </summary>
[JsonConverter(typeof(EvaluatorChatPromptInputConverter))]
public class EvaluatorChatPromptInput
{
    /// <summary>
    ///     User prompt string shortcut, or null when <see cref="Messages" /> is used.
    /// </summary>
    public string? Text { get; }

    /// <summary>
    ///     Ordered chat messages, or null when <see cref="Text" /> is used.
    /// </summary>
    public EvaluatorChatMessage[]? Messages { get; }

    private EvaluatorChatPromptInput(string? text, EvaluatorChatMessage[]? messages)
    {
        Text = text;
        Messages = messages;
    }

    /// <summary>
    ///     Creates a prompt input from a user prompt string.
    /// </summary>
    public static EvaluatorChatPromptInput FromText(string text)
    {
        return new EvaluatorChatPromptInput(text ?? throw new ArgumentNullException(nameof(text)), null);
    }

    /// <summary>
    ///     Creates a prompt input from an ordered list of chat messages.
    /// </summary>
    public static EvaluatorChatPromptInput FromMessages(params EvaluatorChatMessage[] messages)
    {
        return new EvaluatorChatPromptInput(null, messages ?? throw new ArgumentNullException(nameof(messages)));
    }

    /// <summary>
    ///     Converts a user prompt string into a prompt input.
    /// </summary>
    public static implicit operator EvaluatorChatPromptInput(string text)
    {
        return FromText(text);
    }

    /// <summary>
    ///     Converts chat messages into a prompt input.
    /// </summary>
    public static implicit operator EvaluatorChatPromptInput(EvaluatorChatMessage[] messages)
    {
        return FromMessages(messages);
    }
}