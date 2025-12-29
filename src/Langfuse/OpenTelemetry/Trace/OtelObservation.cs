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
    public Activity? Activity { get; }

    /// <summary>
    ///     Gets whether this observation has an active underlying Activity.
    /// </summary>
    public bool HasActivity => Activity != null;

    /// <summary>
    ///     Gets whether this observation has been marked as skipped.
    /// </summary>
    public bool IsSkipped
    {
        get
        {
            if (Activity == null)
            {
                return false;
            }

            return !Activity.IsAllDataRequested || !Activity.Recorded;
        }
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="OtelObservation" /> class.
    /// </summary>
    /// <param name="activity">The underlying Activity for this observation.</param>
    protected OtelObservation(Activity? activity)
    {
        Activity = activity;
    }

    /// <inheritdoc />
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
    ///     Marks this observation as skipped. Skipped observations will not be exported to Langfuse.
    ///     Use this when an operation turns out to be unnecessary (e.g., record already exists)
    ///     and you don't want to send the observation data.
    /// </summary>
    public void Skip()
    {
        if (Activity == null)
        {
            return;
        }

        Activity.IsAllDataRequested = false;
        Activity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;
    }

    /// <summary>
    ///     Ends the observation and disposes the activity.
    /// </summary>
    public virtual void EndObservation()
    {
        Activity?.Dispose();
    }
}