namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     Baggage keys used for propagating Langfuse context across activities.
/// </summary>
public static class LangfuseBaggageKeys
{
    public const string UserId = "langfuse.user_id";
    public const string SessionId = "langfuse.session_id";
    public const string Version = "langfuse.version";
    public const string Release = "langfuse.release";
    public const string Tags = "langfuse.tags";
}
