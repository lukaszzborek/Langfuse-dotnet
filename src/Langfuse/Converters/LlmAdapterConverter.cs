using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.LlmConnection;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter that serializes LlmAdapter enum values to their specific string representations
///     (lowercase with hyphens for Google adapters) and can deserialize from various formats.
/// </summary>
internal class LlmAdapterConverter : JsonConverter<LlmAdapter>
{
    private static readonly Dictionary<LlmAdapter, string> EnumToString = new()
    {
        { LlmAdapter.Anthropic, "anthropic" },
        { LlmAdapter.OpenAi, "openai" },
        { LlmAdapter.Azure, "azure" },
        { LlmAdapter.Bedrock, "bedrock" },
        { LlmAdapter.GoogleVertexAi, "google-vertex-ai" },
        { LlmAdapter.GoogleAiStudio, "google-ai-studio" }
    };

    private static readonly Dictionary<string, LlmAdapter> StringToEnum =
        EnumToString.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    public override LlmAdapter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for enum {nameof(LlmAdapter)}");
        }

        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            throw new JsonException($"Cannot convert null or empty string to enum {nameof(LlmAdapter)}");
        }

        if (StringToEnum.TryGetValue(value, out var enumValue))
        {
            return enumValue;
        }

        // Try parsing as enum name (case-insensitive fallback)
        if (Enum.TryParse<LlmAdapter>(value, true, out var parsedValue))
        {
            return parsedValue;
        }

        throw new JsonException($"Unable to convert '{value}' to enum {nameof(LlmAdapter)}");
    }

    public override void Write(Utf8JsonWriter writer, LlmAdapter value, JsonSerializerOptions options)
    {
        if (EnumToString.TryGetValue(value, out var stringValue))
        {
            writer.WriteStringValue(stringValue);
        }
        else
        {
            // Fallback to lowercase enum name
            writer.WriteStringValue(value.ToString().ToLowerInvariant());
        }
    }
}