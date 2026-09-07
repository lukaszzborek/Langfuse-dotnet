using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Converters;

/// <summary>
///     Deserializes a polymorphic evaluator union (<see cref="Evaluator" />, <see cref="EvaluatorVersion" />)
///     based on the "type" discriminator property, regardless of its position in the JSON object.
/// </summary>
internal class EvaluatorTypeConverter<TBase, TLlmAsJudge, TCode> : JsonConverter<TBase>
    where TBase : class
    where TLlmAsJudge : TBase
    where TCode : TBase
{
    public override TBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("type", out var typeProperty))
        {
            throw new JsonException($"{typeof(TBase).Name} JSON is missing the 'type' discriminator property");
        }

        var type = typeProperty.GetString();
        return type switch
        {
            "llm_as_judge" => root.Deserialize<TLlmAsJudge>(options),
            "code" => root.Deserialize<TCode>(options),
            _ => throw new JsonException($"Unknown evaluator type '{type}'")
        };
    }

    public override void Write(Utf8JsonWriter writer, TBase value, JsonSerializerOptions options)
    {
        // Serialize using the runtime type so all derived properties are written.
        // The [JsonConverter] attribute on the abstract base is not applied to derived types,
        // so this does not recurse.
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}