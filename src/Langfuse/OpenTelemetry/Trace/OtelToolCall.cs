using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents a tool call observation.
/// </summary>
public class OtelToolCall : OtelObservation
{
    internal OtelToolCall(Activity? activity) : base(activity)
    {
    }

    /// <summary>
    ///     Sets the tool call arguments.
    /// </summary>
    public void SetArguments(object arguments)
    {
        GenAiActivityHelper.RecordToolCallArguments(Activity, arguments);
    }

    /// <summary>
    ///     Sets the tool call result.
    /// </summary>
    public void SetResult(object result)
    {
        GenAiActivityHelper.RecordToolCallResult(Activity, result);
    }

    /// <summary>
    ///     Marks this tool call as a transfer (handoff) of work to another agent.
    /// </summary>
    /// <param name="targetAgentName">The name of the agent that receives the work.</param>
    /// <param name="mode">"return_to_caller" or "pass_control".</param>
    public void SetTransfer(string targetAgentName, string? mode = null)
    {
        Activity?.SetTag(GenAiAttributes.TransferTargetName, targetAgentName);

        if (!string.IsNullOrEmpty(mode))
        {
            Activity?.SetTag(GenAiAttributes.TransferMode, mode);
        }
    }

    /// <summary>
    ///     Sets the Agent Skill that this tool call loads, reads, or runs.
    /// </summary>
    /// <param name="skillName">The skill name.</param>
    /// <param name="description">Optional skill description.</param>
    /// <param name="sourceUri">Optional URI the skill was loaded from.</param>
    /// <param name="resourceName">Optional skill-relative resource name (script, reference, or asset).</param>
    public void SetSkill(string skillName, string? description = null, string? sourceUri = null,
        string? resourceName = null)
    {
        if (Activity == null)
        {
            return;
        }

        Activity.SetTag(GenAiAttributes.SkillName, skillName);

        if (!string.IsNullOrEmpty(description))
        {
            Activity.SetTag(GenAiAttributes.SkillDescription, description);
        }

        if (!string.IsNullOrEmpty(sourceUri))
        {
            Activity.SetTag(GenAiAttributes.SkillSourceUri, sourceUri);
        }

        if (!string.IsNullOrEmpty(resourceName))
        {
            Activity.SetTag(GenAiAttributes.SkillResourceName, resourceName);
        }
    }
}