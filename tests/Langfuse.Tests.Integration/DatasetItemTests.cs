using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Dataset;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class DatasetItemTests
{
    private readonly LangfuseTestFixture _fixture;

    public DatasetItemTests(LangfuseTestFixture fixture)
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

    private async Task<string> CreateTestDatasetAsync(ILangfuseClient client)
    {
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest { Name = datasetName });
        return datasetName;
    }

    [Fact]
    public async Task CreateDatasetItemAsync_CreatesItem()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = await CreateTestDatasetAsync(client);
        var request = new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { query = "What is the capital of France?" },
            ExpectedOutput = new { answer = "Paris" },
            Metadata = new { category = "geography", difficulty = "easy" }
        };

        // Act
        var item = await client.CreateDatasetItemAsync(request);

        // Assert
        item.ShouldNotBeNull();
        item.Id.ShouldNotBeNull();
        item.DatasetName.ShouldBe(datasetName);
        item.Input.ShouldNotBeNull();
        item.ExpectedOutput.ShouldNotBeNull();
        item.Status.ShouldBe(DatasetStatus.Active);
    }

    [Fact]
    public async Task GetDatasetItemAsync_ReturnsItem()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = await CreateTestDatasetAsync(client);
        var created = await client.CreateDatasetItemAsync(new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { prompt = "Hello world" },
            ExpectedOutput = new { response = "Hi there" }
        });

        // Act
        var item = await client.GetDatasetItemAsync(created.Id);

        // Assert
        item.ShouldNotBeNull();
        item.Id.ShouldBe(created.Id);
        item.DatasetName.ShouldBe(datasetName);
    }

    [Fact]
    public async Task GetDatasetItemListAsync_ReturnsPaginatedList()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = await CreateTestDatasetAsync(client);

        // Create multiple items
        await client.CreateDatasetItemAsync(new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { query = "Item 1" }
        });
        await client.CreateDatasetItemAsync(new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { query = "Item 2" }
        });
        await client.CreateDatasetItemAsync(new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { query = "Item 3" }
        });

        // Act
        var result = await client.GetDatasetItemListAsync(new DatasetItemListRequest
        {
            DatasetName = datasetName,
            Page = 1,
            Limit = 10
        });

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Length.ShouldBe(3);
        result.Data.ShouldAllBe(item => item.DatasetName == datasetName);
    }

    [Fact]
    public async Task DeleteDatasetItemAsync_DeletesItem()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = await CreateTestDatasetAsync(client);
        var created = await client.CreateDatasetItemAsync(new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { query = "To be deleted" }
        });

        // Act
        var deleteResult = await client.DeleteDatasetItemAsync(created.Id);

        // Assert
        deleteResult.ShouldNotBeNull();

        // Verify item is deleted
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetDatasetItemAsync(created.Id));
        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreateDatasetItemAsync_WithCustomId_UsesProvidedId()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = await CreateTestDatasetAsync(client);
        var customId = $"custom-{Guid.NewGuid():N}";
        var request = new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Id = customId,
            Input = new { query = "Custom ID test" }
        };

        // Act
        var item = await client.CreateDatasetItemAsync(request);

        // Assert
        item.Id.ShouldBe(customId);
    }

    [Fact]
    public async Task CreateDatasetItemAsync_WithArchivedStatus_CreatesArchivedItem()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = await CreateTestDatasetAsync(client);
        var request = new CreateDatasetItemRequest
        {
            DatasetName = datasetName,
            Input = new { query = "Archived item" },
            Status = DatasetStatus.Archived
        };

        // Act
        var item = await client.CreateDatasetItemAsync(request);

        // Assert
        item.Status.ShouldBe(DatasetStatus.Archived);
    }

    [Fact]
    public async Task GetDatasetItemAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetDatasetItemAsync("non-existent-item-id"));

        exception.StatusCode.ShouldBe(404);
    }
}