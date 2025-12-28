using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Prompt;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter that handles polymorphic deserialization of PromptModel based on the "type" field.
/// </summary>
internal class PromptModelConverter : JsonConverter<PromptModel>
{
    public override PromptModel? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Get the type discriminator
        if (!root.TryGetProperty("type", out var typeElement))
        {
            throw new JsonException("Missing 'type' property in prompt response");
        }

        var typeValue = typeElement.GetString()?.ToLowerInvariant();

        // Create a new options without this converter to avoid infinite recursion
        var innerOptions = new JsonSerializerOptions(options);
        innerOptions.Converters.Remove(this);

        return typeValue switch
        {
            "text" => JsonSerializer.Deserialize<TextPrompt>(root.GetRawText(), innerOptions),
            "chat" => JsonSerializer.Deserialize<ChatPrompt>(root.GetRawText(), innerOptions),
            _ => throw new JsonException($"Unknown prompt type: {typeValue}")
        };
    }

    public override void Write(Utf8JsonWriter writer, PromptModel value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
