using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class DeleteDatasetItemResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}