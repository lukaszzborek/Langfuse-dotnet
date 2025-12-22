using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
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
        Assert.NotNull(config);
        Assert.NotNull(config.Id);
        Assert.Equal(configName, config.Name);
        Assert.Equal(ScoreDataType.Numeric, config.DataType);
        Assert.Equal(0, config.MinValue);
        Assert.Equal(100, config.MaxValue);
        Assert.Equal("Numeric score from 0 to 100", config.Description);
        Assert.False(config.IsArchived);
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
        Assert.NotNull(config);
        Assert.Equal(configName, config.Name);
        Assert.Equal(ScoreDataType.Categorical, config.DataType);
        Assert.NotNull(config.Categories);
        Assert.Equal(3, config.Categories.Length);
        Assert.Contains(config.Categories, c => c.Label == "Positive" && c.Value == 1);
        Assert.Contains(config.Categories, c => c.Label == "Neutral" && c.Value == 0);
        Assert.Contains(config.Categories, c => c.Label == "Negative" && c.Value == -1);
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
        Assert.NotNull(config);
        Assert.Equal(configName, config.Name);
        Assert.Equal(ScoreDataType.Boolean, config.DataType);
        Assert.Equal("Pass/Fail evaluation", config.Description);
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
        Assert.NotNull(config);
        Assert.Equal(created.Id, config.Id);
        Assert.Equal(configName, config.Name);
        Assert.Equal(ScoreDataType.Numeric, config.DataType);
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
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Length >= 2);
        Assert.Contains(result.Data, c => c.Name.StartsWith(prefix));
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
        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal("Updated description", updated.Description);
        Assert.Equal(100, updated.MaxValue);
        Assert.Equal(0, updated.MinValue); // Unchanged
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
        Assert.True(updated.IsArchived);
    }

    [Fact]
    public async Task GetScoreConfigAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetScoreConfigAsync("non-existent-config-id"));

        Assert.Equal(404, exception.StatusCode);
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
        Assert.Equal(newName, updated.Name);
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
        Assert.NotNull(updated.Categories);
        Assert.Equal(4, updated.Categories.Length);
        Assert.Contains(updated.Categories, c => c.Label == "Excellent");
    }
}
