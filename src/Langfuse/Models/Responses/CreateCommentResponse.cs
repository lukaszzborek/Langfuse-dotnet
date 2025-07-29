using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Responses;

/// <summary>
///     Response from creating a comment
/// </summary>
public class CreateCommentResponse
{
    /// <summary>
    ///     The id of the created comment in Langfuse
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}