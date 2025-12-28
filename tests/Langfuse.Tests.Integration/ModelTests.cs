using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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
        var client = CreateClient();
        var modelName = $"custom-model-{Guid.NewGuid():N}";
        var request = new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            InputPrice = 0.001,
            OutputPrice = 0.002,
            Unit = ModelUsageUnit.Tokens
        };

        var model = await client.CreateModelAsync(request);

        model.ShouldNotBeNull();
        model.Id.ShouldNotBeNull();
        model.ModelName.ShouldBe(modelName);
        model.InputPrice.ShouldBe(0.001);
        model.OutputPrice.ShouldBe(0.002);
        model.Unit.ShouldBe(ModelUsageUnit.Tokens);
        model.IsLangfuseManaged.ShouldBeFalse();
    }

    [Fact]
    public async Task GetModelAsync_ReturnsModel()
    {
        var client = CreateClient();
        var modelName = $"custom-model-{Guid.NewGuid():N}";
        var created = await client.CreateModelAsync(new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            InputPrice = 0.005,
            Unit = ModelUsageUnit.Tokens
        });

        var model = await client.GetModelAsync(created.Id);

        model.ShouldNotBeNull();
        model.Id.ShouldBe(created.Id);
        model.ModelName.ShouldBe(modelName);
        model.InputPrice.ShouldBe(0.005);
    }

    [Fact]
    public async Task GetModelListAsync_ReturnsPaginatedList()
    {
        var client = CreateClient();
        var prefix = $"list-model-{Guid.NewGuid():N}";

        await client.CreateModelAsync(new CreateModelRequest
        {
            ModelName = $"{prefix}-1",
            MatchPattern = $"(?i)^{prefix}-1$",
            InputPrice = 0.001,
            Unit = ModelUsageUnit.Tokens
        });
        await client.CreateModelAsync(new CreateModelRequest
        {
            ModelName = $"{prefix}-2",
            MatchPattern = $"(?i)^{prefix}-2$",
            InputPrice = 0.001,
            Unit = ModelUsageUnit.Tokens
        });

        var result1 = await client.GetModelListAsync(new ModelListRequest { Page = 1, Limit = 1 });
        var result2 = await client.GetModelListAsync(new ModelListRequest { Page = 2, Limit = 1 });

        result1.ShouldNotBeNull();
        result1.Data.ShouldNotBeNull();
        result1.Data.Length.ShouldBe(1);

        result2.ShouldNotBeNull();
        result2.Data.ShouldNotBeNull();
        result2.Data.Length.ShouldBe(1);

        result1.Data[0].Id.ShouldNotBe(result2.Data[0].Id);
    }

    [Fact]
    public async Task DeleteModelAsync_DeletesCustomModel()
    {
        var client = CreateClient();
        var modelName = $"custom-model-{Guid.NewGuid():N}";
        var created = await client.CreateModelAsync(new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            Unit = ModelUsageUnit.Tokens,
            InputPrice = 0.001
        });

        await client.DeleteModelAsync(created.Id);

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetModelAsync(created.Id));
        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreateModelAsync_WithPricing_SetsPricesCorrectly()
    {
        var client = CreateClient();
        var modelName = $"priced-model-{Guid.NewGuid():N}";
        var request = new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            InputPrice = 0.00001,
            OutputPrice = 0.00002,
            Unit = ModelUsageUnit.Tokens
        };

        var model = await client.CreateModelAsync(request);

        model.InputPrice.ShouldBe(0.00001);
        model.OutputPrice.ShouldBe(0.00002);
    }

    [Fact]
    public async Task CreateModelAsync_WithStartDate_SetsDateCorrectly()
    {
        var client = CreateClient();
        var modelName = $"dated-model-{Guid.NewGuid():N}";
        var startDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var request = new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = $"(?i)^{modelName}$",
            StartDate = startDate,
            Unit = ModelUsageUnit.Tokens,
            InputPrice = 0.001
        };

        var model = await client.CreateModelAsync(request);

        model.StartDate.ShouldNotBeNull();
        model.StartDate.Value.Date.ShouldBe(startDate.Date);
    }

    [Fact]
    public async Task GetModelAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetModelAsync("non-existent-model-id"));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetModelListAsync_IncludesLangfuseManagedModels()
    {
        var client = CreateClient();

        var result = await client.GetModelListAsync(new ModelListRequest { Page = 1, Limit = 100 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldContain(m => m.IsLangfuseManaged);
    }

    [Fact]
    public async Task CreateModelAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var modelName = $"comprehensive-model-{Guid.NewGuid():N}";
        var matchPattern = $"(?i)^{modelName}$";
        var inputPrice = 0.00001;
        var outputPrice = 0.00002;
        var startDate = new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        var request = new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = matchPattern,
            InputPrice = inputPrice,
            OutputPrice = outputPrice,
            Unit = ModelUsageUnit.Tokens,
            StartDate = startDate
        };

        var model = await client.CreateModelAsync(request);

        model.Id.ShouldNotBeNullOrEmpty();
        model.ModelName.ShouldBe(modelName);
        model.MatchPattern.ShouldBe(matchPattern);
        model.InputPrice.ShouldBe(inputPrice);
        model.OutputPrice.ShouldBe(outputPrice);
        model.Unit.ShouldBe(ModelUsageUnit.Tokens);
        model.StartDate.ShouldNotBeNull();
        model.StartDate.Value.Date.ShouldBe(startDate.Date);
        model.IsLangfuseManaged.ShouldBeFalse();
    }

    [Fact]
    public async Task GetModelAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var modelName = $"get-comprehensive-model-{Guid.NewGuid():N}";
        var matchPattern = $"(?i)^{modelName}$";
        var inputPrice = 0.005;
        var outputPrice = 0.01;

        var created = await client.CreateModelAsync(new CreateModelRequest
        {
            ModelName = modelName,
            MatchPattern = matchPattern,
            InputPrice = inputPrice,
            OutputPrice = outputPrice,
            Unit = ModelUsageUnit.Tokens
        });

        var model = await client.GetModelAsync(created.Id);

        model.Id.ShouldBe(created.Id);
        model.ModelName.ShouldBe(modelName);
        model.MatchPattern.ShouldBe(matchPattern);
        model.InputPrice.ShouldBe(inputPrice);
        model.OutputPrice.ShouldBe(outputPrice);
        model.Unit.ShouldBe(ModelUsageUnit.Tokens);
        model.IsLangfuseManaged.ShouldBeFalse();
    }
}