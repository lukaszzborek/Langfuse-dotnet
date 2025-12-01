using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents a span observation.
/// </summary>
public class OtelSpan : OtelObservation
{
    public OtelSpan(OtelLangfuseTrace trace, Activity? activity, bool scoped)
        : base(trace, activity, scoped)
    {
    }
}