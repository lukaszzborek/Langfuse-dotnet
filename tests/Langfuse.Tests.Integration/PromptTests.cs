using Langfuse.Tests.Integration.Fixtures;
using Microsoft.Extensions.DependencyInjection;
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
        Assert.NotNull(prompt);
        Assert.Equal(promptName, prompt.Name);
        Assert.Equal(1, prompt.Version);
        Assert.Equal("text", prompt.Type);
        Assert.IsType<TextPrompt>(prompt);
        var textPrompt = (TextPrompt)prompt;
        Assert.Equal("Hello {{name}}, how can I help you today?", textPrompt.PromptText);
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
        Assert.NotNull(prompt);
        Assert.Equal(promptName, prompt.Name);
        Assert.Equal(1, prompt.Version);
        Assert.Equal("chat", prompt.Type);
        Assert.IsType<ChatPrompt>(prompt);
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
        Assert.NotNull(prompt);
        Assert.Equal(promptName, prompt.Name);
        Assert.Equal(1, prompt.Version);
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
        Assert.Equal(1, promptV1.Version);
        Assert.Equal(2, promptV2.Version);
        Assert.IsType<TextPrompt>(promptV1);
        Assert.IsType<TextPrompt>(promptV2);
        Assert.Equal("Version 1 content", ((TextPrompt)promptV1).PromptText);
        Assert.Equal("Version 2 content", ((TextPrompt)promptV2).PromptText);
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
        Assert.Equal(1, prodPrompt.Version);
        Assert.Contains("production", prodPrompt.Labels);
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
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Length >= 2);
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
        Assert.NotNull(updated);
        Assert.Contains("production", updated.Labels);
        Assert.Contains("active", updated.Labels);
    }

    [Fact]
    public async Task GetPromptAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetPromptAsync("non-existent-prompt"));

        Assert.Equal(404, exception.StatusCode);
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
        Assert.Equal(3, prompt.Tags.Count);
        Assert.Contains("tag1", prompt.Tags);
        Assert.Contains("tag2", prompt.Tags);
        Assert.Contains("tag3", prompt.Tags);
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
        Assert.NotNull(result);
        Assert.Single(result.Data);
        Assert.Equal(uniqueName, result.Data[0].Name);
    }
}