using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Skill;

/// <summary>
///     A file in a skill version manifest.
/// </summary>
public class SkillFile
{
    /// <summary>
    ///     Unique identifier of the file.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Path of the file within the skill.
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the blob that stores the file content.
    /// </summary>
    [JsonPropertyName("blobId")]
    public string BlobId { get; set; } = string.Empty;

    /// <summary>
    ///     Canonical base64-encoded SHA-256 hash of the file content.
    /// </summary>
    [JsonPropertyName("sha256Hash")]
    public string Sha256Hash { get; set; } = string.Empty;

    /// <summary>
    ///     MIME content type of the file.
    /// </summary>
    [JsonPropertyName("contentType")]
    public string ContentType { get; set; } = string.Empty;

    /// <summary>
    ///     Size of the file content in bytes.
    /// </summary>
    [JsonPropertyName("contentLength")]
    public long ContentLength { get; set; }

    /// <summary>
    ///     UTF-8 text content, included when fetching a skill with includeContent enabled.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
