using System.Text.Json;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter that serializes all enum values to lowercase strings (e.g., "hourly", "daily", "weekly")
///     and can deserialize from various formats.
/// </summary>
/// <typeparam name="T">The enum type to convert</typeparam>
internal class LowercaseEnumConverter<T> : JsonConverter<T> where T : struct, Enum
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for enum {typeof(T).Name}");
        }

        var value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            throw new JsonException($"Cannot convert null or empty string to enum {typeof(T).Name}");
        }

        // Try exact match first
        if (Enum.TryParse<T>(value, false, out var exactMatch))
        {
            return exactMatch;
        }

        // Try case-insensitive match
        if (Enum.TryParse<T>(value, true, out var caseInsensitiveMatch))
        {
            return caseInsensitiveMatch;
        }

        throw new JsonException($"Unable to convert '{value}' to enum {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToLowerInvariant());
    }
}
