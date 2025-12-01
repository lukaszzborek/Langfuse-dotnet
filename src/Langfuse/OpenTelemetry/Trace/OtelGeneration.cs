using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents a generation (LLM call) observation.
/// </summary>
public class OtelGeneration : OtelObservation
{
    public OtelGeneration(OtelLangfuseTrace trace, Activity? activity, bool scoped)
        : base(trace, activity, scoped)
    {
    }

    /// <summary>
    ///     Records input messages for this generation.
    /// </summary>
    public void SetInputMessages(IEnumerable<GenAiMessage> messages)
    {
        GenAiActivityHelper.RecordInputMessages(Activity, messages);
        // Also propagate to trace
        Trace.PropagateInput(messages);
    }

    /// <summary>
    ///     Records a simple prompt as input.
    /// </summary>
    public void SetPrompt(string prompt)
    {
        GenAiActivityHelper.RecordPrompt(Activity, prompt);
        Trace.PropagateInput(prompt);
    }

    /// <summary>
    ///     Records the response for this generation.
    /// </summary>
    public void SetResponse(GenAiResponse response)
    {
        GenAiActivityHelper.RecordResponse(Activity, response);

        if (response.OutputMessages != null)
        {
            Trace.PropagateOutput(response.OutputMessages);
        }
        else if (response.Completion != null)
        {
            Trace.PropagateOutput(response.Completion);
        }
    }

    /// <summary>
    ///     Records a simple completion as output.
    /// </summary>
    public void SetCompletion(string completion)
    {
        GenAiActivityHelper.RecordCompletion(Activity, completion);
        Trace.PropagateOutput(completion);
    }

    /// <summary>
    ///     Sets prompt reference for this generation.
    /// </summary>
    public void SetPromptReference(string promptName, int? version = null)
    {
        GenAiActivityHelper.SetPromptReference(Activity, promptName, version);
    }

    /// <summary>
    ///     Records completion start time (Time to First Token).
    /// </summary>
    public void RecordCompletionStartTime(DateTimeOffset? startTime = null)
    {
        GenAiActivityHelper.RecordCompletionStartTime(Activity, startTime);
    }
}