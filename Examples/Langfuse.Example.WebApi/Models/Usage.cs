using System.Text.Json.Serialization;

namespace Langfuse.Example.WebApi.Models;

public class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }

    [JsonPropertyName("completion_tokens_details")]
    public CompletionTokenDetails CompletionTokensDetails { get; set; }
}