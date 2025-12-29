using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Project;

/// <summary>
///     Response returned after deleting a project.
/// </summary>
public class ProjectDeletionResponse
{
    /// <summary>
    ///     Indicates whether the deletion was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    ///     Success or error message describing the result.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}