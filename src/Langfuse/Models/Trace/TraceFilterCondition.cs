using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Trace;

/// <summary>
///     Base class for trace filter conditions. Supports polymorphic serialization based on the type discriminator.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StringFilterCondition), "string")]
[JsonDerivedType(typeof(DateTimeFilterCondition), "datetime")]
[JsonDerivedType(typeof(NumberFilterCondition), "number")]
[JsonDerivedType(typeof(StringOptionsFilterCondition), "stringOptions")]
[JsonDerivedType(typeof(CategoryOptionsFilterCondition), "categoryOptions")]
[JsonDerivedType(typeof(ArrayOptionsFilterCondition), "arrayOptions")]
[JsonDerivedType(typeof(StringObjectFilterCondition), "stringObject")]
[JsonDerivedType(typeof(NumberObjectFilterCondition), "numberObject")]
[JsonDerivedType(typeof(BooleanFilterCondition), "boolean")]
[JsonDerivedType(typeof(NullFilterCondition), "null")]
public abstract record TraceFilterCondition
{
    /// <summary>
    ///     Column to filter on
    /// </summary>
    [JsonPropertyName("column")]
    public required string Column { get; init; }
}

/// <summary>
///     String filter condition with operators: =, contains, does not contain, starts with, ends with
/// </summary>
public record StringFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for string comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}

/// <summary>
///     DateTime filter condition with operators: &gt;, &lt;, &gt;=, &lt;=
/// </summary>
public record DateTimeFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for datetime comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required DateTime Value { get; init; }
}

/// <summary>
///     Number filter condition with operators: =, &gt;, &lt;, &gt;=, &lt;=
/// </summary>
public record NumberFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for number comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required double Value { get; init; }
}

/// <summary>
///     String options filter condition with operators: any of, none of
/// </summary>
public record StringOptionsFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for string options comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Values to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required string[] Value { get; init; }
}

/// <summary>
///     Category options filter condition with operators: any of, none of
/// </summary>
public record CategoryOptionsFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for category options comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Values to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required string[] Value { get; init; }
}

/// <summary>
///     Array options filter condition with operators: any of, none of, all of
/// </summary>
public record ArrayOptionsFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for array options comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Values to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required string[] Value { get; init; }
}

/// <summary>
///     String object filter condition with operators: =, contains, does not contain, starts with, ends with.
///     Used for filtering on string properties within JSON objects.
/// </summary>
public record StringObjectFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for string object comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required string Value { get; init; }

    /// <summary>
    ///     Key property name within the object to filter on
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }
}

/// <summary>
///     Number object filter condition with operators: =, &gt;, &lt;, &gt;=, &lt;=.
///     Used for filtering on numeric properties within JSON objects.
/// </summary>
public record NumberObjectFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for number object comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required double Value { get; init; }

    /// <summary>
    ///     Key property name within the object to filter on
    /// </summary>
    [JsonPropertyName("key")]
    public string? Key { get; init; }
}

/// <summary>
///     Boolean filter condition with operators: =, &lt;&gt;
/// </summary>
public record BooleanFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for boolean comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required bool Value { get; init; }
}

/// <summary>
///     Null filter condition with operators: is null, is not null
/// </summary>
public record NullFilterCondition : TraceFilterCondition
{
    /// <summary>
    ///     Operator for null comparison
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    ///     Value is always null for this filter type
    /// </summary>
    [JsonPropertyName("value")]
    public object? Value { get; init; }
}