using System.Text.Json;
using Shouldly;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Tests.Models;

public class ScoreV3ModelsTests
{
    /// <summary>
    ///     Builds score JSON with the dataType discriminator as the LAST property, since the server does not
    ///     guarantee property order.
    /// </summary>
    private static string ScoreJson(string valueJson, string dataType)
    {
        return $$"""
                 {
                     "id": "score-1",
                     "projectId": "proj-1",
                     "name": "quality",
                     "source": "API",
                     "timestamp": "2024-05-01T10:00:00Z",
                     "environment": "production",
                     "createdAt": "2024-05-01T10:00:01Z",
                     "updatedAt": "2024-05-01T10:00:02Z",
                     "value": {{valueJson}},
                     "dataType": "{{dataType}}"
                 }
                 """;
    }

    [Fact]
    public void Deserialize_NumericScore_WithDiscriminatorNotFirst()
    {
        var score = JsonSerializer.Deserialize<ScoreV3>(ScoreJson("0.87", "NUMERIC"));

        var numeric = score.ShouldBeOfType<NumericScoreV3>();
        numeric.Value.ShouldBe(0.87);
        numeric.DataType.ShouldBe(ScoreV3DataType.Numeric);
        numeric.Id.ShouldBe("score-1");
        numeric.ProjectId.ShouldBe("proj-1");
        numeric.Name.ShouldBe("quality");
        numeric.Source.ShouldBe(ScoreSource.Api);
        numeric.Environment.ShouldBe("production");
    }

    [Fact]
    public void Deserialize_BooleanScore_WithDiscriminatorNotFirst()
    {
        var score = JsonSerializer.Deserialize<ScoreV3>(ScoreJson("true", "BOOLEAN"));

        var boolean = score.ShouldBeOfType<BooleanScoreV3>();
        boolean.Value.ShouldBeTrue();
        boolean.DataType.ShouldBe(ScoreV3DataType.Boolean);
    }

    [Fact]
    public void Deserialize_CategoricalScore_WithDiscriminatorNotFirst()
    {
        var score = JsonSerializer.Deserialize<ScoreV3>(ScoreJson("\"good\"", "CATEGORICAL"));

        var categorical = score.ShouldBeOfType<CategoricalScoreV3>();
        categorical.Value.ShouldBe("good");
        categorical.DataType.ShouldBe(ScoreV3DataType.Categorical);
    }

    [Fact]
    public void Deserialize_TextScore_WithDiscriminatorNotFirst()
    {
        var score = JsonSerializer.Deserialize<ScoreV3>(ScoreJson("\"free form feedback\"", "TEXT"));

        var text = score.ShouldBeOfType<TextScoreV3>();
        text.Value.ShouldBe("free form feedback");
        text.DataType.ShouldBe(ScoreV3DataType.Text);
    }

    [Fact]
    public void Deserialize_CorrectionScore_WithDiscriminatorNotFirst()
    {
        var score = JsonSerializer.Deserialize<ScoreV3>(ScoreJson("\"corrected answer\"", "CORRECTION"));

        var correction = score.ShouldBeOfType<CorrectionScoreV3>();
        correction.Value.ShouldBe("corrected answer");
        correction.DataType.ShouldBe(ScoreV3DataType.Correction);
    }

