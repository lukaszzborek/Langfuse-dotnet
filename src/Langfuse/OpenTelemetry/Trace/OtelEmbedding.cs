using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents an embedding observation.
/// </summary>
public class OtelEmbedding : OtelObservation
{
    public OtelEmbedding(OtelLangfuseTrace trace, Activity? activity, bool scoped)
        : base(trace, activity, scoped)
    {
    }

    /// <summary>
    ///     Sets the text to embed as input.
    /// </summary>
    public void SetText(string text)
    {
        SetInput(text);
    }

    /// <summary>
    ///     Records the response for this embedding.
    /// </summary>
    public void SetResponse(GenAiResponse response)
    {
        GenAiActivityHelper.RecordResponse(Activity, response);
    }
}