using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Feedback;

/// <summary>
///     Response returned after submitting feedback.
/// </summary>
public class SubmitFeedbackResponse
{
    /// <summary>
    ///     Correlation ID for the submitted feedback.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}
