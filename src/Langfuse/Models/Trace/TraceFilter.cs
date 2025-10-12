using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Complex filter for traces with multiple conditions.
///     When provided, takes precedence over legacy filter parameters.
/// </summary>
/// <example>
/// <code>
/// var filter = new TraceFilter
/// {
///     Conditions = new TraceFilterCondition[]
///     {
///         new StringFilterCondition
///         {
///             Column = "userId",
///             Operator = "=",
///             Value = "user123"
///         },
///         new DateTimeFilterCondition
///         {
///             Column = "timestamp",
///             Operator = "&gt;",
///             Value = DateTime.Now.AddDays(-7)
///         }
///     }
/// };
/// </code>
/// </example>
public record TraceFilter
{
    /// <summary>
    ///     Array of filter conditions. All conditions are combined with AND logic.
    /// </summary>
    [JsonPropertyName("filter")]
    public TraceFilterCondition[] Conditions { get; init; } = Array.Empty<TraceFilterCondition>();
}
