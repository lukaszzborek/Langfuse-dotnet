using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Converters;

/// <summary>
///     Deserializes a polymorphic <see cref="EvaluatorOutputDefinition" /> union based on the "dataType"
///     discriminator property, regardless of its position in the JSON object.
/// </summary>
internal class EvaluatorOutputDefinitionConverter : JsonConverter<EvaluatorOutputDefinition>
{
    public override EvaluatorOutputDefinition? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("dataType", out var dataTypeProperty))
        {
            throw new JsonException(
                $"{nameof(EvaluatorOutputDefinition)} JSON is missing the 'dataType' discriminator property");
        }

        var dataType = dataTypeProperty.GetString();
        return dataType switch
        {
            "NUMERIC" => root.Deserialize<NumericEvaluatorOutputDefinition>(options),
            "BOOLEAN" => root.Deserialize<BooleanEvaluatorOutputDefinition>(options),
            "CATEGORICAL" => root.Deserialize<CategoricalEvaluatorOutputDefinition>(options),
            _ => throw new JsonException($"Unknown evaluator output data type '{dataType}'")
        };
    }

    public override void Write(Utf8JsonWriter writer, EvaluatorOutputDefinition value, JsonSerializerOptions options)
    {
        // Serialize using the runtime type so all derived properties are written.
        // The [JsonConverter] attribute on the abstract base is not applied to derived types,
        // so this does not recurse.
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}