using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Base class for OpenTelemetry-based observations.
/// </summary>
public abstract class OtelObservation : IDisposable
{
    private bool _disposed;

    /// <summary>
    ///     The underlying Activity for this observation.
    /// </summary>
    protected Activity? Activity { get; }

    protected OtelObservation(Activity? activity)
    {
        Activity = activity;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        EndObservation();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Sets input for this observation.
    /// </summary>
    public void SetInput(object input)
    {
        GenAiActivityHelper.SetObservationInput(Activity, input);
    }

    /// <summary>
    ///     Sets output for this observation.
    /// </summary>
    public void SetOutput(object output)
    {
        GenAiActivityHelper.SetObservationOutput(Activity, output);
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
    ///     Set a custom tag to the underlying activity.
    /// </summary>
    public void SetTag(string key, object? value)
    {
        Activity?.SetTag(key, value);
    }

    /// <summary>
    ///     Ends the observation and disposes the activity.
    /// </summary>
    public virtual void EndObservation()
    {
        Activity?.Dispose();
    }
}