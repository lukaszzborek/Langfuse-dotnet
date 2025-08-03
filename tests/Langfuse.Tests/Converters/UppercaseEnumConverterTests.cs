using System.Text.Json;
using zborek.Langfuse.Models.Comment;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Tests.Converters;

public class UppercaseEnumConverterTests
{
    [Fact]
    public void Should_Serialize_Enum_To_Uppercase()
    {
        // Arrange
        var testObject = new { Source = ScoreSource.Api };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert
        Assert.Contains("\"API\"", json);
    }

    [Fact]
    public void Should_Deserialize_Uppercase_To_Enum()
    {
        // Arrange
        var json = "{\"Source\":\"ANNOTATION\"}";

        // Act
        var result = JsonSerializer.Deserialize<TestClass>(json);

        // Assert
        Assert.Equal(ScoreSource.Annotation, result.Source);
    }

    [Fact]
    public void Should_Deserialize_Various_Cases_To_Enum()
    {
        // Test case-insensitive deserialization
        var testCases = new[]
        {
            "{\"ObjectType\":\"TRACE\"}", // UPPERCASE
            "{\"ObjectType\":\"trace\"}", // lowercase
            "{\"ObjectType\":\"Trace\"}", // PascalCase
            "{\"ObjectType\":\"OBSERVATION\"}", // UPPERCASE
            "{\"ObjectType\":\"observation\"}", // lowercase
            "{\"ObjectType\":\"Observation\"}" // PascalCase
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
            // Act
            var result = JsonSerializer.Deserialize<TestComment>(testCases[i]);

            // Assert
            Assert.Equal(expectedResults[i], result.ObjectType);
        }
    }

    [Fact]
    public void Should_Serialize_All_Enum_Values_To_Uppercase()
    {
        // Test multiple enum types
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