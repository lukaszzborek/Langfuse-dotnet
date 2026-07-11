using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter for the polymorphic <see cref="ScoreV3" /> union that dispatches on the "dataType"
///     discriminator. A custom converter is used because the discriminator is not guaranteed to be the first
///     property in the JSON object.
/// </summary>
internal class ScoreV3Converter : JsonConverter<ScoreV3>
{
    public override ScoreV3? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("dataType", out var discriminator) ||
            discriminator.ValueKind != JsonValueKind.String)
        {
            throw new JsonException($"Missing or invalid 'dataType' discriminator for {nameof(ScoreV3)}");
        }

        var dataType = discriminator.GetString();
        return dataType?.ToUpperInvariant() switch
        {
            "NUMERIC" => root.Deserialize<NumericScoreV3>(options),
            "BOOLEAN" => root.Deserialize<BooleanScoreV3>(options),
            "CATEGORICAL" => root.Deserialize<CategoricalScoreV3>(options),
            "TEXT" => root.Deserialize<TextScoreV3>(options),
            "CORRECTION" => root.Deserialize<CorrectionScoreV3>(options),
            _ => throw new JsonException($"Unknown {nameof(ScoreV3)} dataType '{dataType}'")
        };
    }

    public override void Write(Utf8JsonWriter writer, ScoreV3 value, JsonSerializerOptions options)
    {
        // Serialize using the runtime type so the concrete properties (including the dataType discriminator)
        // are written. The converter attribute on the abstract base is not inherited by the concrete types,
        // so this does not recurse.
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}