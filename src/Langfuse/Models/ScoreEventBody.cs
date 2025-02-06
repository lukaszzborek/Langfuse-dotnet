using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class ScoreEventBody
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("traceId")]
    public string TraceId { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("value")]
    public object Value { get; set; }
    
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
    
    [JsonPropertyName("dataType")]
    public ScoreDataType DataType { get; set; }
    
    [JsonPropertyName("configId")]
    public string? ConfigId { get; set; }
}

// public class ScoreEventNumber
// {
//     public double Type { get; set; }
// }
//
// public class ScoreEventString
// {
//     public string Type { get; set; }
// }