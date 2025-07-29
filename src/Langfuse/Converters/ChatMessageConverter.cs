using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models;

namespace zborek.Langfuse.Converters;

internal class ChatMessageConverter : JsonConverter<ChatMessageWithPlaceholders>
{
    public override ChatMessageWithPlaceholders Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        
        // Check if type property exists
        if (root.TryGetProperty("type", out var typeElement))
        {
            var typeValue = typeElement.GetString() ?? "chatmessage";

            return typeValue switch
            {
                "placeholder" => JsonSerializer.Deserialize<PlaceholderMessage>(root.GetRawText(), options),
                "chatmessage" => JsonSerializer.Deserialize<ChatMessage>(root.GetRawText(), options),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        // No type property - default to ChatMessage
        return JsonSerializer.Deserialize<ChatMessage>(root.GetRawText(), options);
    }

    public override void Write(Utf8JsonWriter writer, ChatMessageWithPlaceholders value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}