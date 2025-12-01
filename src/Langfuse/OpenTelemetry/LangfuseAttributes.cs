namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     Langfuse-specific attribute names for traces and observations.
/// </summary>
public static class LangfuseAttributes
{
    // Trace-level attributes
    public const string TraceName = "langfuse.trace.name";
    public const string TraceId = "langfuse.trace.id";
    public const string TraceTags = "langfuse.trace.tags";
    public const string TraceInput = "langfuse.trace.input";
    public const string TraceOutput = "langfuse.trace.output";
    public const string TracePublic = "langfuse.trace.public";
    public const string TraceMetadataPrefix = "langfuse.trace.metadata.";

    // User and session
    public const string UserId = "langfuse.user.id";
    public const string SessionId = "langfuse.session.id";

    // Deployment
    public const string Release = "langfuse.release";
    public const string Version = "langfuse.version";
    public const string Environment = "langfuse.environment";

    // Observation-level attributes
    public const string ObservationType = "langfuse.observation.type";
    public const string ObservationLevel = "langfuse.observation.level";
    public const string ObservationStatusMessage = "langfuse.observation.status_message";
    public const string ObservationInput = "langfuse.observation.input";
    public const string ObservationOutput = "langfuse.observation.output";
    public const string ObservationMetadataPrefix = "langfuse.observation.metadata.";
    public const string ObservationModelName = "langfuse.observation.model.name";
    public const string ObservationModelParameters = "langfuse.observation.model.parameters";
    public const string ObservationUsageDetails = "langfuse.observation.usage_details";
    public const string ObservationCostDetails = "langfuse.observation.cost_details";
    public const string ObservationPromptName = "langfuse.observation.prompt.name";
    public const string ObservationPromptVersion = "langfuse.observation.prompt.version";
    public const string ObservationCompletionStartTime = "langfuse.observation.completion_start_time";

    // Observation types
    public const string ObservationTypeGeneration = "generation";
    public const string ObservationTypeEmbedding = "embedding";
    public const string ObservationTypeTool = "tool";
    public const string ObservationTypeAgent = "agent";
    public const string ObservationTypeEvent = "event";
    public const string ObservationTypeSpan = "span";
}