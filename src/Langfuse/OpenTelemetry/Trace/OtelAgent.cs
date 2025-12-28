using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents an agent observation.
/// </summary>
public class OtelAgent : OtelObservation
{
    internal OtelAgent(Activity? activity) : base(activity)
    {
    }

    /// <summary>
    ///     Sets the data source ID for this agent.
    /// </summary>
    public void SetDataSource(string dataSourceId)
    {
        GenAiActivityHelper.SetDataSource(Activity, dataSourceId);
    }
}