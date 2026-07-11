using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter for the polymorphic <see cref="ScoreSubjectV3" /> union that dispatches on the "kind"
///     discriminator. A custom converter is used because the discriminator is not guaranteed to be the first
///     property in the JSON object.
/// </summary>
internal class ScoreSubjectV3Converter : JsonConverter<ScoreSubjectV3>
{
    public override ScoreSubjectV3? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("kind", out var discriminator) ||
            discriminator.ValueKind != JsonValueKind.String)
        {
            throw new JsonException($"Missing or invalid 'kind' discriminator for {nameof(ScoreSubjectV3)}");
        }

        var kind = discriminator.GetString();
        return kind?.ToLowerInvariant() switch
        {
            "trace" => root.Deserialize<ScoreSubjectTraceV3>(options),
            "observation" => root.Deserialize<ScoreSubjectObservationV3>(options),
            "session" => root.Deserialize<ScoreSubjectSessionV3>(options),
            "experiment" => root.Deserialize<ScoreSubjectExperimentV3>(options),
            _ => throw new JsonException($"Unknown {nameof(ScoreSubjectV3)} kind '{kind}'")
        };
    }

    public override void Write(Utf8JsonWriter writer, ScoreSubjectV3 value, JsonSerializerOptions options)
    {
        // Serialize using the runtime type so the concrete properties (including the kind discriminator)
        // are written. The converter attribute on the abstract base is not inherited by the concrete types,
        // so this does not recurse.
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}