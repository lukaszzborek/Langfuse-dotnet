using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Model;

/// <summary>
///     Legacy tokenizer identifier kept for API compatibility. Ingestion no longer uses it to infer observation usage.
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