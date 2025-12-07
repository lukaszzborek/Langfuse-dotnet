using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents a span observation.
/// </summary>
public class OtelSpan : OtelObservation
{
    internal OtelSpan(Activity? activity) : base(activity)
    {
    }
}