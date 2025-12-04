using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents a tool call observation.
/// </summary>
public class OtelToolCall : OtelObservation
{
    public OtelToolCall(OtelLangfuseTrace trace, Activity? activity, bool scoped)
        : base(trace, activity, scoped)
    {
    }

    /// <summary>
    ///     Sets the tool call arguments.
    /// </summary>
    public void SetArguments(object arguments)
    {
        GenAiActivityHelper.RecordToolCallArguments(Activity, arguments);
    }

    /// <summary>
    ///     Sets the tool call result.
    /// </summary>
    public void SetResult(object result)
    {
        GenAiActivityHelper.RecordToolCallResult(Activity, result);
    }
}