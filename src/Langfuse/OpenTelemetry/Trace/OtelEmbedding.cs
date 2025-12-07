using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents an embedding observation.
/// </summary>
public class OtelEmbedding : OtelObservation
{
    internal OtelEmbedding(Activity? activity) : base(activity)
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

    /// <summary>
    ///     Sets the dimensions for this embedding request.
    /// </summary>
    public void SetDimensions(int dimensions)
    {
        Activity?.SetTag(GenAiAttributes.EmbeddingsDimensionCount, dimensions);
    }
}