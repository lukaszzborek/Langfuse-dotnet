using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Dataset;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class DatasetTests
{
    private readonly LangfuseTestFixture _fixture;

    public DatasetTests(LangfuseTestFixture fixture)
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
    public async Task CreateDatasetAsync_CreatesNewDataset()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        var request = new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Test dataset for integration tests",
            Metadata = new { environment = "test", version = 1 }
        };

        // Act
        var dataset = await client.CreateDatasetAsync(request);

        // Assert
        Assert.NotNull(dataset);
        Assert.Equal(datasetName, dataset.Name);
        Assert.Equal("Test dataset for integration tests", dataset.Description);
        Assert.NotNull(dataset.Id);
        Assert.NotNull(dataset.ProjectId);
    }

    [Fact]
    public async Task GetDatasetAsync_ReturnsDataset()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Dataset for retrieval test"
        });

        // Act
        var dataset = await client.GetDatasetAsync(datasetName);

        // Assert
        Assert.NotNull(dataset);
        Assert.Equal(datasetName, dataset.Name);
        Assert.Equal("Dataset for retrieval test", dataset.Description);
    }

    [Fact]
    public async Task GetDatasetListAsync_ReturnsPaginatedList()
    {
        // Arrange
        var client = CreateClient();
        var prefix = $"list-test-{Guid.NewGuid():N}";

        // Create multiple datasets
        await client.CreateDatasetAsync(new CreateDatasetRequest { Name = $"{prefix}-1" });
        await client.CreateDatasetAsync(new CreateDatasetRequest { Name = $"{prefix}-2" });

        // Act
        var result = await client.GetDatasetListAsync(new DatasetListRequest { Page = 1, Limit = 50 });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Length >= 2);
        Assert.Contains(result.Data, d => d.Name.StartsWith(prefix));
    }

    [Fact]
    public async Task GetDatasetRunsAsync_ReturnsEmptyList()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest { Name = datasetName });

        // Act
        var runs = await client.GetDatasetRunsAsync(datasetName, new DatasetRunListRequest { Page = 1, Limit = 10 });

        // Assert
        Assert.NotNull(runs);
        Assert.NotNull(runs.Data);
        Assert.Empty(runs.Data);
    }

    [Fact]
    public async Task GetDatasetRunAsync_ThrowsNotFound()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest { Name = datasetName });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetDatasetRunAsync(datasetName, "non-existent-run"));

        Assert.Equal(404, exception.StatusCode);
    }

    [Fact]
    public async Task DeleteDatasetRunAsync_ThrowsNotFound()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest { Name = datasetName });

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.DeleteDatasetRunAsync(datasetName, "non-existent-run"));

        Assert.Equal(404, exception.StatusCode);
    }

    [Fact]
    public async Task CreateDatasetAsync_WithSameName_UpsertsBehavior()
    {
        // Arrange
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";

        // Act - Create first
        var dataset1 = await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Original description"
        });

        // Act - Create again with same name (upsert)
        var dataset2 = await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Updated description"
        });

        // Assert - Should have same ID (upsert behavior)
        Assert.Equal(dataset1.Id, dataset2.Id);
        Assert.Equal("Updated description", dataset2.Description);
    }
}
