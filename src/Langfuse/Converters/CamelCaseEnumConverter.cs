using System.Text.Json;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter that serializes enum values to camelCase strings (e.g., "input", "expectedOutput")
///     and deserializes them case-insensitively.
/// </summary>
/// <typeparam name="T">The enum type to convert</typeparam>
internal class CamelCaseEnumConverter<T> : JsonStringEnumConverter<T> where T : struct, Enum
{
    /// <summary>
    ///     Initializes the converter with the camelCase naming policy.
    /// </summary>
    public CamelCaseEnumConverter() : base(JsonNamingPolicy.CamelCase)
    {
    }
}