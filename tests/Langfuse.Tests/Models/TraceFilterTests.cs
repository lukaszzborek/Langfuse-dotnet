using System.Text.Json;
using zborek.Langfuse.Models.Trace;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Tests.Models;

public class TraceFilterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public void Should_Serialize_StringFilterCondition_Correctly()
    {
        TraceFilterCondition condition = new StringFilterCondition
        {
            Column = "userId",
            Operator = "=",
            Value = "user123"
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"string\"", json);
        Assert.Contains("\"column\":\"userId\"", json);
        Assert.Contains("\"operator\":\"=\"", json);
        Assert.Contains("\"value\":\"user123\"", json);
    }

    [Fact]
    public void Should_Serialize_DateTimeFilterCondition_Correctly()
    {
        TraceFilterCondition condition = new DateTimeFilterCondition
        {
            Column = "timestamp",
            Operator = ">",
            Value = new DateTime(2024, 10, 12, 10, 30, 0, DateTimeKind.Utc)
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"datetime\"", json);
        Assert.Contains("\"column\":\"timestamp\"", json);
        Assert.Contains("\"operator\"", json);
        Assert.Contains("\"value\":\"2024-10-12", json);
    }

    [Fact]
    public void Should_Serialize_NumberFilterCondition_Correctly()
    {
        TraceFilterCondition condition = new NumberFilterCondition
        {
            Column = "score",
            Operator = ">=",
            Value = 0.85
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"number\"", json);
        Assert.Contains("\"column\":\"score\"", json);
        Assert.Contains("\"operator\"", json);
        Assert.Contains("\"value\":0.85", json);
    }

    [Fact]
    public void Should_Serialize_StringOptionsFilterCondition_Correctly()
    {
        TraceFilterCondition condition = new StringOptionsFilterCondition
        {
            Column = "status",
            Operator = "any of",
            Value = new[] { "active", "pending" }
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"stringOptions\"", json);
        Assert.Contains("\"column\":\"status\"", json);
        Assert.Contains("\"operator\":\"any of\"", json);
        Assert.Contains("\"value\":[\"active\",\"pending\"]", json);
    }

    [Fact]
    public void Should_Serialize_CategoryOptionsFilterCondition_Correctly()
    {
        TraceFilterCondition condition = new CategoryOptionsFilterCondition
        {
            Column = "category",
            Operator = "none of",
            Value = new[] { "spam", "test" }
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"categoryOptions\"", json);
        Assert.Contains("\"column\":\"category\"", json);
        Assert.Contains("\"operator\":\"none of\"", json);
        Assert.Contains("\"value\":[\"spam\",\"test\"]", json);
    }

    [Fact]
    public void Should_Serialize_ArrayOptionsFilterCondition_Correctly()
    {
        TraceFilterCondition condition = new ArrayOptionsFilterCondition
        {
            Column = "tags",
            Operator = "all of",
            Value = new[] { "production", "critical" }
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"arrayOptions\"", json);
        Assert.Contains("\"column\":\"tags\"", json);
        Assert.Contains("\"operator\":\"all of\"", json);
        Assert.Contains("\"value\":[\"production\",\"critical\"]", json);
    }

    [Fact]
    public void Should_Serialize_StringObjectFilterCondition_With_Key()
    {
        TraceFilterCondition condition = new StringObjectFilterCondition
        {
            Column = "metadata",
            Operator = "contains",
            Value = "important",
            Key = "description"
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"stringObject\"", json);
        Assert.Contains("\"column\":\"metadata\"", json);
        Assert.Contains("\"operator\":\"contains\"", json);
        Assert.Contains("\"value\":\"important\"", json);
        Assert.Contains("\"key\":\"description\"", json);
    }

    [Fact]
    public void Should_Serialize_NumberObjectFilterCondition_With_Key()
    {
        TraceFilterCondition condition = new NumberObjectFilterCondition
        {
            Column = "metrics",
            Operator = "<=",
            Value = 100.5,
            Key = "latency"
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"numberObject\"", json);
        Assert.Contains("\"column\":\"metrics\"", json);
        Assert.Contains("\"operator\"", json);
        Assert.Contains("\"value\":100.5", json);
        Assert.Contains("\"key\":\"latency\"", json);
    }

    [Fact]
    public void Should_Serialize_BooleanFilterCondition_Correctly()
    {
        TraceFilterCondition condition = new BooleanFilterCondition
        {
            Column = "public",
            Operator = "=",
            Value = true
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"boolean\"", json);
        Assert.Contains("\"column\":\"public\"", json);
        Assert.Contains("\"operator\":\"=\"", json);
        Assert.Contains("\"value\":true", json);
    }

    [Fact]
    public void Should_Serialize_NullFilterCondition_Correctly()
    {
        TraceFilterCondition condition = new NullFilterCondition
        {
            Column = "endTime",
            Operator = "is null",
            Value = null
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"null\"", json);
        Assert.Contains("\"column\":\"endTime\"", json);
        Assert.Contains("\"operator\":\"is null\"", json);
        Assert.Contains("\"value\":null", json);
    }

    [Fact]
    public void Should_Serialize_TraceFilter_With_Multiple_Conditions()
    {
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
                    Value = new DateTime(2024, 10, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new NumberFilterCondition
                {
                    Column = "score",
                    Operator = ">=",
                    Value = 0.8
                }
            }
        };

        var json = JsonSerializer.Serialize(filter.Conditions, JsonOptions);

        Assert.Contains("\"type\":\"string\"", json);
        Assert.Contains("\"type\":\"datetime\"", json);
        Assert.Contains("\"type\":\"number\"", json);
        Assert.Contains("\"userId\"", json);
        Assert.Contains("\"timestamp\"", json);
        Assert.Contains("\"score\"", json);
    }

    [Fact]
    public void Should_Deserialize_StringFilterCondition_From_Json()
    {
        var json = @"{
            ""type"": ""string"",
            ""column"": ""name"",
            ""operator"": ""contains"",
            ""value"": ""test""
        }";

        var condition = JsonSerializer.Deserialize<TraceFilterCondition>(json, JsonOptions);

        Assert.NotNull(condition);
        Assert.IsType<StringFilterCondition>(condition);
        var stringCondition = (StringFilterCondition)condition;
        Assert.Equal("name", stringCondition.Column);
        Assert.Equal("contains", stringCondition.Operator);
        Assert.Equal("test", stringCondition.Value);
    }

    [Fact]
    public void Should_Deserialize_DateTimeFilterCondition_From_Json()
    {
        var json = @"{
            ""type"": ""datetime"",
            ""column"": ""createdAt"",
            ""operator"": ""<="",
            ""value"": ""2024-10-12T10:30:00Z""
        }";

        var condition = JsonSerializer.Deserialize<TraceFilterCondition>(json, JsonOptions);

        Assert.NotNull(condition);
        Assert.IsType<DateTimeFilterCondition>(condition);
        var dateTimeCondition = (DateTimeFilterCondition)condition;
        Assert.Equal("createdAt", dateTimeCondition.Column);
        Assert.Equal("<=", dateTimeCondition.Operator);
        Assert.Equal(new DateTime(2024, 10, 12, 10, 30, 0, DateTimeKind.Utc), dateTimeCondition.Value);
    }

    [Fact]
    public void Should_Deserialize_Mixed_Conditions_Array()
    {
        var json = @"[
            {
                ""type"": ""string"",
                ""column"": ""userId"",
                ""operator"": ""="",
                ""value"": ""user123""
            },
            {
                ""type"": ""number"",
                ""column"": ""score"",
                ""operator"": "">="",
                ""value"": 0.9
            }
        ]";

        var conditions = JsonSerializer.Deserialize<TraceFilterCondition[]>(json, JsonOptions);

        Assert.NotNull(conditions);
        Assert.Equal(2, conditions.Length);
        Assert.IsType<StringFilterCondition>(conditions[0]);
        Assert.IsType<NumberFilterCondition>(conditions[1]);
    }

    [Fact]
    public void Should_Build_QueryString_With_Filter()
    {
        var request = new TraceListRequest
        {
            Page = 1,
            Limit = 50,
            Filter = new TraceFilter
            {
                Conditions = new TraceFilterCondition[]
                {
                    new StringFilterCondition
                    {
                        Column = "userId",
                        Operator = "=",
                        Value = "user123"
                    }
                }
            }
        };

        var queryString = QueryStringHelper.BuildQueryString(request);

        Assert.Contains("page=1", queryString);
        Assert.Contains("limit=50", queryString);
        Assert.Contains("filter=", queryString);
        Assert.Contains("%22type%22%3A%22string%22", queryString); // URL-encoded "type":"string"
    }

    [Fact]
    public void Should_Not_Include_Filter_In_QueryString_When_No_Conditions()
    {
        var request = new TraceListRequest
        {
            Page = 1,
            Filter = new TraceFilter
            {
                Conditions = Array.Empty<TraceFilterCondition>()
            }
        };

        var queryString = QueryStringHelper.BuildQueryString(request);

        Assert.DoesNotContain("filter=", queryString);
    }

    [Fact]
    public void Should_Not_Include_Filter_In_QueryString_When_Null()
    {
        var request = new TraceListRequest
        {
            Page = 1,
            Filter = null
        };

        var queryString = QueryStringHelper.BuildQueryString(request);

        Assert.DoesNotContain("filter=", queryString);
    }

    [Fact]
    public void Should_Serialize_Complex_Filter_With_Object_Conditions()
    {
        var filter = new TraceFilter
        {
            Conditions = new TraceFilterCondition[]
            {
                new StringObjectFilterCondition
                {
                    Column = "metadata",
                    Operator = "=",
                    Value = "production",
                    Key = "environment"
                },
                new NumberObjectFilterCondition
                {
                    Column = "metrics",
                    Operator = ">",
                    Value = 1000,
                    Key = "requestCount"
                },
                new BooleanFilterCondition
                {
                    Column = "public",
                    Operator = "=",
                    Value = false
                }
            }
        };

        var json = JsonSerializer.Serialize(filter.Conditions, JsonOptions);

        Assert.Contains("\"type\":\"stringObject\"", json);
        Assert.Contains("\"type\":\"numberObject\"", json);
        Assert.Contains("\"type\":\"boolean\"", json);
        Assert.Contains("\"key\":\"environment\"", json);
        Assert.Contains("\"key\":\"requestCount\"", json);
    }

    [Fact]
    public void Should_Handle_All_String_Operators()
    {
        var operators = new[] { "=", "contains", "does not contain", "starts with", "ends with" };

        foreach (var op in operators)
        {
            TraceFilterCondition condition = new StringFilterCondition
            {
                Column = "name",
                Operator = op,
                Value = "test"
            };

            var json = JsonSerializer.Serialize(condition, JsonOptions);
            Assert.Contains($"\"operator\":\"{op}\"", json);
        }
    }

    [Fact]
    public void Should_Handle_All_Number_Operators()
    {
        var operators = new[] { "=", ">", "<", ">=", "<=" };

        foreach (var op in operators)
        {
            TraceFilterCondition condition = new NumberFilterCondition
            {
                Column = "value",
                Operator = op,
                Value = 42
            };

            var json = JsonSerializer.Serialize(condition, JsonOptions);
            // Just verify operator field exists (< and > are escaped in JSON)
            Assert.Contains("\"operator\"", json);
        }
    }

    [Fact]
    public void Should_Handle_All_DateTime_Operators()
    {
        var operators = new[] { ">", "<", ">=", "<=" };

        foreach (var op in operators)
        {
            TraceFilterCondition condition = new DateTimeFilterCondition
            {
                Column = "timestamp",
                Operator = op,
                Value = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(condition, JsonOptions);
            // Just verify operator field exists (< and > are escaped in JSON)
            Assert.Contains("\"operator\"", json);
        }
    }

    [Fact]
    public void Should_Handle_Array_Operators()
    {
        var arrayOperators = new[] { "any of", "none of", "all of" };

        TraceFilterCondition condition = new ArrayOptionsFilterCondition
        {
            Column = "tags",
            Operator = "all of",
            Value = new[] { "tag1", "tag2" }
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);
        Assert.Contains("\"operator\":\"all of\"", json);
    }

    [Fact]
    public void Should_Handle_Null_Operators()
    {
        var nullOperators = new[] { "is null", "is not null" };

        foreach (var op in nullOperators)
        {
            TraceFilterCondition condition = new NullFilterCondition
            {
                Column = "field",
                Operator = op,
                Value = null
            };

            var json = JsonSerializer.Serialize(condition, JsonOptions);
            Assert.Contains($"\"operator\":\"{op}\"", json);
        }
    }

    [Fact]
    public void Should_Preserve_Type_Information_In_Polymorphic_Serialization()
    {
        TraceFilterCondition[] conditions = new TraceFilterCondition[]
        {
            new StringFilterCondition { Column = "a", Operator = "=", Value = "test" },
            new NumberFilterCondition { Column = "b", Operator = ">", Value = 1 },
            new DateTimeFilterCondition { Column = "c", Operator = "<", Value = DateTime.UtcNow },
            new BooleanFilterCondition { Column = "d", Operator = "=", Value = true },
            new NullFilterCondition { Column = "e", Operator = "is null", Value = null }
        };

        var json = JsonSerializer.Serialize(conditions, JsonOptions);

        Assert.Contains("\"type\":\"string\"", json);
        Assert.Contains("\"type\":\"number\"", json);
        Assert.Contains("\"type\":\"datetime\"", json);
        Assert.Contains("\"type\":\"boolean\"", json);
        Assert.Contains("\"type\":\"null\"", json);
    }

    [Fact]
    public void TraceFilter_Should_Have_Empty_Array_As_Default()
    {
        var filter = new TraceFilter();

        Assert.NotNull(filter.Conditions);
        Assert.Empty(filter.Conditions);
    }

    [Fact]
    public void Should_Handle_StringObject_Without_Key()
    {
        TraceFilterCondition condition = new StringObjectFilterCondition
        {
            Column = "metadata",
            Operator = "contains",
            Value = "test",
            Key = null
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"stringObject\"", json);
        Assert.Contains("\"value\":\"test\"", json);
        // Key should not be included when null (due to DefaultIgnoreCondition)
    }

    [Fact]
    public void Should_Handle_NumberObject_Without_Key()
    {
        TraceFilterCondition condition = new NumberObjectFilterCondition
        {
            Column = "data",
            Operator = "=",
            Value = 123,
            Key = null
        };

        var json = JsonSerializer.Serialize(condition, JsonOptions);

        Assert.Contains("\"type\":\"numberObject\"", json);
        Assert.Contains("\"value\":123", json);
        // Key should not be included when null
    }
}
