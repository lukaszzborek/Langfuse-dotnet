namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     Langfuse-specific attribute names for traces and observations.
/// </summary>
public static class LangfuseAttributes
{
    /// <summary>
    ///     Attribute key for the trace name.
    /// </summary>
    public const string TraceName = "langfuse.trace.name";

    /// <summary>
    ///     Attribute key for the trace identifier.
    /// </summary>
    public const string TraceId = "langfuse.trace.id";

    /// <summary>
    ///     Attribute key for the trace tags.
    /// </summary>
    public const string TraceTags = "langfuse.trace.tags";

    /// <summary>
    ///     Attribute key for the trace input data.
    /// </summary>
    public const string TraceInput = "langfuse.trace.input";

    /// <summary>
    ///     Attribute key for the trace output data.
    /// </summary>
    public const string TraceOutput = "langfuse.trace.output";

    /// <summary>
    ///     Attribute key indicating whether the trace is public.
    /// </summary>
    public const string TracePublic = "langfuse.trace.public";

    /// <summary>
    ///     Prefix for trace metadata attributes.
    /// </summary>
    public const string TraceMetadataPrefix = "langfuse.trace.metadata.";

    /// <summary>
    ///     Attribute key for the user identifier.
    /// </summary>
    public const string UserId = "langfuse.user.id";

    /// <summary>
    ///     Attribute key for the session identifier.
    /// </summary>
    public const string SessionId = "langfuse.session.id";

    /// <summary>
    ///     Attribute key for the release identifier.
    /// </summary>
    public const string Release = "langfuse.release";

    /// <summary>
    ///     Attribute key for the version identifier.
    /// </summary>
    public const string Version = "langfuse.version";

    /// <summary>
    ///     Attribute key for the environment name.
    /// </summary>
    public const string Environment = "langfuse.environment";

    /// <summary>
    ///     Attribute key for the observation type.
    /// </summary>
    public const string ObservationType = "langfuse.observation.type";

    /// <summary>
    ///     Attribute key for the observation level.
    /// </summary>
    public const string ObservationLevel = "langfuse.observation.level";

    /// <summary>
    ///     Attribute key for the observation status message.
    /// </summary>
    public const string ObservationStatusMessage = "langfuse.observation.status_message";

    /// <summary>
    ///     Attribute key for the observation input data.
    /// </summary>
    public const string ObservationInput = "langfuse.observation.input";

    /// <summary>
    ///     Attribute key for the observation output data.
    /// </summary>
    public const string ObservationOutput = "langfuse.observation.output";

    /// <summary>
    ///     Prefix for observation metadata attributes.
    /// </summary>
    public const string ObservationMetadataPrefix = "langfuse.observation.metadata.";

    /// <summary>
    ///     Attribute key for the observation model name.
    /// </summary>
    public const string ObservationModelName = "langfuse.observation.model.name";

    /// <summary>
    ///     Attribute key for the observation model parameters.
    /// </summary>
    public const string ObservationModelParameters = "langfuse.observation.model.parameters";

    /// <summary>
    ///     Attribute key for the observation usage details.
    /// </summary>
    public const string ObservationUsageDetails = "langfuse.observation.usage_details";

    /// <summary>
    ///     Attribute key for the observation cost details.
    /// </summary>
    public const string ObservationCostDetails = "langfuse.observation.cost_details";

    /// <summary>
    ///     Attribute key for the observation prompt name.
    /// </summary>
    public const string ObservationPromptName = "langfuse.observation.prompt.name";

    /// <summary>
    ///     Attribute key for the observation prompt version.
    /// </summary>
    public const string ObservationPromptVersion = "langfuse.observation.prompt.version";

    /// <summary>
    ///     Attribute key for the observation completion start time.
    /// </summary>
    public const string ObservationCompletionStartTime = "langfuse.observation.completion_start_time";

    /// <summary>
    ///     Value for generation observation type.
    /// </summary>
    public const string ObservationTypeGeneration = "generation";

    /// <summary>
    ///     Value for embedding observation type.
    /// </summary>
    public const string ObservationTypeEmbedding = "embedding";

    /// <summary>
    ///     Value for tool observation type.
    /// </summary>
    public const string ObservationTypeTool = "tool";

    /// <summary>
    ///     Value for agent observation type.
    /// </summary>
    public const string ObservationTypeAgent = "agent";

    /// <summary>
    ///     Value for event observation type.
    /// </summary>
    public const string ObservationTypeEvent = "event";

    /// <summary>
    ///     Value for span observation type.
    /// </summary>
    public const string ObservationTypeSpan = "span";
}