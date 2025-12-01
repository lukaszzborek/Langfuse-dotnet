using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents an agent observation.
/// </summary>
public class OtelAgent : OtelObservation
{
    public OtelAgent(OtelLangfuseTrace trace, Activity? activity, bool scoped)
        : base(trace, activity, scoped)
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