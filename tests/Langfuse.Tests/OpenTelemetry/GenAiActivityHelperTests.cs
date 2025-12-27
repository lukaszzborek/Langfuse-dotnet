using System.Diagnostics;
using Shouldly;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class GenAiActivityHelperTests : IDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly List<Activity> _capturedActivities;
    private readonly ActivityListener _listener;

    public GenAiActivityHelperTests()
    {
        _activitySource = new ActivitySource("TestSource");
        _capturedActivities = new List<Activity>();

        _listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => _capturedActivities.Add(activity)
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public void Dispose()
    {
        _listener.Dispose();
        _activitySource.Dispose();
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsRequiredTags()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai"
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.OperationName).ShouldBe("chat");
        activity.GetTagItem(GenAiAttributes.ProviderName).ShouldBe("openai");
        activity.GetTagItem(GenAiAttributes.RequestModel).ShouldBe("gpt-4");
        activity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeGeneration);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalTemperature()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            Temperature = 0.7
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.RequestTemperature).ShouldBe(0.7);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalTopP()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            TopP = 0.9
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.RequestTopP).ShouldBe(0.9);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalTopK()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            TopK = 50
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.RequestTopK).ShouldBe(50.0);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalMaxTokens()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            MaxTokens = 1000
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.RequestMaxTokens).ShouldBe(1000);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalFrequencyPenalty()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            FrequencyPenalty = 0.5
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.RequestFrequencyPenalty).ShouldBe(0.5);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalPresencePenalty()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            PresencePenalty = 0.3
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.RequestPresencePenalty).ShouldBe(0.3);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalChoiceCount()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            ChoiceCount = 3
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.RequestChoiceCount).ShouldBe(3);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalSeed()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            Seed = 12345
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.RequestSeed).ShouldBe(12345);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalStopSequences()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            StopSequences = ["stop1", "stop2"]
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        var stopSequencesJson = activity.GetTagItem(GenAiAttributes.RequestStopSequences) as string;
        stopSequencesJson.ShouldNotBeNull();
        stopSequencesJson.ShouldContain("stop1");
        stopSequencesJson.ShouldContain("stop2");
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalOutputType()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            OutputType = "json"
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.OutputType).ShouldBe("json");
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalConversationId()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            ConversationId = "conv-123"
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.ConversationId).ShouldBe("conv-123");
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalSystemInstructions()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            SystemInstructions = "You are a helpful assistant."
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.SystemInstructions).ShouldBe("You are a helpful assistant.");
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalServerInfo()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            ServerAddress = "api.openai.com",
            ServerPort = 443
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.ServerAddress).ShouldBe("api.openai.com");
        activity.GetTagItem(GenAiAttributes.ServerPort).ShouldBe(443);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalPromptReference()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            PromptName = "my-prompt",
            PromptVersion = 2
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(LangfuseAttributes.ObservationPromptName).ShouldBe("my-prompt");
        activity.GetTagItem(LangfuseAttributes.ObservationPromptVersion).ShouldBe(2);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalLevel()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            Level = LangfuseObservationLevel.Warning
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(LangfuseAttributes.ObservationLevel).ShouldBe("WARNING");
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalMetadata()
    {
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            Metadata = new Dictionary<string, object>
            {
                { "custom_key", "custom_value" },
                { "number_key", 42 }
            }
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key").ShouldBe("custom_value");
        activity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}number_key").ShouldBe(42);
    }


    [Fact]
    public void CreateTextCompletionActivity_SetsRequiredTags()
    {
        var config = new GenAiTextCompletionConfig
        {
            Model = "text-davinci-003",
            Provider = "openai"
        };

        using var activity = GenAiActivityHelper.CreateTextCompletionActivity(_activitySource, "test-text", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.OperationName).ShouldBe("text_completion");
        activity.GetTagItem(GenAiAttributes.ProviderName).ShouldBe("openai");
        activity.GetTagItem(GenAiAttributes.RequestModel).ShouldBe("text-davinci-003");
        activity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeGeneration);
    }

    [Fact]
    public void CreateTextCompletionActivity_SetsOptionalParameters()
    {
        var config = new GenAiTextCompletionConfig
        {
            Model = "text-davinci-003",
            Provider = "openai",
            Temperature = 0.5,
            MaxTokens = 500
        };

        using var activity = GenAiActivityHelper.CreateTextCompletionActivity(_activitySource, "test-text", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.RequestTemperature).ShouldBe(0.5);
        activity.GetTagItem(GenAiAttributes.RequestMaxTokens).ShouldBe(500);
    }


    [Fact]
    public void CreateEmbeddingsActivity_WithSimpleParams_SetsRequiredTags()
    {
        using var activity =
            GenAiActivityHelper.CreateEmbeddingsActivity(_activitySource, "test-embed", "openai", "text-embedding-ada");

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.OperationName).ShouldBe("embeddings");
        activity.GetTagItem(GenAiAttributes.ProviderName).ShouldBe("openai");
        activity.GetTagItem(GenAiAttributes.RequestModel).ShouldBe("text-embedding-ada");
        activity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeEmbedding);
    }

    [Fact]
    public void CreateEmbeddingsActivity_WithConfig_SetsAllTags()
    {
        var config = new GenAiEmbeddingsConfig
        {
            Model = "text-embedding-3-large",
            Provider = "openai",
            Dimensions = 1536,
            EncodingFormats = ["float", "base64"],
            ServerAddress = "api.openai.com",
            ServerPort = 443
        };

        using var activity = GenAiActivityHelper.CreateEmbeddingsActivity(_activitySource, "test-embed", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.EmbeddingsDimensionCount).ShouldBe(1536);
        activity.GetTagItem(GenAiAttributes.ServerAddress).ShouldBe("api.openai.com");
        activity.GetTagItem(GenAiAttributes.ServerPort).ShouldBe(443);

        var encodingFormatsJson = activity.GetTagItem(GenAiAttributes.RequestEncodingFormats) as string;
        encodingFormatsJson.ShouldNotBeNull();
        encodingFormatsJson.ShouldContain("float");
        encodingFormatsJson.ShouldContain("base64");
    }


    [Fact]
    public void CreateToolCallActivity_SetsRequiredTags()
    {
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.OperationName).ShouldBe("execute_tool");
        activity.GetTagItem(GenAiAttributes.ToolName).ShouldBe("get_weather");
        activity.GetTagItem(GenAiAttributes.ToolType).ShouldBe("function");
        activity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeTool);
    }

    [Fact]
    public void CreateToolCallActivity_SetsOptionalParameters()
    {
        using var activity = GenAiActivityHelper.CreateToolCallActivity(
            _activitySource,
            "test-tool",
            "get_weather",
            "Gets the current weather for a location",
            "api",
            "call-123");

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.ToolDescription).ShouldBe("Gets the current weather for a location");
        activity.GetTagItem(GenAiAttributes.ToolType).ShouldBe("api");
        activity.GetTagItem(GenAiAttributes.ToolCallId).ShouldBe("call-123");
    }


    [Fact]
    public void CreateAgentActivity_SetsRequiredTags()
    {
        var config = new GenAiAgentConfig
        {
            Id = "agent-123",
            Name = "TestAgent"
        };

        using var activity = GenAiActivityHelper.CreateAgentActivity(_activitySource, "test-agent", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.OperationName).ShouldBe("create_agent");
        activity.GetTagItem(GenAiAttributes.AgentId).ShouldBe("agent-123");
        activity.GetTagItem(GenAiAttributes.AgentName).ShouldBe("TestAgent");
        activity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeAgent);
    }

    [Fact]
    public void CreateAgentActivity_SetsOptionalDescription()
    {
        var config = new GenAiAgentConfig
        {
            Id = "agent-123",
            Name = "TestAgent",
            Description = "A helpful assistant agent"
        };

        using var activity = GenAiActivityHelper.CreateAgentActivity(_activitySource, "test-agent", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.AgentDescription).ShouldBe("A helpful assistant agent");
    }


    [Fact]
    public void CreateInvokeAgentActivity_SetsRequiredTags()
    {
        using var activity =
            GenAiActivityHelper.CreateInvokeAgentActivity(_activitySource, "invoke-agent", "agent-456");

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.OperationName).ShouldBe("invoke_agent");
        activity.GetTagItem(GenAiAttributes.AgentId).ShouldBe("agent-456");
        activity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeAgent);
    }

    [Fact]
    public void CreateInvokeAgentActivity_SetsOptionalAgentName()
    {
        using var activity =
            GenAiActivityHelper.CreateInvokeAgentActivity(_activitySource, "invoke-agent", "agent-456", "MyAgent");

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.AgentName).ShouldBe("MyAgent");
    }


    [Fact]
    public void CreateTraceActivity_SetsTraceName()
    {
        var config = new TraceConfig { Name = "my-trace" };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(LangfuseAttributes.TraceName).ShouldBe("my-trace");
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalUserAndSessionId()
    {
        var config = new TraceConfig
        {
            UserId = "user-123",
            SessionId = "session-456"
        };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(LangfuseAttributes.UserId).ShouldBe("user-123");
        activity.GetTagItem(LangfuseAttributes.SessionId).ShouldBe("session-456");
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalVersionAndRelease()
    {
        var config = new TraceConfig
        {
            Version = "1.0.0",
            Release = "prod-release-1"
        };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(LangfuseAttributes.Version).ShouldBe("1.0.0");
        activity.GetTagItem(LangfuseAttributes.Release).ShouldBe("prod-release-1");
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalEnvironment()
    {
        var config = new TraceConfig { Environment = "production" };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(LangfuseAttributes.Environment).ShouldBe("production");
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalServiceInfo()
    {
        var config = new TraceConfig
        {
            ServiceName = "my-service",
            ServiceVersion = "2.0.0"
        };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(GenAiAttributes.ServiceName).ShouldBe("my-service");
        activity.GetTagItem(GenAiAttributes.ServiceVersion).ShouldBe("2.0.0");
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalPublicFlag()
    {
        var config = new TraceConfig { Public = true };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(LangfuseAttributes.TracePublic).ShouldBe(true);
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalTags()
    {
        var config = new TraceConfig { Tags = ["tag1", "tag2", "tag3"] };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        activity.ShouldNotBeNull();
        var tagsJson = activity.GetTagItem(LangfuseAttributes.TraceTags) as string;
        tagsJson.ShouldNotBeNull();
        tagsJson.ShouldContain("tag1");
        tagsJson.ShouldContain("tag2");
        tagsJson.ShouldContain("tag3");
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalInputOutput()
    {
        var config = new TraceConfig
        {
            Input = new { query = "test query" },
            Output = new { result = "test result" }
        };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        activity.ShouldNotBeNull();
        var inputJson = activity.GetTagItem(LangfuseAttributes.TraceInput) as string;
        var outputJson = activity.GetTagItem(LangfuseAttributes.TraceOutput) as string;
        inputJson.ShouldNotBeNull();
        outputJson.ShouldNotBeNull();
        inputJson.ShouldContain("test query");
        outputJson.ShouldContain("test result");
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalMetadata()
    {
        var config = new TraceConfig
        {
            Metadata = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", 123 }
            }
        };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem($"{LangfuseAttributes.TraceMetadataPrefix}key1").ShouldBe("value1");
        activity.GetTagItem($"{LangfuseAttributes.TraceMetadataPrefix}key2").ShouldBe(123);
    }

    [Fact]
    public void CreateTraceActivity_WithIsRoot_CreatesNewTraceId()
    {
        var config = new TraceConfig { Name = "root-trace" };

        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config, true);

        activity.ShouldNotBeNull();
        activity.TraceId.ShouldNotBe(default);
    }


    [Fact]
    public void CreateSpanActivity_SetsObservationType()
    {
        var config = new SpanConfig();

        using var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, "test-span", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeSpan);
    }

    [Fact]
    public void CreateSpanActivity_SetsOptionalSpanType()
    {
        var config = new SpanConfig { SpanType = "retrieval" };

        using var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, "test-span", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem("span.type").ShouldBe("retrieval");
    }

    [Fact]
    public void CreateSpanActivity_SetsOptionalDescription()
    {
        var config = new SpanConfig { Description = "Test span description" };

        using var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, "test-span", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem("span.description").ShouldBe("Test span description");
    }

    [Fact]
    public void CreateSpanActivity_SetsOptionalAttributes()
    {
        var config = new SpanConfig
        {
            Attributes = new Dictionary<string, object>
            {
                { "custom.attr1", "value1" },
                { "custom.attr2", 42 }
            }
        };

        using var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, "test-span", config);

        activity.ShouldNotBeNull();
        activity.GetTagItem("custom.attr1").ShouldBe("value1");
        activity.GetTagItem("custom.attr2").ShouldBe(42);
    }


    [Fact]
    public void RecordResponse_SetsResponseId()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse { ResponseId = "resp-123" };

        GenAiActivityHelper.RecordResponse(activity, response);

        activity?.GetTagItem(GenAiAttributes.ResponseId).ShouldBe("resp-123");
    }

    [Fact]
    public void RecordResponse_SetsResponseModel()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse { Model = "gpt-4-0613" };

        GenAiActivityHelper.RecordResponse(activity, response);

        activity?.GetTagItem(GenAiAttributes.ResponseModel).ShouldBe("gpt-4-0613");
    }

    [Fact]
    public void RecordResponse_SetsFinishReasons()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse { FinishReasons = ["stop", "length"] };

        GenAiActivityHelper.RecordResponse(activity, response);

        var finishReasonsJson = activity?.GetTagItem(GenAiAttributes.ResponseFinishReasons) as string;
        finishReasonsJson.ShouldNotBeNull();
        finishReasonsJson.ShouldContain("stop");
        finishReasonsJson.ShouldContain("length");
    }

    [Fact]
    public void RecordResponse_SetsTokenUsage()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse
        {
            InputTokens = 100,
            OutputTokens = 50
        };

        GenAiActivityHelper.RecordResponse(activity, response);

        activity?.GetTagItem(GenAiAttributes.UsageInputTokens).ShouldBe(100);
        activity?.GetTagItem(GenAiAttributes.UsageOutputTokens).ShouldBe(50);
    }

    [Fact]
    public void RecordResponse_SetsUsageDetails()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse
        {
            UsageDetails = new Dictionary<string, int>
            {
                { "cached_tokens", 50 },
                { "reasoning_tokens", 20 }
            }
        };

        GenAiActivityHelper.RecordResponse(activity, response);

        var usageDetailsJson = activity?.GetTagItem(LangfuseAttributes.ObservationUsageDetails) as string;
        usageDetailsJson.ShouldNotBeNull();
        usageDetailsJson.ShouldContain("cached_tokens");
        usageDetailsJson.ShouldContain("50");
    }

    [Fact]
    public void RecordResponse_SetsCostDetails()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse
        {
            InputCost = 0.01m,
            OutputCost = 0.02m,
            TotalCost = 0.03m
        };

        GenAiActivityHelper.RecordResponse(activity, response);

        var costDetailsJson = activity?.GetTagItem(LangfuseAttributes.ObservationCostDetails) as string;
        costDetailsJson.ShouldNotBeNull();
        costDetailsJson.ShouldContain("input");
        costDetailsJson.ShouldContain("output");
        costDetailsJson.ShouldContain("total");
    }

    [Fact]
    public void RecordResponse_SetsCompletionStartTime()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var startTime = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero);
        var response = new GenAiResponse { CompletionStartTime = startTime };

        GenAiActivityHelper.RecordResponse(activity, response);

        var completionStartTime = activity?.GetTagItem(LangfuseAttributes.ObservationCompletionStartTime) as string;
        completionStartTime.ShouldNotBeNull();
        completionStartTime.ShouldContain("2024-01-15");
    }

    [Fact]
    public void RecordResponse_WithCompletion_SetsOutputMessage()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse { Completion = "Hello, world!" };

        GenAiActivityHelper.RecordResponse(activity, response);

        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        completionJson.ShouldNotBeNull();
        completionJson.ShouldContain("Hello, world!");
        completionJson.ShouldContain("assistant");
    }

    [Fact]
    public void RecordResponse_WithOutputMessages_SetsOutputMessages()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse
        {
            OutputMessages =
            [
                new GenAiMessage { Role = "assistant", Content = "Response 1" },
                new GenAiMessage { Role = "assistant", Content = "Response 2" }
            ]
        };

        GenAiActivityHelper.RecordResponse(activity, response);

        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        completionJson.ShouldNotBeNull();
        completionJson.ShouldContain("Response 1");
        completionJson.ShouldContain("Response 2");
    }


    [Fact]
    public void RecordError_SetsErrorStatus()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var exception = new InvalidOperationException("Test error message");

        GenAiActivityHelper.RecordError(activity!, exception);

        activity?.Status.ShouldBe(ActivityStatusCode.Error);
        activity?.StatusDescription.ShouldBe("Test error message");
    }

    [Fact]
    public void RecordError_SetsErrorTags()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var exception = new ArgumentException("Invalid argument");

        GenAiActivityHelper.RecordError(activity!, exception);

        activity?.GetTagItem(GenAiAttributes.ErrorType).ShouldBe("System.ArgumentException");
        activity?.GetTagItem(GenAiAttributes.ErrorMessage).ShouldBe("Invalid argument");
    }

    [Fact]
    public void RecordError_SetsStackTrace()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        Exception? capturedException = null;
        try
        {
            throw new Exception("Test exception");
        }
        catch (Exception ex)
        {
            capturedException = ex;
        }

        GenAiActivityHelper.RecordError(activity!, capturedException!);

        var stackTrace = activity?.GetTagItem(GenAiAttributes.ErrorStack) as string;
        stackTrace.ShouldNotBeNull();
        stackTrace.ShouldContain("GenAiActivityHelperTests");
    }

    [Fact]
    public void RecordError_AddsExceptionEvent()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var exception = new Exception("Test exception");

        GenAiActivityHelper.RecordError(activity!, exception);

        ActivityEvent? exceptionEvent = activity?.Events.FirstOrDefault(e => e.Name == "exception");
        exceptionEvent.ShouldNotBeNull();
    }


    [Fact]
    public void RecordInputMessages_SerializesMessagesToJson()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var messages = new List<GenAiMessage>
        {
            new() { Role = "system", Content = "You are helpful." },
            new() { Role = "user", Content = "Hello!" }
        };

        GenAiActivityHelper.RecordInputMessages(activity, messages);

        var promptJson = activity?.GetTagItem(GenAiAttributes.Prompt) as string;
        promptJson.ShouldNotBeNull();
        promptJson.ShouldContain("system");
        promptJson.ShouldContain("You are helpful.");
        promptJson.ShouldContain("user");
        promptJson.ShouldContain("Hello!");
    }

    [Fact]
    public void RecordInputMessages_WithEmptyList_DoesNotSetTag()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var messages = new List<GenAiMessage>();

        GenAiActivityHelper.RecordInputMessages(activity, messages);

        activity?.GetTagItem(GenAiAttributes.Prompt).ShouldBeNull();
    }

    [Fact]
    public void RecordInputMessage_RecordsSingleMessage()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var message = new GenAiMessage { Role = "user", Content = "Test message" };

        GenAiActivityHelper.RecordInputMessage(activity, message);

        var promptJson = activity?.GetTagItem(GenAiAttributes.Prompt) as string;
        promptJson.ShouldNotBeNull();
        promptJson.ShouldContain("Test message");
    }

    [Fact]
    public void RecordPrompt_RecordsUserMessage()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.RecordPrompt(activity, "What is the weather?");

        var promptJson = activity?.GetTagItem(GenAiAttributes.Prompt) as string;
        promptJson.ShouldNotBeNull();
        promptJson.ShouldContain("user");
        promptJson.ShouldContain("What is the weather?");
    }

    [Fact]
    public void RecordOutputMessages_SerializesMessagesToJson()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var messages = new List<GenAiMessage>
        {
            new() { Role = "assistant", Content = "Here is my response." }
        };

        GenAiActivityHelper.RecordOutputMessages(activity, messages);

        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        completionJson.ShouldNotBeNull();
        completionJson.ShouldContain("assistant");
        completionJson.ShouldContain("Here is my response.");
    }

    [Fact]
    public void RecordCompletion_RecordsAssistantMessage()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.RecordCompletion(activity, "The weather is sunny.");

        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        completionJson.ShouldNotBeNull();
        completionJson.ShouldContain("assistant");
        completionJson.ShouldContain("The weather is sunny.");
    }

    [Fact]
    public void RecordMessages_RecordsBothInputAndOutput()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var inputMessages = new List<GenAiMessage> { new() { Role = "user", Content = "Hi" } };
        var outputMessages = new List<GenAiMessage> { new() { Role = "assistant", Content = "Hello!" } };

        GenAiActivityHelper.RecordMessages(activity, inputMessages, outputMessages);

        var promptJson = activity?.GetTagItem(GenAiAttributes.Prompt) as string;
        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        promptJson.ShouldNotBeNull();
        completionJson.ShouldNotBeNull();
        promptJson.ShouldContain("Hi");
        completionJson.ShouldContain("Hello!");
    }

    [Fact]
    public void RecordToolCallArguments_WithString_SetsArguments()
    {
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        GenAiActivityHelper.RecordToolCallArguments(activity, "{\"location\": \"NYC\"}");

        activity?.GetTagItem(GenAiAttributes.ToolCallArguments).ShouldBe("{\"location\": \"NYC\"}");
    }

    [Fact]
    public void RecordToolCallArguments_WithObject_SerializesToJson()
    {
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        GenAiActivityHelper.RecordToolCallArguments(activity, new { location = "NYC", unit = "celsius" });

        var argsJson = activity?.GetTagItem(GenAiAttributes.ToolCallArguments) as string;
        argsJson.ShouldNotBeNull();
        argsJson.ShouldContain("NYC");
        argsJson.ShouldContain("celsius");
    }

    [Fact]
    public void RecordToolCallResult_WithString_SetsResult()
    {
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        GenAiActivityHelper.RecordToolCallResult(activity, "72°F, Sunny");

        activity?.GetTagItem(GenAiAttributes.ToolCallResult).ShouldBe("72°F, Sunny");
    }

    [Fact]
    public void RecordToolCallResult_WithObject_SerializesToJson()
    {
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        GenAiActivityHelper.RecordToolCallResult(activity, new { temperature = 72, condition = "Sunny" });

        var resultJson = activity?.GetTagItem(GenAiAttributes.ToolCallResult) as string;
        resultJson.ShouldNotBeNull();
        resultJson.ShouldContain("72");
        resultJson.ShouldContain("Sunny");
    }

    [Fact]
    public void SetObservationInput_WithObject_SerializesToJson()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.SetObservationInput(activity, new { query = "test query" });

        var inputJson = activity?.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        inputJson.ShouldNotBeNull();
        inputJson.ShouldContain("test query");
    }

    [Fact]
    public void SetObservationInput_WithString_PreservesString()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.SetObservationInput(activity, "plain text input");

        activity?.GetTagItem(LangfuseAttributes.ObservationInput).ShouldBe("plain text input");
    }

    [Fact]
    public void SetObservationOutput_WithObject_SerializesToJson()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.SetObservationOutput(activity, new { result = "test result" });

        var outputJson = activity?.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        outputJson.ShouldNotBeNull();
        outputJson.ShouldContain("test result");
    }

    [Fact]
    public void SetObservationLevel_SetsLevelTag()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.SetObservationLevel(activity, LangfuseObservationLevel.Error);

        activity?.GetTagItem(LangfuseAttributes.ObservationLevel).ShouldBe("ERROR");
    }

    [Fact]
    public void SetObservationMetadata_SetsMetadataTag()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.SetObservationMetadata(activity, "custom_key", "custom_value");

        activity?.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key").ShouldBe("custom_value");
    }

    [Fact]
    public void SetPromptReference_SetsPromptNameAndVersion()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.SetPromptReference(activity, "my-prompt", 3);

        activity?.GetTagItem(LangfuseAttributes.ObservationPromptName).ShouldBe("my-prompt");
        activity?.GetTagItem(LangfuseAttributes.ObservationPromptVersion).ShouldBe(3);
    }

    [Fact]
    public void SetPromptReference_WithoutVersion_OnlySetsName()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.SetPromptReference(activity, "my-prompt");

        activity?.GetTagItem(LangfuseAttributes.ObservationPromptName).ShouldBe("my-prompt");
        activity?.GetTagItem(LangfuseAttributes.ObservationPromptVersion).ShouldBeNull();
    }

    [Fact]
    public void RecordCompletionStartTime_SetsTimestamp()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var startTime = new DateTimeOffset(2024, 6, 15, 14, 30, 0, TimeSpan.Zero);

        GenAiActivityHelper.RecordCompletionStartTime(activity, startTime);

        var timestamp = activity?.GetTagItem(LangfuseAttributes.ObservationCompletionStartTime) as string;
        timestamp.ShouldNotBeNull();
        timestamp.ShouldContain("2024-06-15");
    }

    [Fact]
    public void RecordCompletionStartTime_WithoutParam_UsesCurrentTime()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.RecordCompletionStartTime(activity);

        var timestamp = activity?.GetTagItem(LangfuseAttributes.ObservationCompletionStartTime) as string;
        timestamp.ShouldNotBeNull();
    }


    [Fact]
    public void SetTraceInput_SetsTraceAndObservationInput()
    {
        var config = new TraceConfig { Name = "test-trace" };
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace", config);

        GenAiActivityHelper.SetTraceInput(activity, new { query = "test" });

        var traceInput = activity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        var observationInput = activity?.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        traceInput.ShouldNotBeNull();
        observationInput.ShouldNotBeNull();
        traceInput.ShouldBe(observationInput);
    }

    [Fact]
    public void SetTraceOutput_SetsTraceAndObservationOutput()
    {
        var config = new TraceConfig { Name = "test-trace" };
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace", config);

        GenAiActivityHelper.SetTraceOutput(activity, new { result = "test" });

        var traceOutput = activity?.GetTagItem(LangfuseAttributes.TraceOutput) as string;
        var observationOutput = activity?.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        traceOutput.ShouldNotBeNull();
        observationOutput.ShouldNotBeNull();
        traceOutput.ShouldBe(observationOutput);
    }

    [Fact]
    public void SetTraceTags_SetsTagsAsJson()
    {
        var config = new TraceConfig { Name = "test-trace" };
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace", config);

        GenAiActivityHelper.SetTraceTags(activity, ["tag1", "tag2"]);

        var tagsJson = activity?.GetTagItem(LangfuseAttributes.TraceTags) as string;
        tagsJson.ShouldNotBeNull();
        tagsJson.ShouldContain("tag1");
        tagsJson.ShouldContain("tag2");
    }

    [Fact]
    public void SetTraceTags_WithEmptyList_DoesNotSetTag()
    {
        var config = new TraceConfig { Name = "test-trace" };
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace", config);

        GenAiActivityHelper.SetTraceTags(activity, []);

        var freshConfig = new TraceConfig { Name = "fresh-trace" };
        using var freshActivity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "fresh", freshConfig);
        GenAiActivityHelper.SetTraceTags(freshActivity, []);

        freshActivity?.GetTagItem(LangfuseAttributes.TraceTags).ShouldBeNull();
    }

    [Fact]
    public void SetDataSource_SetsDataSourceId()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.SetDataSource(activity, "datasource-123");

        activity?.GetTagItem(GenAiAttributes.DataSourceId).ShouldBe("datasource-123");
    }

    [Fact]
    public void RecordEvaluation_WithObject_SetsEvaluationTags()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var evaluation = new GenAiEvaluation
        {
            Name = "accuracy",
            ScoreValue = 0.95,
            ScoreLabel = "high",
            Explanation = "Very accurate response"
        };

        GenAiActivityHelper.RecordEvaluation(activity, evaluation);

        activity?.GetTagItem(GenAiAttributes.EvaluationName).ShouldBe("accuracy");
        activity?.GetTagItem(GenAiAttributes.EvaluationScoreValue).ShouldBe(0.95);
        activity?.GetTagItem(GenAiAttributes.EvaluationScoreLabel).ShouldBe("high");
        activity?.GetTagItem(GenAiAttributes.EvaluationExplanation).ShouldBe("Very accurate response");
    }

    [Fact]
    public void RecordEvaluation_WithConvenienceOverload_SetsEvaluationTags()
    {
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        GenAiActivityHelper.RecordEvaluation(activity, "relevance", 0.8, "medium", "Somewhat relevant");

        activity?.GetTagItem(GenAiAttributes.EvaluationName).ShouldBe("relevance");
        activity?.GetTagItem(GenAiAttributes.EvaluationScoreValue).ShouldBe(0.8);
        activity?.GetTagItem(GenAiAttributes.EvaluationScoreLabel).ShouldBe("medium");
        activity?.GetTagItem(GenAiAttributes.EvaluationExplanation).ShouldBe("Somewhat relevant");
    }


    [Fact]
    public void RecordResponse_WithNullActivity_DoesNotThrow()
    {
        var response = new GenAiResponse { ResponseId = "resp-123" };

        GenAiActivityHelper.RecordResponse(null, response);
    }

    [Fact]
    public void RecordInputMessages_WithNullActivity_DoesNotThrow()
    {
        var messages = new List<GenAiMessage> { new() { Role = "user", Content = "Test" } };

        GenAiActivityHelper.RecordInputMessages(null, messages);
    }

    [Fact]
    public void SetObservationInput_WithNullActivity_DoesNotThrow()
    {
        GenAiActivityHelper.SetObservationInput(null, new { test = "value" });
    }

    [Fact]
    public void SetObservationOutput_WithNullActivity_DoesNotThrow()
    {
        GenAiActivityHelper.SetObservationOutput(null, new { test = "value" });
    }

    [Fact]
    public void RecordToolCallArguments_WithNullActivity_DoesNotThrow()
    {
        GenAiActivityHelper.RecordToolCallArguments(null, "test");
        GenAiActivityHelper.RecordToolCallArguments(null, new { test = "value" });
    }

    [Fact]
    public void RecordToolCallResult_WithNullActivity_DoesNotThrow()
    {
        GenAiActivityHelper.RecordToolCallResult(null, "test");
        GenAiActivityHelper.RecordToolCallResult(null, new { test = "value" });
    }
}