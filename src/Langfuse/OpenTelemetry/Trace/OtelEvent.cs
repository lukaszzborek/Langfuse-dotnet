using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents an event observation.
/// </summary>
public class OtelEvent : OtelObservation
{
    internal OtelEvent(Activity? activity) : base(activity)
    {
    }
}