# Task 09: Trace Filter Support

**Task ID:** openapi-update-09
**Priority:** High
**Dependencies:** None
**Approach:** Implementation (Complex feature)

## Objective

Implement advanced JSON-based filtering for the traces API, allowing complex query conditions with type-specific operators. This is the most complex task as it requires designing an intuitive C# API for building filter conditions.

## What Needs to Be Done

### Files to Create

Create the following files in `src/Langfuse/Models/Trace/`:

1. **TraceFilter.cs** - Main filter container
2. **TraceFilterCondition.cs** - Base filter condition
3. **TraceFilterType.cs** - Enum for filter types
4. **TraceFilterOperator.cs** - Operators for each type

### Design Decision Required

Choose between two approaches:

**Option A: Strongly-Typed Filter Classes**
```csharp
public record TraceFilter
{
    public TraceFilterCondition[] Conditions { get; init; } = Array.Empty<TraceFilterCondition>();
}

public abstract record TraceFilterCondition
{
    public required string Column { get; init; }
}

public record StringFilter : TraceFilterCondition
{
    public required StringOperator Operator { get; init; }
    public required string Value { get; init; }
}

public record DateTimeFilter : TraceFilterCondition
{
    public required DateTimeOperator Operator { get; init; }
    public required DateTime Value { get; init; }
}
// ... more specific types
```

**Option B: Fluent Builder Pattern**
```csharp
public class TraceFilterBuilder
{
    public TraceFilterBuilder Where(string column, StringOperator op, string value);
    public TraceFilterBuilder WhereDateTime(string column, DateTimeOperator op, DateTime value);
    public TraceFilterBuilder WhereNumber(string column, NumberOperator op, double value);
    public TraceFilter Build();
}
```

### Recommended Approach: Option A (Strongly-Typed)

More type-safe, easier to serialize, and follows existing SDK patterns.

### Implementation Details

**1. TraceFilterType.cs**
```csharp
/// <summary>
/// Type of filter condition
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<TraceFilterType>))]
public enum TraceFilterType
{
    [JsonPropertyName("datetime")]
    DateTime,

    [JsonPropertyName("string")]
    String,

    [JsonPropertyName("number")]
    Number,

    [JsonPropertyName("stringOptions")]
    StringOptions,

    [JsonPropertyName("categoryOptions")]
    CategoryOptions,

    [JsonPropertyName("arrayOptions")]
    ArrayOptions,

    [JsonPropertyName("stringObject")]
    StringObject,

    [JsonPropertyName("numberObject")]
    NumberObject,

    [JsonPropertyName("boolean")]
    Boolean,

    [JsonPropertyName("null")]
    Null
}
```

**2. TraceFilterCondition.cs** (Polymorphic base)
```csharp
/// <summary>
/// Base class for trace filter conditions
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(StringFilterCondition), "string")]
[JsonDerivedType(typeof(DateTimeFilterCondition), "datetime")]
[JsonDerivedType(typeof(NumberFilterCondition), "number")]
// ... other derived types
public abstract record TraceFilterCondition
{
    /// <summary>
    /// Column to filter on
    /// </summary>
    [JsonPropertyName("column")]
    public required string Column { get; init; }
}

/// <summary>
/// String filter condition
/// </summary>
public record StringFilterCondition : TraceFilterCondition
{
    /// <summary>
    /// Operator: =, contains, does not contain, starts with, ends with
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    /// Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required string Value { get; init; }
}

/// <summary>
/// DateTime filter condition
/// </summary>
public record DateTimeFilterCondition : TraceFilterCondition
{
    /// <summary>
    /// Operator: >, <, >=, <=
    /// </summary>
    [JsonPropertyName("operator")]
    public required string Operator { get; init; }

    /// <summary>
    /// Value to compare against
    /// </summary>
    [JsonPropertyName("value")]
    public required DateTime Value { get; init; }
}

// ... implement other condition types
```

**3. TraceFilter.cs**
```csharp
/// <summary>
/// Complex filter for traces
/// </summary>
public record TraceFilter
{
    /// <summary>
    /// Array of filter conditions
    /// </summary>
    [JsonPropertyName("filter")]
    public TraceFilterCondition[] Conditions { get; init; } = Array.Empty<TraceFilterCondition>();
}
```

### Files to Modify

**1. src/Langfuse/Models/Trace/GetTracesRequest.cs**

Add filter property:
```csharp
/// <summary>
/// JSON-based filter conditions.
/// When provided, takes precedence over legacy filter parameters.
/// </summary>
[JsonPropertyName("filter")]
public TraceFilter? Filter { get; init; }
```

**2. src/Langfuse/Client/LangfuseClient.Trace.cs**

Update GetTracesAsync to serialize filter:
```csharp
// In query string building:
if (request.Filter != null && request.Filter.Conditions.Length > 0)
{
    var filterJson = JsonSerializer.Serialize(request.Filter.Conditions, _jsonOptions);
    queryParams.Add($"filter={Uri.EscapeDataString(filterJson)}");
}
```

### Filter Operators by Type

| Type | Operators |
|------|-----------|
| datetime | `>`, `<`, `>=`, `<=` |
| string | `=`, `contains`, `does not contain`, `starts with`, `ends with` |
| number | `=`, `>`, `<`, `>=`, `<=` |
| stringOptions | `any of`, `none of` |
| categoryOptions | `any of`, `none of` |
| arrayOptions | `any of`, `none of`, `all of` |
| stringObject | `=`, `contains`, `does not contain`, `starts with`, `ends with` |
| numberObject | `=`, `>`, `<`, `>=`, `<=` |
| boolean | `=`, `<>` |
| null | `is null`, `is not null` |

### Testing Requirements

Create test file: `tests/Langfuse.Tests/Models/TraceFilterTests.cs`

Tests to write:
1. **Serialization:**
   - Test StringFilterCondition serializes correctly
   - Test DateTimeFilterCondition serializes correctly
   - Test polymorphic serialization (type discriminator)
   - Test array of mixed conditions
   - Test null filter (not sent)

2. **Complex Filters:**
   - Test multiple conditions
   - Test stringObject with key property
   - Test numberObject with key property
   - Test operators serialize as strings

3. **Integration:**
   - Test GetTracesRequest with filter
   - Test filter JSON in query string
   - Test filter precedence over legacy params

### Usage Example

```csharp
var filter = new TraceFilter
{
    Conditions = new TraceFilterCondition[]
    {
        new StringFilterCondition
        {
            Column = "userId",
            Operator = "=",
            Value = "user123"
        },
        new DateTimeFilterCondition
        {
            Column = "timestamp",
            Operator = ">",
            Value = DateTime.Now.AddDays(-7)
        }
    }
};

var request = new GetTracesRequest
{
    Filter = filter
};

var traces = await client.GetTracesAsync(request);
```

## Acceptance Criteria

- [x] TraceFilterCondition base class created with polymorphic support
- [x] Derived condition classes for all filter types (10 types)
- [x] TraceFilter container class created
- [x] TraceFilterType enum created with correct serialization
- [x] All condition types support correct operators
- [x] stringObject and numberObject support key property
- [x] Filter property added to GetTracesRequest
- [x] GetTracesAsync updated to serialize filter to JSON
- [x] Filter JSON properly URL encoded in query string
- [x] Comprehensive tests for all condition types
- [x] Polymorphic serialization tested
- [x] Complex multi-condition filters tested
- [x] Filter precedence documented (overrides legacy params)
- [x] Usage examples in XML documentation
- [x] All tests pass
- [x] Code builds without warnings
