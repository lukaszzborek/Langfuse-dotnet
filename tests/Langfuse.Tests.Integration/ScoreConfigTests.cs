using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class ScoreConfigTests
{
    private readonly LangfuseTestFixture _fixture;

    public ScoreConfigTests(LangfuseTestFixture fixture)
    {
        _fixture = fixture;
    }

    private ILangfuseClient CreateClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLangfuse(config =>
        {
            config.Url = _fixture.LangfuseBaseUrl;
            config.PublicKey = _fixture.PublicKey;
            config.SecretKey = _fixture.SecretKey;
            config.BatchMode = false;
        });

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ILangfuseClient>();
    }

    [Fact]
    public async Task CreateScoreConfigAsync_CreatesNumericConfig()
    {
        // Arrange
        var client = CreateClient();
        var configName = $"nc-{Guid.NewGuid():N}"[..20];
        var request = new CreateScoreConfigRequest
        {
            Name = configName,
            DataType = ScoreDataType.Numeric,
            Description = "Numeric score from 0 to 100",
            MinValue = 0,
            MaxValue = 100
        };

        // Act
        var config = await client.CreateScoreConfigAsync(request);

        // Assert
        config.ShouldNotBeNull();
        config.Id.ShouldNotBeNull();
        config.Name.ShouldBe(configName);
        config.DataType.ShouldBe(ScoreDataType.Numeric);
        config.MinValue.ShouldBe(0);
        config.MaxValue.ShouldBe(100);
        config.Description.ShouldBe("Numeric score from 0 to 100");
        config.IsArchived.ShouldBeFalse();
    }

    [Fact]
    public async Task CreateScoreConfigAsync_CreatesCategoricalConfig()
    {
        // Arrange
        var client = CreateClient();
        var configName = $"cc-{Guid.NewGuid():N}"[..20];
        var request = new CreateScoreConfigRequest
        {
            Name = configName,
            DataType = ScoreDataType.Categorical,
            Description = "Sentiment categories",
            Categories =
            [
                new ConfigCategory { Label = "Positive", Value = 1 },
                new ConfigCategory { Label = "Neutral", Value = 0 },
                new ConfigCategory { Label = "Negative", Value = -1 }
            ]
        };

        // Act
        var config = await client.CreateScoreConfigAsync(request);

        // Assert
        config.ShouldNotBeNull();
        config.Name.ShouldBe(configName);
        config.DataType.ShouldBe(ScoreDataType.Categorical);
        config.Categories.ShouldNotBeNull();
        config.Categories.Length.ShouldBe(3);
        config.Categories.ShouldContain(c => c.Label == "Positive" && c.Value == 1);
        config.Categories.ShouldContain(c => c.Label == "Neutral" && c.Value == 0);
        config.Categories.ShouldContain(c => c.Label == "Negative" && c.Value == -1);
    }

    [Fact]
    public async Task CreateScoreConfigAsync_CreatesBooleanConfig()
    {
        // Arrange
        var client = CreateClient();
        var configName = $"bc-{Guid.NewGuid():N}"[..20];
        var request = new CreateScoreConfigRequest
        {
            Name = configName,
            DataType = ScoreDataType.Boolean,
            Description = "Pass/Fail evaluation"
        };

        // Act
        var config = await client.CreateScoreConfigAsync(request);

        // Assert
        config.ShouldNotBeNull();
        config.Name.ShouldBe(configName);
        config.DataType.ShouldBe(ScoreDataType.Boolean);
        config.Description.ShouldBe("Pass/Fail evaluation");
    }

    [Fact]
    public async Task GetScoreConfigAsync_ReturnsConfig()
    {
        // Arrange
        var client = CreateClient();
        var configName = $"gc-{Guid.NewGuid():N}"[..20];
        var created = await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
        {
            Name = configName,
            DataType = ScoreDataType.Numeric,
            MinValue = 1,
            MaxValue = 5
        });

        // Act
        var config = await client.GetScoreConfigAsync(created.Id);

        // Assert
        config.ShouldNotBeNull();
        config.Id.ShouldBe(created.Id);
        config.Name.ShouldBe(configName);
        config.DataType.ShouldBe(ScoreDataType.Numeric);
    }

    [Fact]
    public async Task GetScoreConfigListAsync_ReturnsPaginatedList()
    {
        // Arrange
        var client = CreateClient();
        var prefix = $"lc-{Guid.NewGuid():N}"[..15];

        // Create multiple configs
        await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
        {
            Name = $"{prefix}-1",
            DataType = ScoreDataType.Numeric
        });
        await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
        {
            Name = $"{prefix}-2",
            DataType = ScoreDataType.Boolean
        });

        // Act
        var result = await client.GetScoreConfigListAsync(new ScoreConfigListRequest { Offset = 0, Limit = 50 });

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        (result.Data.Length >= 2).ShouldBeTrue();
        result.Data.ShouldContain(c => c.Name.StartsWith(prefix));
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_UpdatesPartially()
    {
        // Arrange
        var client = CreateClient();
        var configName = $"uc-{Guid.NewGuid():N}"[..20];
        var created = await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
        {
            Name = configName,
            DataType = ScoreDataType.Numeric,
            Description = "Original description",
            MinValue = 0,
            MaxValue = 10
        });

        // Act
        var updated = await client.UpdateScoreConfigAsync(created.Id, new UpdateScoreConfigRequest
        {
            Description = "Updated description",
            MaxValue = 100
        });

        // Assert
        updated.ShouldNotBeNull();
        updated.Id.ShouldBe(created.Id);
        updated.Description.ShouldBe("Updated description");
        updated.MaxValue.ShouldBe(100);
        updated.MinValue.ShouldBe(0); // Unchanged
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_ArchivesConfig()
    {
        // Arrange
        var client = CreateClient();
        var configName = $"ac-{Guid.NewGuid():N}"[..20];
        var created = await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
        {
            Name = configName,
            DataType = ScoreDataType.Boolean
        });

        // Act
        var updated = await client.UpdateScoreConfigAsync(created.Id, new UpdateScoreConfigRequest
        {
            IsArchived = true
        });

        // Assert
        updated.IsArchived.ShouldBeTrue();
    }

    [Fact]
    public async Task GetScoreConfigAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetScoreConfigAsync("non-existent-config-id"));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_UpdatesName()
    {
        // Arrange
        var client = CreateClient();
        var originalName = $"on-{Guid.NewGuid():N}"[..20];
        var newName = $"nn-{Guid.NewGuid():N}"[..20];
        var created = await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
        {
            Name = originalName,
            DataType = ScoreDataType.Numeric
        });

        // Act
        var updated = await client.UpdateScoreConfigAsync(created.Id, new UpdateScoreConfigRequest
        {
            Name = newName
        });

        // Assert
        updated.Name.ShouldBe(newName);
    }

    [Fact]
    public async Task UpdateScoreConfigAsync_UpdatesCategoricalCategories()
    {
        // Arrange
        var client = CreateClient();
        var configName = $"cu-{Guid.NewGuid():N}"[..20];
        var created = await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
        {
            Name = configName,
            DataType = ScoreDataType.Categorical,
            Categories =
            [
                new ConfigCategory { Label = "Good", Value = 1 },
                new ConfigCategory { Label = "Bad", Value = 0 }
            ]
        });

        // Act
        var updated = await client.UpdateScoreConfigAsync(created.Id, new UpdateScoreConfigRequest
        {
            Categories =
            [
                new ConfigCategory { Label = "Excellent", Value = 2 },
                new ConfigCategory { Label = "Good", Value = 1 },
                new ConfigCategory { Label = "Fair", Value = 0 },
                new ConfigCategory { Label = "Poor", Value = -1 }
            ]
        });

        // Assert
        updated.Categories.ShouldNotBeNull();
        updated.Categories.Length.ShouldBe(4);
        updated.Categories.ShouldContain(c => c.Label == "Excellent");
    }
}