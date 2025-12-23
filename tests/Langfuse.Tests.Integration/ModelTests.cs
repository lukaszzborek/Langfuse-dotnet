using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Model;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class ModelTests
{
    private readonly LangfuseTestFixture _fixture;

    public ModelTests(LangfuseTestFixture fixture)
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
    public async Task CreateModelAsync_CreatesCustomModel()
    {
        // Arrange
        var client = CreateClient();
        var modelName = $"custom-model-{Guid.NewGuid():N}";
        var request = new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            InputPrice = 0.001m,
            OutputPrice = 0.002m,
            Unit = ModelUsageUnit.Tokens
        };

        // Act
        var model = await client.CreateModelAsync(request);

        // Assert
        Assert.NotNull(model);
        Assert.NotNull(model.Id);
        Assert.Equal(modelName, model.ModelName);
        Assert.Equal(0.001m, model.InputPrice);
        Assert.Equal(0.002m, model.OutputPrice);
        Assert.Equal(ModelUsageUnit.Tokens, model.Unit);
        Assert.False(model.IsLangfuseManaged);
    }

    [Fact]
    public async Task GetModelAsync_ReturnsModel()
    {
        // Arrange
        var client = CreateClient();
        var modelName = $"custom-model-{Guid.NewGuid():N}";
        var created = await client.CreateModelAsync(new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            InputPrice = 0.005m,
            Unit = ModelUsageUnit.Tokens
        });

        // Act
        var model = await client.GetModelAsync(created.Id);

        // Assert
        Assert.NotNull(model);
        Assert.Equal(created.Id, model.Id);
        Assert.Equal(modelName, model.ModelName);
        Assert.Equal(0.005m, model.InputPrice);
    }

    [Fact]
    public async Task GetModelListAsync_ReturnsPaginatedList()
    {
        // Arrange
        var client = CreateClient();

        // Act - List models without creating any (uses Langfuse's predefined models)
        var result = await client.GetModelListAsync(new ModelListRequest { Page = 1, Limit = 100 });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        // Langfuse has many predefined models
        Assert.True(result.Data.Length >= 2);
        Assert.True(result.Meta.TotalItems >= result.Data.Length);
    }

    [Fact]
    public async Task DeleteModelAsync_DeletesCustomModel()
    {
        // Arrange
        var client = CreateClient();
        var modelName = $"custom-model-{Guid.NewGuid():N}";
        var created = await client.CreateModelAsync(new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            Unit = ModelUsageUnit.Tokens,
            InputPrice = 0.001m
        });

        // Act
        await client.DeleteModelAsync(created.Id);

        // Assert - Verify model is deleted
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetModelAsync(created.Id));
        Assert.Equal(404, exception.StatusCode);
    }

    [Fact]
    public async Task CreateModelAsync_WithPricing_SetsPricesCorrectly()
    {
        // Arrange
        var client = CreateClient();
        var modelName = $"priced-model-{Guid.NewGuid():N}";
        // Note: Cannot set both input/output prices AND totalPrice - they are mutually exclusive
        var request = new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            InputPrice = 0.00001m,
            OutputPrice = 0.00002m,
            Unit = ModelUsageUnit.Tokens
        };

        // Act
        var model = await client.CreateModelAsync(request);

        // Assert
        Assert.Equal(0.00001m, model.InputPrice);
        Assert.Equal(0.00002m, model.OutputPrice);
    }

    [Fact]
    public async Task CreateModelAsync_WithStartDate_SetsDateCorrectly()
    {
        // Arrange
        var client = CreateClient();
        var modelName = $"dated-model-{Guid.NewGuid():N}";
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            StartDate = startDate,
            Unit = ModelUsageUnit.Tokens,
            InputPrice = 0.001m
        };

        // Act
        var model = await client.CreateModelAsync(request);

        // Assert
        Assert.NotNull(model.StartDate);
        Assert.Equal(startDate.Date, model.StartDate.Value.Date);
    }

    [Fact]
    public async Task GetModelAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetModelAsync("non-existent-model-id"));

        Assert.Equal(404, exception.StatusCode);
    }

    [Fact]
    public async Task GetModelListAsync_IncludesLangfuseManagedModels()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var result = await client.GetModelListAsync(new ModelListRequest { Page = 1, Limit = 100 });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        // Langfuse includes many predefined models
        Assert.Contains(result.Data, m => m.IsLangfuseManaged);
    }
}