using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Response returned after deleting an API key.
/// </summary>
public class ApiKeyDeletionResponse
{
    /// <summary>
    ///     Indicates whether the deletion was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }
}