using System.Text.Json;
using zborek.Langfuse.Models.Comment;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Tests.Converters;

public class UppercaseEnumConverterTests
{
    [Fact]
    public void Should_Serialize_Enum_To_Uppercase()
    {
        var testObject = new { Source = ScoreSource.Api };

        var json = JsonSerializer.Serialize(testObject);

        Assert.Contains("\"API\"", json);
    }

    [Fact]
    public void Should_Deserialize_Uppercase_To_Enum()
    {
        var json = "{\"Source\":\"ANNOTATION\"}";

        var result = JsonSerializer.Deserialize<TestClass>(json);

        Assert.Equal(ScoreSource.Annotation, result.Source);
    }

    [Fact]
    public void Should_Deserialize_Various_Cases_To_Enum()
    {
        var testCases = new[]
        {
            "{\"ObjectType\":\"TRACE\"}",
            "{\"ObjectType\":\"trace\"}",
            "{\"ObjectType\":\"Trace\"}",
            "{\"ObjectType\":\"OBSERVATION\"}",
            "{\"ObjectType\":\"observation\"}",
            "{\"ObjectType\":\"Observation\"}"
        };

        var expectedResults = new[]
        {
            CommentObjectType.Trace,
            CommentObjectType.Trace,
            CommentObjectType.Trace,
            CommentObjectType.Observation,
            CommentObjectType.Observation,
            CommentObjectType.Observation
        };

        for (var i = 0; i < testCases.Length; i++)
        {
            var result = JsonSerializer.Deserialize<TestComment>(testCases[i]);

            Assert.Equal(expectedResults[i], result.ObjectType);
        }
    }

    [Fact]
    public void Should_Serialize_All_Enum_Values_To_Uppercase()
    {
        var scoreSource = ScoreSource.Eval;
        var commentType = CommentObjectType.Session;

        var testObject = new { ScoreSource = scoreSource, CommentType = commentType };
        var json = JsonSerializer.Serialize(testObject);

        Assert.Contains("\"EVAL\"", json);
        Assert.Contains("\"SESSION\"", json);
    }

    private class TestClass
    {
        public ScoreSource Source { get; set; }
    }

    private class TestComment
    {
        public CommentObjectType ObjectType { get; set; }
    }
}