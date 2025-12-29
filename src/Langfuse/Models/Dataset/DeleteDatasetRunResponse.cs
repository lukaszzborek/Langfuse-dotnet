using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

/// <summary>
///     Response returned after deleting a dataset run.
/// </summary>
public class DeleteDatasetRunResponse
{
    /// <summary>
    ///     Success message after deletion.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}