using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter that serializes enum values to kebab-case-lower format (e.g., "scores-numeric")
///     and can deserialize from various formats.
/// </summary>
/// <typeparam name="T">The enum type to convert</typeparam>
internal class KebabCaseLowerEnumConverter<T> : JsonConverter<T> where T : struct, Enum
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

        // Convert kebab-case to PascalCase
        var pascalCaseValue = ConvertKebabCaseToPascalCase(value);
        if (Enum.TryParse<T>(pascalCaseValue, false, out var convertedMatch))
        {
            return convertedMatch;
        }

        throw new JsonException($"Unable to convert '{value}' to enum {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ConvertPascalCaseToKebabCaseLower(value.ToString()));
    }

    /// <summary>
    ///     Converts PascalCase to kebab-case-lower
    /// </summary>
    private static string ConvertPascalCaseToKebabCaseLower(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var builder = new StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (i > 0 && char.IsUpper(c))
            {
                builder.Append('-');
            }

            builder.Append(char.ToLowerInvariant(c));
        }

        return builder.ToString();
    }

    /// <summary>
    ///     Converts kebab-case to PascalCase
    /// </summary>
    private static string ConvertKebabCaseToPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var parts = value.Split('-', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Select(part =>
            char.ToUpperInvariant(part[0]) + (part.Length > 1 ? part[1..].ToLowerInvariant() : string.Empty)));
    }
}