using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Converters;

/// <summary>
///     Serializes <see cref="EvaluatorChatPromptInput" /> as either a plain string or an array of chat messages.
/// </summary>
internal class EvaluatorChatPromptInputConverter : JsonConverter<EvaluatorChatPromptInput>
{
    public override EvaluatorChatPromptInput? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return EvaluatorChatPromptInput.FromText(reader.GetString()!);
        }

        var messages = JsonSerializer.Deserialize<EvaluatorChatMessage[]>(ref reader, options)
                       ?? throw new JsonException("Evaluator prompt must be a string or an array of chat messages");
        return EvaluatorChatPromptInput.FromMessages(messages);
    }

    public override void Write(Utf8JsonWriter writer, EvaluatorChatPromptInput value, JsonSerializerOptions options)
    {
        if (value.Text != null)
        {
            writer.WriteStringValue(value.Text);
            return;
        }

        JsonSerializer.Serialize(writer, value.Messages, options);
    }
}