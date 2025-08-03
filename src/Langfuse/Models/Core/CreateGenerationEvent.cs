using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Create generation event
/// </summary>
public class CreateGenerationEvent : IIngestionEvent
{
    /// <summary>
    ///     Create generation event body
    /// </summary>
    [JsonPropertyName("body")]
    public CreateGenerationEventBody Body { get; set; }

    /// <summary>
    ///     Event type
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = "generation-create";

    /// <summary>
    ///     Event ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    ///     Date of the event
    /// </summary>
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; }

    /// <summary>
    /// </summary>
    /// <param name="body">Generation event body</param>
    /// <param name="id">Generation event id</param>
    /// <param name="timestamp">Generation date</param>
    public CreateGenerationEvent(CreateGenerationEventBody body, string id, string timestamp)
    {
        Body = body;
        Id = id;
        Timestamp = timestamp;
    }

    /// <summary>
    /// </summary>
    /// <param name="body">Generation event body</param>
    /// <param name="timestamp">Generation date</param>
    public CreateGenerationEvent(CreateGenerationEventBody body, string timestamp)
        : this(body, Guid.NewGuid().ToString(), timestamp)
    {
    }
}