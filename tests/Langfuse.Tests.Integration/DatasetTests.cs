using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        var request = new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Test dataset for integration tests",
            Metadata = new { environment = "test", version = 1 }
        };

        var dataset = await client.CreateDatasetAsync(request);

        dataset.ShouldNotBeNull();
        dataset.Name.ShouldBe(datasetName);
        dataset.Description.ShouldBe("Test dataset for integration tests");
        dataset.Id.ShouldNotBeNull();
        dataset.ProjectId.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetDatasetAsync_ReturnsDataset()
    {
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Dataset for retrieval test"
        });

        var dataset = await client.GetDatasetAsync(datasetName);

        dataset.ShouldNotBeNull();
        dataset.Name.ShouldBe(datasetName);
        dataset.Description.ShouldBe("Dataset for retrieval test");
    }

    [Fact]
    public async Task GetDatasetListAsync_ReturnsPaginatedList()
    {
        var client = CreateClient();
        var prefix = $"list-test-{Guid.NewGuid():N}";

        var dataset1 = await client.CreateDatasetAsync(new CreateDatasetRequest { Name = $"{prefix}-1" });
        var dataset2 = await client.CreateDatasetAsync(new CreateDatasetRequest { Name = $"{prefix}-2" });

        var result = await client.GetDatasetListAsync(new DatasetListRequest { Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Length.ShouldBeGreaterThanOrEqualTo(2);
        result.Data.ShouldContain(d => d.Id == dataset1.Id && d.Name == $"{prefix}-1");
        result.Data.ShouldContain(d => d.Id == dataset2.Id && d.Name == $"{prefix}-2");
    }

    [Fact]
    public async Task GetDatasetRunsAsync_ReturnsEmptyList()
    {
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest { Name = datasetName });

        var runs = await client.GetDatasetRunsAsync(datasetName, new DatasetRunListRequest { Page = 1, Limit = 10 });

        runs.ShouldNotBeNull();
        runs.Data.ShouldNotBeNull();
        runs.Data.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetDatasetRunAsync_ThrowsNotFound()
    {
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest { Name = datasetName });

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetDatasetRunAsync(datasetName, "non-existent-run"));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task DeleteDatasetRunAsync_ThrowsNotFound()
    {
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";
        await client.CreateDatasetAsync(new CreateDatasetRequest { Name = datasetName });

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.DeleteDatasetRunAsync(datasetName, "non-existent-run"));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreateDatasetAsync_WithSameName_UpsertsBehavior()
    {
        var client = CreateClient();
        var datasetName = $"test-dataset-{Guid.NewGuid():N}";

        var dataset1 = await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Original description"
        });

        var dataset2 = await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = "Updated description"
        });

        dataset2.Id.ShouldBe(dataset1.Id);
        dataset2.Description.ShouldBe("Updated description");
    }

    [Fact]
    public async Task CreateDatasetAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);
        var datasetName = $"comprehensive-dataset-{Guid.NewGuid():N}";
        var description = "Comprehensive test dataset for field validation";
        var metadata = new { category = "integration-test", version = 1, nested = new { key = "value" } };

        var request = new CreateDatasetRequest
        {
            Name = datasetName,
            Description = description,
            Metadata = metadata
        };

        var dataset = await client.CreateDatasetAsync(request);

        dataset.Id.ShouldNotBeNullOrEmpty();
        dataset.Name.ShouldBe(datasetName);
        dataset.Description.ShouldBe(description);
        dataset.ProjectId.ShouldNotBeNullOrEmpty();
        dataset.Metadata.ShouldNotBeNull();
        dataset.CreatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        dataset.UpdatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetDatasetAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);
        var datasetName = $"get-comprehensive-dataset-{Guid.NewGuid():N}";
        var description = "Dataset for get field validation";
        var metadata = new { source = "integration-test" };

        await client.CreateDatasetAsync(new CreateDatasetRequest
        {
            Name = datasetName,
            Description = description,
            Metadata = metadata
        });

        var dataset = await client.GetDatasetAsync(datasetName);

        dataset.Id.ShouldNotBeNullOrEmpty();
        dataset.Name.ShouldBe(datasetName);
        dataset.Description.ShouldBe(description);
        dataset.ProjectId.ShouldNotBeNullOrEmpty();
        dataset.Metadata.ShouldNotBeNull();
        dataset.CreatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        dataset.UpdatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
    }
}