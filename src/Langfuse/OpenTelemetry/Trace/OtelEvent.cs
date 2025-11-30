using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents an event observation.
/// </summary>
public class OtelEvent : IDisposable
{
    private readonly Activity? _activity;
    private bool _disposed;

    public OtelEvent(OtelLangfuseTrace trace, Activity? activity)
    {
        _activity = activity;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _activity?.Dispose();
        GC.SuppressFinalize(this);
    }
}