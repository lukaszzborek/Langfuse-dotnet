using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents a generation (LLM call) observation.
/// </summary>
public class OtelGeneration : OtelObservation
{
    internal OtelGeneration(Activity? activity) : base(activity)
    {
    }

    /// <summary>
    ///     Records input messages for this generation.
    /// </summary>
    public void SetInputMessages(IEnumerable<GenAiMessage> messages)
    {
        GenAiActivityHelper.RecordInputMessages(Activity, messages);
    }

    /// <summary>
    ///     Records a simple prompt as input.
    /// </summary>
    public void SetPrompt(string prompt)
    {
        GenAiActivityHelper.RecordPrompt(Activity, prompt);
    }

    /// <summary>
    ///     Records the response for this generation.
    /// </summary>
    public void SetResponse(GenAiResponse response)
    {
        GenAiActivityHelper.RecordResponse(Activity, response);
    }

    /// <summary>
    ///     Records a simple completion as output.
    /// </summary>
    public void SetCompletion(string completion)
    {
        GenAiActivityHelper.RecordCompletion(Activity, completion);
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

    /// <summary>
    ///     Sets the temperature for this generation request.
    /// </summary>
    public void SetTemperature(double temperature)
    {
        Activity?.SetTag(GenAiAttributes.RequestTemperature, temperature);
    }

    /// <summary>
    ///     Sets the max tokens for this generation request.
    /// </summary>
    public void SetMaxTokens(int maxTokens)
    {
        Activity?.SetTag(GenAiAttributes.RequestMaxTokens, maxTokens);
    }

    /// <summary>
    ///     Sets the top P (nucleus sampling) for this generation request.
    /// </summary>
    public void SetTopP(double topP)
    {
        Activity?.SetTag(GenAiAttributes.RequestTopP, topP);
    }

    /// <summary>
    ///     Sets the top K for this generation request.
    /// </summary>
    public void SetTopK(int topK)
    {
        Activity?.SetTag(GenAiAttributes.RequestTopK, topK);
    }

    /// <summary>
    ///     Sets the frequency penalty for this generation request.
    /// </summary>
    public void SetFrequencyPenalty(double frequencyPenalty)
    {
        Activity?.SetTag(GenAiAttributes.RequestFrequencyPenalty, frequencyPenalty);
    }

    /// <summary>
    ///     Sets the presence penalty for this generation request.
    /// </summary>
    public void SetPresencePenalty(double presencePenalty)
    {
        Activity?.SetTag(GenAiAttributes.RequestPresencePenalty, presencePenalty);
    }
}