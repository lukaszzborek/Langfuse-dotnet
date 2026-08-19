using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Model;

namespace zborek.Langfuse.Converters;

/// <summary>
///     A JSON converter for the polymorphic <see cref="PricingTierCondition" /> union. The union has no
///     discriminator property, so the concrete type is detected by shape: usage conditions carry
///     <c>usageDetailPattern</c>, attribute conditions carry <c>source</c>.
/// </summary>
internal class PricingTierConditionConverter : JsonConverter<PricingTierCondition>
{
    public override PricingTierCondition? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (root.TryGetProperty("usageDetailPattern", out _))
        {
            return root.Deserialize<PricingTierUsageCondition>(options);
        }

        if (root.TryGetProperty("source", out _))
        {
            return root.Deserialize<PricingTierAttributeCondition>(options);
        }

        throw new JsonException(
            $"Unable to determine {nameof(PricingTierCondition)} type: expected 'usageDetailPattern' or 'source' property");
    }

    public override void Write(Utf8JsonWriter writer, PricingTierCondition value, JsonSerializerOptions options)
    {
        // Serialize using the runtime type. The converter attribute on the abstract base is not inherited
        // by the concrete types, so this does not recurse.
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
