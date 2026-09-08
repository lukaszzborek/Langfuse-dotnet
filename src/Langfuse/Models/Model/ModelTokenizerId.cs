using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Tokenizer supported by Langfuse for model usage inference.
/// </summary>
[JsonConverter(typeof(LowercaseEnumConverter<ModelTokenizerId>))]
public enum ModelTokenizerId
{
    /// <summary>
    ///     OpenAI tokenizer.
    /// </summary>
    Openai,

    /// <summary>
    ///     Claude tokenizer.
    /// </summary>
    Claude
}