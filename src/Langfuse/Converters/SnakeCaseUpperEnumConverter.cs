using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter that serializes enum values to SNAKE_CASE_UPPER format (e.g., S3_COMPATIBLE)
///     and can deserialize from various formats.
/// </summary>
/// <typeparam name="T">The enum type to convert</typeparam>
internal class SnakeCaseUpperEnumConverter<T> : JsonConverter<T> where T : struct, Enum
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

        // Convert SNAKE_CASE_UPPER to PascalCase
        var pascalCaseValue = ConvertSnakeCaseToPascalCase(value);
        if (Enum.TryParse<T>(pascalCaseValue, false, out var convertedMatch))
        {
            return convertedMatch;
        }

        throw new JsonException($"Unable to convert '{value}' to enum {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        var pascalCase = value.ToString();
        var snakeCaseUpper = ConvertPascalCaseToSnakeCaseUpper(pascalCase);
        writer.WriteStringValue(snakeCaseUpper);
    }

    /// <summary>
    ///     Converts PascalCase to SNAKE_CASE_UPPER
    /// </summary>
    private static string ConvertPascalCaseToSnakeCaseUpper(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var builder = new StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (i > 0 && char.IsUpper(c) && (i == value.Length - 1 || !char.IsUpper(value[i - 1]) || (i < value.Length - 1 && !char.IsUpper(value[i + 1]))))
            {
                builder.Append('_');
            }
            builder.Append(char.ToUpperInvariant(c));
        }

        return builder.ToString();
    }

    /// <summary>
    ///     Converts SNAKE_CASE_UPPER to PascalCase
    /// </summary>
    private static string ConvertSnakeCaseToPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var parts = value.Split('_', StringSplitOptions.RemoveEmptyEntries);
        return string.Concat(parts.Select(part =>
            char.ToUpperInvariant(part[0]) + (part.Length > 1 ? part[1..].ToLowerInvariant() : string.Empty)));
    }
}