    [Fact]
    public void Deserialize_UnknownDataType_Throws()
    {
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<ScoreV3>(ScoreJson("1", "PERCENTAGE")));
    }

    [Fact]
    public void Deserialize_MissingDataType_Throws()
    {
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<ScoreV3>("""{"id":"score-1","value":1}"""));
    }

    [Fact]
    public void Deserialize_Score_WithDetailsAnnotationAndSubjectFields()
    {
        var json = """
                   {
                       "id": "score-1",
                       "projectId": "proj-1",
                       "name": "quality",
                       "source": "ANNOTATION",
                       "timestamp": "2024-05-01T10:00:00Z",
                       "environment": "production",
                       "createdAt": "2024-05-01T10:00:01Z",
                       "updatedAt": "2024-05-01T10:00:02Z",
                       "comment": "looks good",
                       "configId": "config-1",
                       "metadata": { "reviewer": "human" },
                       "authorUserId": "user-1",
                       "queueId": "queue-1",
                       "subject": { "id": "obs-1", "traceId": "trace-1", "kind": "observation" },
                       "value": 1,
                       "dataType": "NUMERIC"
                   }
                   """;

        var score = JsonSerializer.Deserialize<ScoreV3>(json).ShouldBeOfType<NumericScoreV3>();

        score.Source.ShouldBe(ScoreSource.Annotation);
        score.Comment.ShouldBe("looks good");
        score.ConfigId.ShouldBe("config-1");
        score.Metadata.ShouldNotBeNull();
        score.AuthorUserId.ShouldBe("user-1");
        score.QueueId.ShouldBe("queue-1");

        var subject = score.Subject.ShouldBeOfType<ScoreSubjectObservationV3>();
        subject.Id.ShouldBe("obs-1");
        subject.TraceId.ShouldBe("trace-1");
        subject.Kind.ShouldBe(ScoreSubjectV3Kind.Observation);
    }

    [Theory]
    [InlineData("trace", typeof(ScoreSubjectTraceV3), ScoreSubjectV3Kind.Trace)]
    [InlineData("observation", typeof(ScoreSubjectObservationV3), ScoreSubjectV3Kind.Observation)]
    [InlineData("session", typeof(ScoreSubjectSessionV3), ScoreSubjectV3Kind.Session)]
    [InlineData("experiment", typeof(ScoreSubjectExperimentV3), ScoreSubjectV3Kind.Experiment)]
    public void Deserialize_SubjectKinds_WithDiscriminatorNotFirst(string kind, Type expectedType,
        ScoreSubjectV3Kind expectedKind)
    {
        // The kind discriminator is intentionally the last property
        var json = $$"""{ "id": "subject-1", "kind": "{{kind}}" }""";

        var subject = JsonSerializer.Deserialize<ScoreSubjectV3>(json);

        subject.ShouldNotBeNull();
        subject.ShouldBeOfType(expectedType);
        subject.Kind.ShouldBe(expectedKind);
        subject.Id.ShouldBe("subject-1");
    }

    [Fact]
    public void Deserialize_UnknownSubjectKind_Throws()
    {
        Should.Throw<JsonException>(() =>
            JsonSerializer.Deserialize<ScoreSubjectV3>("""{ "id": "subject-1", "kind": "dataset" }"""));
    }

    [Fact]
    public void Serialize_NumericScore_RoundTrips_And_WritesDiscriminator()
    {
        ScoreV3 score = new NumericScoreV3
        {
            Id = "score-1",
            ProjectId = "proj-1",
            Name = "quality",
            Source = ScoreSource.Eval,
            Timestamp = new DateTime(2024, 5, 1, 10, 0, 0, DateTimeKind.Utc),
            Environment = "production",
            CreatedAt = new DateTime(2024, 5, 1, 10, 0, 1, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 5, 1, 10, 0, 2, DateTimeKind.Utc),
            Value = 0.5,
            Subject = new ScoreSubjectTraceV3 { Id = "trace-1" }
        };

        var json = JsonSerializer.Serialize(score);

        json.ShouldContain("\"dataType\":\"NUMERIC\"");
        json.ShouldContain("\"source\":\"EVAL\"");
        json.ShouldContain("\"kind\":\"trace\"");

        var roundTripped = JsonSerializer.Deserialize<ScoreV3>(json).ShouldBeOfType<NumericScoreV3>();
        roundTripped.Value.ShouldBe(0.5);
        roundTripped.Id.ShouldBe("score-1");
        roundTripped.Subject.ShouldBeOfType<ScoreSubjectTraceV3>().Id.ShouldBe("trace-1");
    }

    [Fact]
    public void Serialize_SubjectObservation_RoundTrips_And_WritesDiscriminator()
    {
        ScoreSubjectV3 subject = new ScoreSubjectObservationV3 { Id = "obs-1", TraceId = "trace-1" };

        var json = JsonSerializer.Serialize(subject);

        json.ShouldContain("\"kind\":\"observation\"");
        json.ShouldContain("\"traceId\":\"trace-1\"");

        var roundTripped = JsonSerializer.Deserialize<ScoreSubjectV3>(json)
            .ShouldBeOfType<ScoreSubjectObservationV3>();
        roundTripped.Id.ShouldBe("obs-1");
        roundTripped.TraceId.ShouldBe("trace-1");
    }

    [Fact]
    public void Deserialize_GetScoresV3Response_WithMixedDataTypes()
    {
        var json = """
                   {
                       "data": [
                           {
                               "id": "score-1",
                               "projectId": "proj-1",
                               "name": "accuracy",
                               "source": "EVAL",
                               "timestamp": "2024-05-01T10:00:00Z",
                               "environment": "default",
                               "createdAt": "2024-05-01T10:00:01Z",
                               "updatedAt": "2024-05-01T10:00:02Z",
                               "value": 0.75,
                               "dataType": "NUMERIC"
                           },
                           {
                               "id": "score-2",
                               "projectId": "proj-1",
                               "name": "sentiment",
                               "source": "API",
                               "timestamp": "2024-05-01T11:00:00Z",
                               "environment": "default",
                               "createdAt": "2024-05-01T11:00:01Z",
                               "updatedAt": "2024-05-01T11:00:02Z",
                               "value": "positive",
                               "dataType": "CATEGORICAL"
                           }
                       ],
                       "meta": { "limit": 50, "cursor": "bmV4dC1wYWdl" }
                   }
                   """;

        var response = JsonSerializer.Deserialize<GetScoresV3Response>(json);

        response.ShouldNotBeNull();
        response.Data.Length.ShouldBe(2);
        response.Data[0].ShouldBeOfType<NumericScoreV3>().Value.ShouldBe(0.75);
        response.Data[1].ShouldBeOfType<CategoricalScoreV3>().Value.ShouldBe("positive");
        response.Meta.Limit.ShouldBe(50);
        response.Meta.Cursor.ShouldBe("bmV4dC1wYWdl");
    }
}