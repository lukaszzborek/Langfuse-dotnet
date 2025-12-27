using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Prompt;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class PromptTests
{
    private readonly LangfuseTestFixture _fixture;

    public PromptTests(LangfuseTestFixture fixture)
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
    public async Task CreatePromptAsync_CreatesTextPrompt()
    {
        // Arrange
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";
        var request = new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Hello {{name}}, how can I help you today?",
            Config = new { temperature = 0.7, model = "gpt-4" },
            Labels = ["test"],
            Tags = ["integration-test"]
        };

        // Act
        var prompt = await client.CreatePromptAsync(request);

        // Assert
        prompt.ShouldNotBeNull();
        prompt.Name.ShouldBe(promptName);
        prompt.Version.ShouldBe(1);
        prompt.Type.ShouldBe("text");
        prompt.ShouldBeOfType<TextPrompt>();
        var textPrompt = (TextPrompt)prompt;
        textPrompt.PromptText.ShouldBe("Hello {{name}}, how can I help you today?");
    }

    [Fact]
    public async Task CreatePromptAsync_CreatesChatPrompt()
    {
        // Arrange
        var client = CreateClient();
        var promptName = $"test-chat-prompt-{Guid.NewGuid():N}";
        var request = new CreateChatPromptRequest
        {
            Name = promptName,
            Prompt =
            [
                new ChatMessage { Role = "system", Content = "You are a helpful assistant." },
                new ChatMessage { Role = "user", Content = "{{user_input}}" }
            ],
            Config = new { temperature = 0.5 },
            Labels = ["test", "chat"]
        };

        // Act
        var prompt = await client.CreatePromptAsync(request);

        // Assert
        prompt.ShouldNotBeNull();
        prompt.Name.ShouldBe(promptName);
        prompt.Version.ShouldBe(1);
        prompt.Type.ShouldBe("chat");
        prompt.ShouldBeOfType<ChatPrompt>();
    }

    [Fact]
    public async Task GetPromptAsync_ReturnsByName()
    {
        // Arrange
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";
        // The API defaults to 'production' label when fetching without version/label
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Test prompt content",
            Labels = ["production"]
        });

        // Act
        var prompt = await client.GetPromptAsync(promptName);

        // Assert
        prompt.ShouldNotBeNull();
        prompt.Name.ShouldBe(promptName);
        prompt.Version.ShouldBe(1);
    }

    [Fact]
    public async Task GetPromptAsync_ReturnsByVersion()
    {
        // Arrange
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";

        // Create version 1
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Version 1 content"
        });

        // Create version 2
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Version 2 content"
        });

        // Act - Get specific version
        var promptV1 = await client.GetPromptAsync(promptName, 1);
        var promptV2 = await client.GetPromptAsync(promptName, 2);

        // Assert
        promptV1.Version.ShouldBe(1);
        promptV2.Version.ShouldBe(2);
        promptV1.ShouldBeOfType<TextPrompt>();
        promptV2.ShouldBeOfType<TextPrompt>();
        ((TextPrompt)promptV1).PromptText.ShouldBe("Version 1 content");
        ((TextPrompt)promptV2).PromptText.ShouldBe("Version 2 content");
    }

    [Fact]
    public async Task GetPromptAsync_ReturnsByLabel()
    {
        // Arrange
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";

        // Create version 1 with production label
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Production content",
            Labels = ["production"]
        });

        // Create version 2 without production label
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Development content",
            Labels = ["development"]
        });

        // Act - Get by label
        var prodPrompt = await client.GetPromptAsync(promptName, label: "production");

        // Assert
        prodPrompt.Version.ShouldBe(1);
        prodPrompt.Labels.ShouldContain("production");
    }

    [Fact]
    public async Task GetPromptListAsync_ReturnsPaginatedList()
    {
        // Arrange
        var client = CreateClient();
        var prefix = $"list-prompt-{Guid.NewGuid():N}";

        // Create multiple prompts
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = $"{prefix}-1",
            Prompt = "Prompt 1"
        });
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = $"{prefix}-2",
            Prompt = "Prompt 2"
        });

        // Act
        var result = await client.GetPromptListAsync(new PromptListRequest { Page = 1, Limit = 50 });

        // Assert
        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        (result.Data.Length >= 2).ShouldBeTrue();
    }

    [Fact]
    public async Task UpdatePromptVersionAsync_UpdatesLabels()
    {
        // Arrange
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Test content",
            Labels = ["draft"]
        });

        // Act
        var updated = await client.UpdatePromptVersionAsync(promptName, 1, new UpdatePromptVersionRequest
        {
            NewLabels = ["production", "active"]
        });

        // Assert
        updated.ShouldNotBeNull();
        updated.Labels.ShouldContain("production");
        updated.Labels.ShouldContain("active");
    }

    [Fact]
    public async Task GetPromptAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();

        // Act & Assert
        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetPromptAsync("non-existent-prompt"));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreatePromptAsync_WithTags_SetsTagsCorrectly()
    {
        // Arrange
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";
        var request = new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Tagged prompt",
            Tags = ["tag1", "tag2", "tag3"]
        };

        // Act
        var prompt = await client.CreatePromptAsync(request);

        // Assert
        prompt.Tags.Count.ShouldBe(3);
        prompt.Tags.ShouldContain("tag1");
        prompt.Tags.ShouldContain("tag2");
        prompt.Tags.ShouldContain("tag3");
    }

    [Fact]
    public async Task GetPromptListAsync_FiltersByName()
    {
        // Arrange
        var client = CreateClient();
        var uniqueName = $"unique-prompt-{Guid.NewGuid():N}";
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = uniqueName,
            Prompt = "Unique content"
        });

        // Act
        var result = await client.GetPromptListAsync(new PromptListRequest
        {
            Name = uniqueName,
            Page = 1,
            Limit = 10
        });

        // Assert
        result.ShouldNotBeNull();
        result.Data.Length.ShouldBe(1);
        result.Data[0].Name.ShouldBe(uniqueName);
    }
}