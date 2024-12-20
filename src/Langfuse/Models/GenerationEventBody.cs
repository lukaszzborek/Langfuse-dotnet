using System.Text.Json.Serialization;

namespace Langfuse.Models;

public class GenerationEventBody
{
    [JsonIgnore]
    public TimeProvider TimeProvider { private get; set; }
    
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }
    
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("startTime")]
    public DateTime? StartTime { get; set; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
    
    [JsonPropertyName("input")]
    public object? Input { get; set; }
    
    [JsonPropertyName("output")]
    public object? Output { get; set; }

    [JsonPropertyName("level")]
    public LangfuseLogLevel Level { get; set; } = LangfuseLogLevel.Default;
    
    [JsonPropertyName("statusMessage")]
    public string? StatusMessage { get; set; }

    [JsonPropertyName("parentObservationId")]
    public string? ParentObservationId { get; set; }
    
    [JsonPropertyName("version")]
    public string? Version { get; set; }
    
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }

    [JsonPropertyName("completionStartTime")]
    public DateTime? CompletionStartTime { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; }
    
    // modelParameters

    [JsonPropertyName("usage")]
    public object Usage { get; private set; }

    [JsonPropertyName("promptName")]
    public string? PromptName { get; set; }
    
    [JsonPropertyName("promptVersion")]
    public int? PromptVersion { get; set; }
    
    public void SetUsage(object usage)
    {
        Usage = usage;
    }
    
    public void SetOutput(object output)
    {
        Output = output;
        EndTime = TimeProvider.GetUtcNow().UtcDateTime;
    }
}