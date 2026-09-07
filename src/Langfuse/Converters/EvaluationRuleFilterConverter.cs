using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Evaluation;

namespace zborek.Langfuse.Converters;

/// <summary>
///     Deserializes the polymorphic <see cref="EvaluationRuleFilter" /> write union based on the "type"
///     discriminator property, regardless of its position in the JSON object.
/// </summary>
internal class EvaluationRuleFilterConverter : JsonConverter<EvaluationRuleFilter>
{
    public override EvaluationRuleFilter? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("type", out var typeProperty))
        {
            throw new JsonException(
                $"{nameof(EvaluationRuleFilter)} JSON is missing the 'type' discriminator property");
        }

        var type = typeProperty.GetString();
        return type switch
        {
            "datetime" => root.Deserialize<DateTimeEvaluationRuleFilter>(options),
            "string" => root.Deserialize<StringEvaluationRuleFilter>(options),
            "number" => root.Deserialize<NumberEvaluationRuleFilter>(options),
            "stringOptions" => root.Deserialize<StringOptionsEvaluationRuleFilter>(options),
            "categoryOptions" => root.Deserialize<CategoryOptionsEvaluationRuleFilter>(options),
            "arrayOptions" => root.Deserialize<ArrayOptionsEvaluationRuleFilter>(options),
            "stringObject" => root.Deserialize<StringObjectEvaluationRuleFilter>(options),
            "numberObject" => root.Deserialize<NumberObjectEvaluationRuleFilter>(options),
            "boolean" => root.Deserialize<BooleanEvaluationRuleFilter>(options),
            "null" => root.Deserialize<NullEvaluationRuleFilter>(options),
            _ => throw new JsonException($"Unknown evaluation rule filter type '{type}'")
        };
    }

    public override void Write(Utf8JsonWriter writer, EvaluationRuleFilter value, JsonSerializerOptions options)
    {
        // Serialize using the runtime type. The converter attribute on the abstract base is not inherited
        // by the concrete types, so this does not recurse.
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}