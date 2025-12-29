using System.Text.Json;
using System.Text.Json.Serialization;
using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Observation;

/// <summary>
///     Represents an observation - a specific activity within a trace that can be a span (duration-based operation),
///     generation (AI model interaction), or event (discrete point-in-time occurrence).
/// </summary>
public class ObservationModel
{
    /// <summary>
    ///     Unique identifier of the observation
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the observation
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    ///     Type of observation. Can be SPAN (duration-based operations), GENERATION (AI model interactions), or EVENT
    ///     (discrete point-in-time occurrences).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the trace this observation belongs to. Links the observation to its parent trace context.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    ///     ID of the parent observation (if any). Creates hierarchical relationships between observations within a trace.
    /// </summary>
    [JsonPropertyName("parentObservationId")]
    public string? ParentObservationId { get; set; }

    /// <summary>
    ///     Start time of the observation
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    /// <summary>
    ///     End time of the observation
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTime? EndTime { get; set; }

    /// <summary>
    ///     The time at which the completion started (streaming). Set for generation observations.
    /// </summary>
    [JsonPropertyName("completionStartTime")]
    public DateTime? CompletionStartTime { get; set; }

    /// <summary>
    ///     Input data for the observation. Can be any JSON object representing the request, parameters, or initial data.
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; set; }

    /// <summary>
    ///     Output data for the observation. Can be any JSON object representing the result, response, or final data.
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; set; }

    /// <summary>
    ///     Metadata associated with the observation. Additional context and custom properties stored as JSON.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Level of the observation indicating importance or severity. Used for filtering and highlighting in the UI.
    /// </summary>
    [JsonPropertyName("level")]
    public LangfuseLogLevel Level { get; set; }

    /// <summary>
    ///     Status message of the observation. Additional field providing context about the observation state, such as error
    ///     messages.
    /// </summary>
    [JsonPropertyName("statusMessage")]
    public string? StatusMessage { get; set; }

    /// <summary>
    ///     Version of the observation implementation. Used to understand how changes to the observation type affect metrics.
    /// </summary>
    [JsonPropertyName("version")]
    public string? Version { get; set; }

    /// <summary>
    ///     Model name used for this observation (applicable to generation-type observations). Identifies which AI model was
    ///     used.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    ///     Model parameters used for this observation (applicable to generation-type observations). Configuration settings
    ///     passed to the AI model.
    /// </summary>
    [JsonPropertyName("modelParameters")]
    public object? ModelParameters { get; set; }

    /// <summary>
    ///     Usage statistics for this observation (applicable to generation-type observations). Contains token counts and cost
    ///     information.
    /// </summary>
    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }

    /// <summary>
    ///     Detailed usage information by type. Keys are usage types (e.g., "input_tokens", "output_tokens"),
    ///     values are counts.
    /// </summary>
    [JsonPropertyName("usageDetails")]
    public Dictionary<string, int>? UsageDetails { get; set; }

    /// <summary>
    ///     Detailed cost information by type. Keys are cost types, values are costs in USD.
    /// </summary>
    [JsonPropertyName("costDetails")]
    public Dictionary<string, double>? CostDetails { get; set; }

    /// <summary>
    ///     ID of the prompt used for this observation.
    /// </summary>
    [JsonPropertyName("promptId")]
    public string? PromptId { get; set; }

    /// <summary>
    ///     Name of the prompt used for this observation.
    /// </summary>
    [JsonPropertyName("promptName")]
    public string? PromptName { get; set; }

    /// <summary>
    ///     Version number of the prompt used for this observation.
    /// </summary>
    [JsonPropertyName("promptVersion")]
    public int? PromptVersion { get; set; }

    /// <summary>
    ///     ID of the model definition used for this observation.
    /// </summary>
    [JsonPropertyName("modelId")]
    public string? ModelId { get; set; }

    /// <summary>
    ///     Price per input unit in USD.
    /// </summary>
    [JsonPropertyName("inputPrice")]
    public double? InputPrice { get; set; }

    /// <summary>
    ///     Price per output unit in USD.
    /// </summary>
    [JsonPropertyName("outputPrice")]
    public double? OutputPrice { get; set; }

    /// <summary>
    ///     Total price per unit in USD.
    /// </summary>
    [JsonPropertyName("totalPrice")]
    public double? TotalPrice { get; set; }

    /// <summary>
    ///     Latency of the observation in seconds.
    /// </summary>
    [JsonPropertyName("latency")]
    public double? Latency { get; set; }

    /// <summary>
    ///     Time to first token in seconds (for streaming generations).
    /// </summary>
    [JsonPropertyName("timeToFirstToken")]
    public double? TimeToFirstToken { get; set; }

    /// <summary>
    ///     The environment of the observation (e.g., production, staging, development).
    /// </summary>
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    /// <summary>
    ///     Creation timestamp
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Last update timestamp
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    ///     Parses the metadata as an OpenTelemetry-structured metadata object.
    ///     Returns null if metadata is null or cannot be parsed as OTEL metadata.
    /// </summary>
    /// <returns>The parsed OTEL metadata, or null.</returns>
    public ObservationOtelMetadata? GetOtelMetadata()
    {
        return GetMetadataAs<ObservationOtelMetadata>();
    }

    /// <summary>
    ///     Parses the metadata as the specified type.
    ///     Handles both JsonElement (from deserialization) and already-typed objects.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the metadata to.</typeparam>
    /// <returns>The parsed metadata, or default if metadata is null or cannot be parsed.</returns>
    public T? GetMetadataAs<T>() where T : class
    {
        if (Metadata is null)
        {
            return null;
        }

        if (Metadata is T typed)
        {
            return typed;
        }

        if (Metadata is JsonElement jsonElement)
        {
            try
            {
                return jsonElement.Deserialize<T>();
            }
            catch (JsonException)
            {
                return null;
            }
        }

        // Try to serialize and deserialize for other object types
        try
        {
            var json = JsonSerializer.Serialize(Metadata);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}