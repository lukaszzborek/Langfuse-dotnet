using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Trace body
/// </summary>
public class CreateTraceBody
{
    /// <summary>
    ///     Trace id
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    ///     Date of the trace
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTime? Timestamp { get; set; }

    /// <summary>
    ///     The input of the trace. Can be any JSON object
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     The id of the user that triggered the execution
    /// </summary>
    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    /// <summary>
    ///     The input of the trace. Can be any JSON object
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }

    /// <summary>
    ///     The output of the trace. Can be any JSON object
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; set; }

    /// <summary>
    ///     Used to group multiple traces into a session in Langfuse. Use your own session/thread identifier
    /// </summary>
    [JsonPropertyName("sessionId")]
    public string? SessionId { get; set; }

    /// <summary>
    ///     The release identifier of the current deployment. Used to understand how changes of different deployments affect
    ///     metrics. Useful in debugging
    /// </summary>
    [JsonPropertyName("release")]
    public string? Release { get; set; }

    /// <summary>
    ///     The version of the trace type. Used to understand how changes to the trace type affect metrics. Useful in debugging
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    ///     Additional metadata of the trace. Can be any JSON object. Metadata is merged when being updated via the API
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Tags are used to categorize or label traces. Traces can be filtered by tags in the UI and GET API. Tags can also be
    ///     changed in the UI. Tags are merged and never deleted via the API
    /// </summary>
    [JsonPropertyName("tags")]
    public string[]? Tags { get; set; }

    /// <summary>
    ///     You can make a trace public to share it via a public link. This allows others to view the trace without needing to
    ///     log in or be members of your Langfuse project
    /// </summary>
    [JsonPropertyName("public")]
    public bool? Public { get; set; }

    /// <summary>
    ///     The environment of the trace. Used to differentiate between production, staging, development, etc.
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }
}