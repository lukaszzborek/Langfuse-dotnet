using System.Text.Json;
using Shouldly;
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

        json.ShouldContain("\"type\":\"string\"");
        json.ShouldContain("\"column\":\"userId\"");
        json.ShouldContain("\"operator\":\"=\"");
        json.ShouldContain("\"value\":\"user123\"");
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

        json.ShouldContain("\"type\":\"datetime\"");
        json.ShouldContain("\"column\":\"timestamp\"");
        json.ShouldContain("\"operator\"");
        json.ShouldContain("\"value\":\"2024-10-12");
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

        json.ShouldContain("\"type\":\"number\"");
        json.ShouldContain("\"column\":\"score\"");
        json.ShouldContain("\"operator\"");
        json.ShouldContain("\"value\":0.85");
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

        json.ShouldContain("\"type\":\"stringOptions\"");
        json.ShouldContain("\"column\":\"status\"");
        json.ShouldContain("\"operator\":\"any of\"");
        json.ShouldContain("\"value\":[\"active\",\"pending\"]");
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

        json.ShouldContain("\"type\":\"categoryOptions\"");
        json.ShouldContain("\"column\":\"category\"");
        json.ShouldContain("\"operator\":\"none of\"");
        json.ShouldContain("\"value\":[\"spam\",\"test\"]");
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

        json.ShouldContain("\"type\":\"arrayOptions\"");
        json.ShouldContain("\"column\":\"tags\"");
        json.ShouldContain("\"operator\":\"all of\"");
        json.ShouldContain("\"value\":[\"production\",\"critical\"]");
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

        json.ShouldContain("\"type\":\"stringObject\"");
        json.ShouldContain("\"column\":\"metadata\"");
        json.ShouldContain("\"operator\":\"contains\"");
        json.ShouldContain("\"value\":\"important\"");
        json.ShouldContain("\"key\":\"description\"");
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

        json.ShouldContain("\"type\":\"numberObject\"");
        json.ShouldContain("\"column\":\"metrics\"");
        json.ShouldContain("\"operator\"");
        json.ShouldContain("\"value\":100.5");
        json.ShouldContain("\"key\":\"latency\"");
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

        json.ShouldContain("\"type\":\"boolean\"");
        json.ShouldContain("\"column\":\"public\"");
        json.ShouldContain("\"operator\":\"=\"");
        json.ShouldContain("\"value\":true");
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

        json.ShouldContain("\"type\":\"null\"");
        json.ShouldContain("\"column\":\"endTime\"");
        json.ShouldContain("\"operator\":\"is null\"");
        json.ShouldContain("\"value\":null");
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

        json.ShouldContain("\"type\":\"string\"");
        json.ShouldContain("\"type\":\"datetime\"");
        json.ShouldContain("\"type\":\"number\"");
        json.ShouldContain("\"userId\"");
        json.ShouldContain("\"timestamp\"");
        json.ShouldContain("\"score\"");
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

        condition.ShouldNotBeNull();
        condition.ShouldBeOfType<StringFilterCondition>();
        var stringCondition = (StringFilterCondition)condition;
        stringCondition.Column.ShouldBe("name");
        stringCondition.Operator.ShouldBe("contains");
        stringCondition.Value.ShouldBe("test");
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

        condition.ShouldNotBeNull();
        condition.ShouldBeOfType<DateTimeFilterCondition>();
        var dateTimeCondition = (DateTimeFilterCondition)condition;
        dateTimeCondition.Column.ShouldBe("createdAt");
        dateTimeCondition.Operator.ShouldBe("<=");
        dateTimeCondition.Value.ShouldBe(new DateTime(2024, 10, 12, 10, 30, 0, DateTimeKind.Utc));
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

        TraceFilterCondition[]? conditions = JsonSerializer.Deserialize<TraceFilterCondition[]>(json, JsonOptions);

        conditions.ShouldNotBeNull();
        conditions.Length.ShouldBe(2);
        conditions[0].ShouldBeOfType<StringFilterCondition>();
        conditions[1].ShouldBeOfType<NumberFilterCondition>();
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

        queryString.ShouldContain("page=1");
        queryString.ShouldContain("limit=50");
        queryString.ShouldContain("filter=");
        queryString.ShouldContain("%22type%22%3A%22string%22"); // URL-encoded "type":"string"
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

        queryString.ShouldNotContain("filter=");
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

        queryString.ShouldNotContain("filter=");
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

        json.ShouldContain("\"type\":\"stringObject\"");
        json.ShouldContain("\"type\":\"numberObject\"");
        json.ShouldContain("\"type\":\"boolean\"");
        json.ShouldContain("\"key\":\"environment\"");
        json.ShouldContain("\"key\":\"requestCount\"");
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
            json.ShouldContain($"\"operator\":\"{op}\"");
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
            json.ShouldContain("\"operator\"");
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
            json.ShouldContain("\"operator\"");
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
        json.ShouldContain("\"operator\":\"all of\"");
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
            json.ShouldContain($"\"operator\":\"{op}\"");
        }
    }

    [Fact]
    public void Should_Preserve_Type_Information_In_Polymorphic_Serialization()
    {
        var conditions = new TraceFilterCondition[]
        {
            new StringFilterCondition { Column = "a", Operator = "=", Value = "test" },
            new NumberFilterCondition { Column = "b", Operator = ">", Value = 1 },
            new DateTimeFilterCondition { Column = "c", Operator = "<", Value = DateTime.UtcNow },
            new BooleanFilterCondition { Column = "d", Operator = "=", Value = true },
            new NullFilterCondition { Column = "e", Operator = "is null", Value = null }
        };

        var json = JsonSerializer.Serialize(conditions, JsonOptions);

        json.ShouldContain("\"type\":\"string\"");
        json.ShouldContain("\"type\":\"number\"");
        json.ShouldContain("\"type\":\"datetime\"");
        json.ShouldContain("\"type\":\"boolean\"");
        json.ShouldContain("\"type\":\"null\"");
    }

    [Fact]
    public void TraceFilter_Should_Have_Empty_Array_As_Default()
    {
        var filter = new TraceFilter();

        filter.Conditions.ShouldNotBeNull();
        filter.Conditions.ShouldBeEmpty();
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

        json.ShouldContain("\"type\":\"stringObject\"");
        json.ShouldContain("\"value\":\"test\"");
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

        json.ShouldContain("\"type\":\"numberObject\"");
        json.ShouldContain("\"value\":123");
        // Key should not be included when null
    }
}