namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Defines the type of prompt template, determining its structure and intended AI model interface.
/// </summary>
public enum PromptType
{
    /// <summary>
    ///     Text-based prompt for completion-style models. Contains a single text string with optional placeholders.
    /// </summary>
    Text,

    /// <summary>
    ///     Chat-based prompt for conversational models. Contains structured message sequences with roles and content.
    /// </summary>
    Chat
}