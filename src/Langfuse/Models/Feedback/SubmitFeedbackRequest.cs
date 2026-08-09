using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Feedback;

/// <summary>
///     Request for submitting explicit user-approved feedback about Langfuse skills, MCP tools, CLI, docs, or public
///     API. Do not include secrets, credentials, customer data, trace payloads, or unrelated use-case details.
/// </summary>
public class SubmitFeedbackRequest
{
    /// <summary>
    ///     Category of the thing the feedback is about.
    /// </summary>
    [JsonPropertyName("targetType")]
    public required FeedbackTargetType TargetType { get; set; }

    /// <summary>
    ///     The specific instance within targetType: the skill name, MCP tool name, CLI command, API endpoint path, or
    ///     docs page path (e.g. "queryMetrics", "/docs/mcp"). An identifier, not a sentence. Must be between 1 and 200
    ///     characters.
    /// </summary>
    [JsonPropertyName("target")]
    public required string Target { get; set; }

    /// <summary>
    ///     Concise feedback text approved by the user. Must be between 1 and 3000 characters.
    /// </summary>
    [JsonPropertyName("feedback")]
    public required string Feedback { get; set; }

    /// <summary>
    ///     Optional user-approved goal or use case they were trying to achieve. Must be between 1 and 1500 characters
    ///     when provided.
    /// </summary>
    [JsonPropertyName("goal")]
    public string? Goal { get; set; }

    /// <summary>
    ///     Optional HTTP(S) reference URL. Langfuse stores it as text for triage and does not fetch it.
    /// </summary>
    [JsonPropertyName("referenceUrl")]
    public string? ReferenceUrl { get; set; }
}
