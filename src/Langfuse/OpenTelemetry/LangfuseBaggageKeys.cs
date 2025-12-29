namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     Baggage keys used for propagating Langfuse context across activities.
/// </summary>
public static class LangfuseBaggageKeys
{
    /// <summary>
    ///     Baggage key for the user identifier.
    /// </summary>
    public const string UserId = "langfuse.user_id";

    /// <summary>
    ///     Baggage key for the session identifier.
    /// </summary>
    public const string SessionId = "langfuse.session_id";

    /// <summary>
    ///     Baggage key for the version identifier.
    /// </summary>
    public const string Version = "langfuse.version";

    /// <summary>
    ///     Baggage key for the release identifier.
    /// </summary>
    public const string Release = "langfuse.release";

    /// <summary>
    ///     Baggage key for trace tags.
    /// </summary>
    public const string Tags = "langfuse.tags";
}