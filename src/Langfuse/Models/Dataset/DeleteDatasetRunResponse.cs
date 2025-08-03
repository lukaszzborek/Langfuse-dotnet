using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

public class DeleteDatasetRunResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}