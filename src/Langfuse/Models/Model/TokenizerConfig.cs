using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Configuration settings for tokenization behavior, including overhead costs and model-specific parameters.
/// </summary>
public class TokenizerConfig
{
    /// <summary>
    ///     Additional tokens to add per function/tool name. Accounts for overhead in function calling scenarios.
    /// </summary>
    [JsonPropertyName("tokensPerName")]
    public int? TokensPerName { get; set; }

    /// <summary>
    ///     Name of the specific tokenizer model to use for counting tokens (e.g., "cl100k_base", "r50k_base").
    /// </summary>
    [JsonPropertyName("tokenizerModel")]
    public string? TokenizerModel { get; set; }

    /// <summary>
    ///     Additional tokens to add per message in chat-based scenarios. Accounts for message formatting overhead.
    /// </summary>
    [JsonPropertyName("tokensPerMessage")]
    public int? TokensPerMessage { get; set; }
}