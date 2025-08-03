using System.Text.Json;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter that serializes all enum values to UPPERCASE strings and can deserialize from various formats.
///     This provides consistent enum serialization across the entire codebase.
/// </summary>
/// <typeparam name="T">The enum type to convert</typeparam>
internal class UppercaseEnumConverter<T> : JsonConverter<T> where T : struct, Enum
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

        if (Enum.TryParse<T>(value, false, out var exactMatch))
        {
            return exactMatch;
        }

        if (Enum.TryParse<T>(value, true, out var caseInsensitiveMatch))
        {
            return caseInsensitiveMatch;
        }

        var pascalCaseValue = ConvertToPascalCase(value);
        if (Enum.TryParse<T>(pascalCaseValue, false, out var convertedMatch))
        {
            return convertedMatch;
        }

        throw new JsonException($"Unable to convert '{value}' to enum {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToUpperInvariant());
    }

    /// <summary>
    ///     Converts various naming conventions to PascalCase for enum parsing
    /// </summary>
    /// <param name="value">The string value to convert</param>
    /// <returns>PascalCase version of the input</returns>
    private static string ConvertToPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (value.All(char.IsUpper))
        {
            return char.ToUpperInvariant(value[0]) + value[1..].ToLowerInvariant();
        }

        var parts = value.Split(['-', '_', '.'], StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length > 1)
        {
            return string.Concat(parts.Select(part =>
                char.ToUpperInvariant(part[0]) + (part.Length > 1 ? part[1..].ToLowerInvariant() : string.Empty)));
        }

        if (value.Length > 0 && char.IsLower(value[0]))
        {
            return char.ToUpperInvariant(value[0]) + value[1..];
        }

        return value;
    }
}