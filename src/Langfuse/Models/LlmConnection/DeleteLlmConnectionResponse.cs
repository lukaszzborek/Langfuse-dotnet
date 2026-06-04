using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.LlmConnection;

/// <summary>
///     Response returned after deleting an LLM connection.
/// </summary>
public class DeleteLlmConnectionResponse
{
    /// <summary>
    ///     Confirmation message about the deletion.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; init; }
}
