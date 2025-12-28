using System.Text.Json;
using System.Text.Json.Serialization;
using Shouldly;
using zborek.Langfuse.Models.LlmConnection;
using zborek.Langfuse.Models.Media;
using zborek.Langfuse.Models.Organization;
using zborek.Langfuse.Models.Project;

namespace zborek.Langfuse.Tests.Models;

public class UpdatedModelsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    #region Organization Tests

    [Fact]
    public void Organization_Should_Serialize_Correctly()
    {
        // Arrange
        var org = new Organization
        {
            Id = "org-123",
            Name = "Test Organization"
        };

        // Act
        var json = JsonSerializer.Serialize(org, JsonOptions);

        // Assert
        json.ShouldContain("\"id\":\"org-123\"");
        json.ShouldContain("\"name\":\"Test Organization\"");
    }

    [Fact]
    public void Organization_Should_Deserialize_Correctly()
    {
        // Arrange
        var json = """
                   {
                     "id": "org-xyz",
                     "name": "My Company"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<Organization>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("org-xyz");
        result.Name.ShouldBe("My Company");
    }

    #endregion

    #region Project with Organization Tests

    [Fact]
    public void Project_Should_Deserialize_WithOrganization()
    {
        // Arrange
        var json = """
                   {
                     "id": "project-123",
                     "name": "My Project",
                     "organization": {
                       "id": "org-456",
                       "name": "My Org"
                     },
                     "metadata": null,
                     "retentionDays": 30
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<ProjectModel>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("project-123");
        result.Name.ShouldBe("My Project");
        result.Organization.ShouldNotBeNull();
        result.Organization.Id.ShouldBe("org-456");
        result.Organization.Name.ShouldBe("My Org");
        result.RetentionDays.ShouldBe(30);
    }

    [Fact]
    public void Project_Should_Serialize_WithOrganization()
    {
        // Arrange
        var project = new ProjectModel
        {
            Id = "project-789",
            Name = "Test Project",
            Organization = new Organization
            {
                Id = "org-abc",
                Name = "Test Org"
            },
            RetentionDays = 90
        };

        // Act
        var json = JsonSerializer.Serialize(project, JsonOptions);

        // Assert
        json.ShouldContain("\"organization\":");
        json.ShouldContain("\"id\":\"org-abc\"");
        json.ShouldContain("\"name\":\"Test Org\"");
    }

    #endregion

    #region LlmConnection Config Tests

    [Fact]
    public void LlmConnection_Should_Deserialize_WithConfig()
    {
        // Arrange
        var json = """
                   {
                     "id": "conn-123",
                     "provider": "bedrock",
                     "adapter": "bedrock",
                     "displaySecretKey": "***",
                     "customModels": ["claude-3-sonnet"],
                     "withDefaultModels": true,
                     "extraHeaderKeys": [],
                     "config": {
                       "region": "us-east-1"
                     },
                     "createdAt": "2024-01-15T10:00:00Z",
                     "updatedAt": "2024-01-15T10:00:00Z"
                   }
                   """;

        // Act
        var result = JsonSerializer.Deserialize<LlmConnection>(json, JsonOptions);

        // Assert
        result.ShouldNotBeNull();
        result.Config.ShouldNotBeNull();
        result.Config["region"]?.ToString().ShouldBe("us-east-1");
    }

    [Fact]
    public void UpsertLlmConnectionRequest_Should_Serialize_WithConfig()
    {
        // Arrange
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "vertex-ai",
            Adapter = LlmAdapter.GoogleVertexAi,
            SecretKey = "secret",
            Config = new Dictionary<string, object>
            {
                ["location"] = "us-central1"
            }
        };

        // Act
        var json = JsonSerializer.Serialize(request, JsonOptions);

        // Assert
        json.ShouldContain("\"config\":");
        json.ShouldContain("\"location\":\"us-central1\"");
    }

    #endregion

    #region MediaContentType New Values Tests

    [Theory]
    [InlineData(MediaContentType.ImageAvif, "IMAGEAVIF")]
    [InlineData(MediaContentType.ImageHeic, "IMAGEHEIC")]
    [InlineData(MediaContentType.AudioOpus, "AUDIOOPUS")]
    [InlineData(MediaContentType.AudioWebm, "AUDIOWEBM")]
    [InlineData(MediaContentType.VideoOgg, "VIDEOOGG")]
    [InlineData(MediaContentType.VideoMpeg, "VIDEOMPEG")]
    [InlineData(MediaContentType.VideoQuicktime, "VIDEOQUICKTIME")]
    [InlineData(MediaContentType.VideoXMsvideo, "VIDEOXMSVIDEO")]
    [InlineData(MediaContentType.VideoXMatroska, "VIDEOXMATROSKA")]
    [InlineData(MediaContentType.TextMarkdown, "TEXTMARKDOWN")]
    [InlineData(MediaContentType.TextXPython, "TEXTXPYTHON")]
    [InlineData(MediaContentType.ApplicationJavascript, "APPLICATIONJAVASCRIPT")]
    [InlineData(MediaContentType.TextXTypescript, "TEXTXTYPESCRIPT")]
    [InlineData(MediaContentType.ApplicationXYaml, "APPLICATIONXYAML")]
    [InlineData(MediaContentType.ApplicationVndOpenxmlformatsOfficedocumentSpreadsheetmlSheet,
        "APPLICATIONVNDOPENXMLFORMATSOFFICEDOCUMENTSPREADSHEETMLSHEET")]
    public void MediaContentType_NewValues_Should_Serialize_Correctly(MediaContentType value, string expected)
    {
        // Arrange
        var testObject = new { ContentType = value };

        // Act
        var json = JsonSerializer.Serialize(testObject);

        // Assert - The enum value is serialized to uppercase string
        json.ShouldContain($"\"{expected}\"");
    }

    [Theory]
    [InlineData("IMAGEAVIF", MediaContentType.ImageAvif)]
    [InlineData("ImageAvif", MediaContentType.ImageAvif)]
    [InlineData("imageavif", MediaContentType.ImageAvif)]
    [InlineData("IMAGEHEIC", MediaContentType.ImageHeic)]
    [InlineData("AUDIOOPUS", MediaContentType.AudioOpus)]
    [InlineData("AUDIOWEBM", MediaContentType.AudioWebm)]
    [InlineData("VIDEOOGG", MediaContentType.VideoOgg)]
    [InlineData("VIDEOMPEG", MediaContentType.VideoMpeg)]
    [InlineData("TEXTMARKDOWN", MediaContentType.TextMarkdown)]
    [InlineData("TEXTXPYTHON", MediaContentType.TextXPython)]
    [InlineData("APPLICATIONJAVASCRIPT", MediaContentType.ApplicationJavascript)]
    public void MediaContentType_NewValues_Should_Deserialize_Correctly(string jsonValue, MediaContentType expected)
    {
        // Arrange
        var json = $"{{\"ContentType\":\"{jsonValue}\"}}";

        // Act
        var result = JsonSerializer.Deserialize<TestMediaContentType>(json);

        // Assert
        result.ShouldNotBeNull();
        result.ContentType.ShouldBe(expected);
    }

    #endregion

    #region Test Classes

    private class TestMediaContentType
    {
        public MediaContentType ContentType { get; set; }
    }

    #endregion
}
