using System.Text.Json;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter that serializes enum values to snake_case_lower format (e.g., model_parameters)
///     and can deserialize from various formats.
/// </summary>
/// <typeparam name="T">The enum type to convert</typeparam>
internal class SnakeCaseLowerEnumConverter<T> : SnakeCaseUpperEnumConverter<T> where T : struct, Enum
{
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ConvertPascalCaseToSnakeCaseUpper(value.ToString()).ToLowerInvariant());
    }
}
