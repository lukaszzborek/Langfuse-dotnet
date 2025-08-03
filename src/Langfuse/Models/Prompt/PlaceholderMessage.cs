using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Represents a placeholder variable in a chat prompt template that will be replaced with actual values at runtime.
/// </summary>
public class PlaceholderMessage : ChatMessageWithPlaceholders
{
    /// <summary>
    ///     Type discriminator value for placeholder messages.
    /// </summary>
    public override string Type => "placeholder";

    /// <summary>
    ///     Name of the placeholder variable. Used to identify which value should be substituted during prompt resolution.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}