using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public abstract class BasePrompt
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("config")]
    public object Config { get; set; }

    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = new List<string>();

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();

    [JsonPropertyName("commitMessage")]
    public string? CommitMessage { get; set; }

    [JsonPropertyName("resolutionGraph")]
    public Dictionary<string, object>? ResolutionGraph { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ChatPrompt), "chat")]
[JsonDerivedType(typeof(TextPrompt), "text")]
public abstract class Prompt : BasePrompt
{
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}

public class ChatPrompt : Prompt
{
    public override string Type => "chat";

    [JsonPropertyName("prompt")]
    public List<ChatMessageWithPlaceholders> PromptMessages { get; set; } = new List<ChatMessageWithPlaceholders>();
}

public class TextPrompt : Prompt
{
    public override string Type => "text";

    [JsonPropertyName("prompt")]
    public string PromptText { get; set; }
}


[JsonConverter(typeof(ChatMessageConverter))]
public abstract class ChatMessageWithPlaceholders
{
    [JsonPropertyName("type")]
    public abstract string Type { get; }
}

public class ChatMessage : ChatMessageWithPlaceholders
{
    public override string Type => "chatmessage";

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public string Content { get; set; }
}

public class PlaceholderMessage : ChatMessageWithPlaceholders
{
    public override string Type => "placeholder";

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class PromptSchema
{
    [JsonPropertyName("schema")]
    public SchemaDefinition Schema { get; set; }
}

public class SchemaDefinition
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
}
