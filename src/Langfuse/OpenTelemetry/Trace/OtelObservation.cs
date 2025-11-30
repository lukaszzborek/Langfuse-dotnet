using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Base class for OpenTelemetry-based observations.
/// </summary>
public abstract class OtelObservation : IDisposable
{
    protected readonly Activity? Activity;
    protected readonly bool Scoped;
    protected readonly OtelLangfuseTrace Trace;
    private bool _disposed;

    protected OtelObservation(OtelLangfuseTrace trace, Activity? activity, bool scoped)
    {
        Trace = trace;
        Activity = activity;
        Scoped = scoped;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        End();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Sets input for this observation. Also propagates to trace if this is the first input.
    /// </summary>
    public void SetInput(object input)
    {
        GenAiActivityHelper.SetObservationInput(Activity, input);
        Trace.PropagateInput(input);
    }

    /// <summary>
    ///     Sets output for this observation. Also propagates to trace.
    /// </summary>
    public void SetOutput(object output)
    {
        GenAiActivityHelper.SetObservationOutput(Activity, output);
        Trace.PropagateOutput(output);
    }

    /// <summary>
    ///     Sets metadata for this observation.
    /// </summary>
    public void SetMetadata(string key, object value)
    {
        GenAiActivityHelper.SetObservationMetadata(Activity, key, value);
    }

    /// <summary>
    ///     Sets the observation level.
    /// </summary>
    public void SetLevel(LangfuseObservationLevel level)
    {
        GenAiActivityHelper.SetObservationLevel(Activity, level);
    }

    /// <summary>
    ///     Ends the observation.
    /// </summary>
    public virtual void End()
    {
        if (Scoped)
        {
            Trace.PopActivity();
        }

        Activity?.Dispose();
    }
}