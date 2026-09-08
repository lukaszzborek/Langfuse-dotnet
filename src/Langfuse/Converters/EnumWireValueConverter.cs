using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter that maps enum members to explicit wire strings declared via
///     <see cref="EnumMemberAttribute" />. Use for enums whose wire values contain spaces or symbols
///     (e.g. "&gt;=", "any of", "does not contain") that the case-transforming enum converters
///     (<see cref="LowercaseEnumConverter{T}" /> and friends) cannot produce.
/// </summary>
/// <typeparam name="T">The enum type to convert. Every member must carry an <see cref="EnumMemberAttribute" />.</typeparam>
internal class EnumWireValueConverter<T> : JsonConverter<T> where T : struct, Enum
{
    private static readonly Dictionary<T, string> ToWireMap = BuildToWireMap();
    private static readonly Dictionary<string, T> FromWireMap = BuildFromWireMap(ToWireMap);

    private static Dictionary<T, string> BuildToWireMap()
    {
        var map = new Dictionary<T, string>();
        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<EnumMemberAttribute>();
            if (attribute?.Value is null)
            {
                throw new InvalidOperationException(
                    $"Enum member {typeof(T).Name}.{field.Name} is missing an [EnumMember(Value = \"...\")] attribute");
            }

            map[(T)field.GetValue(null)!] = attribute.Value;
        }

        return map;
    }

    private static Dictionary<string, T> BuildFromWireMap(Dictionary<T, string> toWireMap)
    {
        var map = new Dictionary<string, T>();
        foreach (var (enumValue, wireValue) in toWireMap)
        {
            map[wireValue] = enumValue;
        }

        return map;
    }

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for enum {typeof(T).Name}");
        }

        var value = reader.GetString();
        if (value is not null && FromWireMap.TryGetValue(value, out var result))
        {
            return result;
        }

        throw new JsonException($"Unable to convert '{value}' to enum {typeof(T).Name}");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(ToWireMap[value]);
    }
}