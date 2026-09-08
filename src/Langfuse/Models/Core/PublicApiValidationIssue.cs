using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     A single validation issue reported by the stable evaluators and evaluation-rules API.
/// </summary>
public class PublicApiValidationIssue
{
    /// <summary>
    ///     Machine-readable code identifying the validation issue.
    /// </summary>
    [JsonPropertyName("code")]
    public required string Code { get; set; }

    /// <summary>
    ///     Human-readable description of the validation issue.
    /// </summary>
    [JsonPropertyName("message")]
    public required string Message { get; set; }

    /// <summary>
    ///     Path to the field that failed validation. Each element is either a property name (string)
    ///     or an array index (integer), e.g. <c>["mapping", 0, "jsonPath"]</c>.
    /// </summary>
    [JsonPropertyName("path")]
    public required List<object> Path { get; set; }
}