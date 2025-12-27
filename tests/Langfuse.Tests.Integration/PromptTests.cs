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

        var prompt = await client.CreatePromptAsync(request);

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

        var prompt = await client.CreatePromptAsync(request);

        prompt.ShouldNotBeNull();
        prompt.Name.ShouldBe(promptName);
        prompt.Version.ShouldBe(1);
        prompt.Type.ShouldBe("chat");
        prompt.ShouldBeOfType<ChatPrompt>();
    }

    [Fact]
    public async Task GetPromptAsync_ReturnsByName()
    {
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Test prompt content",
            Labels = ["production"]
        });

        var prompt = await client.GetPromptAsync(promptName);

        prompt.ShouldNotBeNull();
        prompt.Name.ShouldBe(promptName);
        prompt.Version.ShouldBe(1);
    }

    [Fact]
    public async Task GetPromptAsync_ReturnsByVersion()
    {
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";

        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Version 1 content"
        });

        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Version 2 content"
        });

        var promptV1 = await client.GetPromptAsync(promptName, 1);
        var promptV2 = await client.GetPromptAsync(promptName, 2);

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
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";

        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Production content",
            Labels = ["production"]
        });

        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Development content",
            Labels = ["development"]
        });

        var prodPrompt = await client.GetPromptAsync(promptName, label: "production");

        prodPrompt.Version.ShouldBe(1);
        prodPrompt.Labels.ShouldContain("production");
    }

    [Fact]
    public async Task GetPromptListAsync_ReturnsPaginatedList()
    {
        var client = CreateClient();
        var prefix = $"list-prompt-{Guid.NewGuid():N}";

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

        var result = await client.GetPromptListAsync(new PromptListRequest { Page = 1, Limit = 50 });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        (result.Data.Length >= 2).ShouldBeTrue();
    }

    [Fact]
    public async Task UpdatePromptVersionAsync_UpdatesLabels()
    {
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Test content",
            Labels = ["draft"]
        });

        var updated = await client.UpdatePromptVersionAsync(promptName, 1, new UpdatePromptVersionRequest
        {
            NewLabels = ["production", "active"]
        });

        updated.ShouldNotBeNull();
        updated.Labels.ShouldContain("production");
        updated.Labels.ShouldContain("active");
    }

    [Fact]
    public async Task GetPromptAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetPromptAsync("non-existent-prompt"));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task CreatePromptAsync_WithTags_SetsTagsCorrectly()
    {
        var client = CreateClient();
        var promptName = $"test-prompt-{Guid.NewGuid():N}";
        var request = new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = "Tagged prompt",
            Tags = ["tag1", "tag2", "tag3"]
        };

        var prompt = await client.CreatePromptAsync(request);

        prompt.Tags.Count.ShouldBe(3);
        prompt.Tags.ShouldContain("tag1");
        prompt.Tags.ShouldContain("tag2");
        prompt.Tags.ShouldContain("tag3");
    }

    [Fact]
    public async Task GetPromptListAsync_FiltersByName()
    {
        var client = CreateClient();
        var uniqueName = $"unique-prompt-{Guid.NewGuid():N}";
        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = uniqueName,
            Prompt = "Unique content"
        });

        var result = await client.GetPromptListAsync(new PromptListRequest
        {
            Name = uniqueName,
            Page = 1,
            Limit = 10
        });

        result.ShouldNotBeNull();
        result.Data.Length.ShouldBe(1);
        result.Data[0].Name.ShouldBe(uniqueName);
    }

    [Fact]
    public async Task CreatePromptAsync_TextPrompt_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var promptName = $"comprehensive-prompt-{Guid.NewGuid():N}";
        var promptContent = "Hello {{name}}, welcome to {{platform}}! Your query is: {{query}}";
        var config = new { temperature = 0.7, model = "gpt-4", maxTokens = 1000 };

        var request = new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = promptContent,
            Config = config,
            Labels = ["production", "active"],
            Tags = ["integration-test", "comprehensive"]
        };

        var prompt = await client.CreatePromptAsync(request);

        prompt.ShouldBeOfType<TextPrompt>();
        var textPrompt = (TextPrompt)prompt;
        prompt.Name.ShouldBe(promptName);
        prompt.Version.ShouldBe(1);
        prompt.Type.ShouldBe("text");
        textPrompt.PromptText.ShouldBe(promptContent);
        prompt.Config.ShouldNotBeNull();
        prompt.Labels.ShouldNotBeNull();
        prompt.Labels.Count.ShouldBe(2);
        prompt.Labels.ShouldContain("production");
        prompt.Labels.ShouldContain("active");
        prompt.Tags.ShouldNotBeNull();
        prompt.Tags.Count.ShouldBe(2);
        prompt.Tags.ShouldContain("integration-test");
        prompt.Tags.ShouldContain("comprehensive");
    }

    [Fact]
    public async Task CreatePromptAsync_ChatPrompt_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var promptName = $"comprehensive-chat-{Guid.NewGuid():N}";
        var messages = new[]
        {
            new ChatMessage { Role = "system", Content = "You are a helpful assistant specialized in {{domain}}." },
            new ChatMessage { Role = "user", Content = "{{user_query}}" }
        };
        var config = new { temperature = 0.5, model = "gpt-4-turbo" };

        var request = new CreateChatPromptRequest
        {
            Name = promptName,
            Prompt = messages,
            Config = config,
            Labels = ["development"],
            Tags = ["chat", "integration-test"]
        };

        var prompt = await client.CreatePromptAsync(request);

        prompt.ShouldBeOfType<ChatPrompt>();
        var chatPrompt = (ChatPrompt)prompt;
        prompt.Name.ShouldBe(promptName);
        prompt.Version.ShouldBe(1);
        prompt.Type.ShouldBe("chat");
        chatPrompt.PromptMessages.ShouldNotBeNull();
        chatPrompt.PromptMessages.Count.ShouldBe(2);
        prompt.Config.ShouldNotBeNull();
        prompt.Labels.ShouldContain("development");
        prompt.Tags.ShouldContain("chat");
        prompt.Tags.ShouldContain("integration-test");
    }

    [Fact]
    public async Task GetPromptAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var promptName = $"get-comprehensive-{Guid.NewGuid():N}";
        var promptContent = "Test prompt for get validation with {{placeholder}}";
        var config = new { temperature = 0.8 };

        await client.CreatePromptAsync(new CreateTextPromptRequest
        {
            Name = promptName,
            Prompt = promptContent,
            Config = config,
            Labels = ["test"],
            Tags = ["validation"]
        });

        var prompt = await client.GetPromptAsync(promptName);

        prompt.ShouldBeOfType<TextPrompt>();
        var textPrompt = (TextPrompt)prompt;
        prompt.Name.ShouldBe(promptName);
        prompt.Version.ShouldBe(1);
        prompt.Type.ShouldBe("text");
        textPrompt.PromptText.ShouldBe(promptContent);
        prompt.Config.ShouldNotBeNull();
        prompt.Labels.ShouldContain("test");
        prompt.Tags.ShouldContain("validation");
    }
}