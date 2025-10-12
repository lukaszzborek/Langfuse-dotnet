using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.LlmConnection;

/// <summary>
/// The adapter used to interface with the LLM
/// </summary>
[JsonConverter(typeof(LlmAdapterConverter))]
public enum LlmAdapter
{
    /// <summary>
    /// Anthropic adapter
    /// </summary>
    Anthropic,

    /// <summary>
    /// OpenAI adapter
    /// </summary>
    OpenAi,

    /// <summary>
    /// Azure OpenAI adapter
    /// </summary>
    Azure,

    /// <summary>
    /// AWS Bedrock adapter
    /// </summary>
    Bedrock,

    /// <summary>
    /// Google Vertex AI adapter
    /// </summary>
    GoogleVertexAi,

    /// <summary>
    /// Google AI Studio adapter
    /// </summary>
    GoogleAiStudio
}
