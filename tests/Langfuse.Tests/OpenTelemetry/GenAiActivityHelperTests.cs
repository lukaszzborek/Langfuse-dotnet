using System.Diagnostics;
using System.Text.Json;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Models;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class GenAiActivityHelperTests : IDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly ActivityListener _listener;
    private readonly List<Activity> _capturedActivities;

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

    #region CreateChatCompletionActivity Tests

    [Fact]
    public void CreateChatCompletionActivity_SetsRequiredTags()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai"
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("chat", activity.GetTagItem(GenAiAttributes.OperationName));
        Assert.Equal("openai", activity.GetTagItem(GenAiAttributes.ProviderName));
        Assert.Equal("gpt-4", activity.GetTagItem(GenAiAttributes.RequestModel));
        Assert.Equal(LangfuseAttributes.ObservationTypeGeneration,
            activity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalTemperature()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            Temperature = 0.7
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(0.7, activity.GetTagItem(GenAiAttributes.RequestTemperature));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalTopP()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            TopP = 0.9
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(0.9, activity.GetTagItem(GenAiAttributes.RequestTopP));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalTopK()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            TopK = 50
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(50.0, activity.GetTagItem(GenAiAttributes.RequestTopK));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalMaxTokens()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            MaxTokens = 1000
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(1000, activity.GetTagItem(GenAiAttributes.RequestMaxTokens));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalFrequencyPenalty()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            FrequencyPenalty = 0.5
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(0.5, activity.GetTagItem(GenAiAttributes.RequestFrequencyPenalty));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalPresencePenalty()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            PresencePenalty = 0.3
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(0.3, activity.GetTagItem(GenAiAttributes.RequestPresencePenalty));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalChoiceCount()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            ChoiceCount = 3
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(3, activity.GetTagItem(GenAiAttributes.RequestChoiceCount));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalSeed()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            Seed = 12345
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(12345, activity.GetTagItem(GenAiAttributes.RequestSeed));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalStopSequences()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            StopSequences = ["stop1", "stop2"]
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        var stopSequencesJson = activity.GetTagItem(GenAiAttributes.RequestStopSequences) as string;
        Assert.NotNull(stopSequencesJson);
        Assert.Contains("stop1", stopSequencesJson);
        Assert.Contains("stop2", stopSequencesJson);
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalOutputType()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            OutputType = "json"
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("json", activity.GetTagItem(GenAiAttributes.OutputType));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalConversationId()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            ConversationId = "conv-123"
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("conv-123", activity.GetTagItem(GenAiAttributes.ConversationId));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalSystemInstructions()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            SystemInstructions = "You are a helpful assistant."
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("You are a helpful assistant.", activity.GetTagItem(GenAiAttributes.SystemInstructions));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalServerInfo()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            ServerAddress = "api.openai.com",
            ServerPort = 443
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("api.openai.com", activity.GetTagItem(GenAiAttributes.ServerAddress));
        Assert.Equal(443, activity.GetTagItem(GenAiAttributes.ServerPort));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalPromptReference()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            PromptName = "my-prompt",
            PromptVersion = 2
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("my-prompt", activity.GetTagItem(LangfuseAttributes.ObservationPromptName));
        Assert.Equal(2, activity.GetTagItem(LangfuseAttributes.ObservationPromptVersion));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalLevel()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig
        {
            Model = "gpt-4",
            Provider = "openai",
            Level = LangfuseObservationLevel.Warning
        };

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("WARNING", activity.GetTagItem(LangfuseAttributes.ObservationLevel));
    }

    [Fact]
    public void CreateChatCompletionActivity_SetsOptionalMetadata()
    {
        // Arrange
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

        // Act
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test-chat", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("custom_value",
            activity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key"));
        Assert.Equal(42, activity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}number_key"));
    }

    #endregion

    #region CreateTextCompletionActivity Tests

    [Fact]
    public void CreateTextCompletionActivity_SetsRequiredTags()
    {
        // Arrange
        var config = new GenAiTextCompletionConfig
        {
            Model = "text-davinci-003",
            Provider = "openai"
        };

        // Act
        using var activity = GenAiActivityHelper.CreateTextCompletionActivity(_activitySource, "test-text", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("text_completion", activity.GetTagItem(GenAiAttributes.OperationName));
        Assert.Equal("openai", activity.GetTagItem(GenAiAttributes.ProviderName));
        Assert.Equal("text-davinci-003", activity.GetTagItem(GenAiAttributes.RequestModel));
        Assert.Equal(LangfuseAttributes.ObservationTypeGeneration,
            activity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateTextCompletionActivity_SetsOptionalParameters()
    {
        // Arrange
        var config = new GenAiTextCompletionConfig
        {
            Model = "text-davinci-003",
            Provider = "openai",
            Temperature = 0.5,
            MaxTokens = 500
        };

        // Act
        using var activity = GenAiActivityHelper.CreateTextCompletionActivity(_activitySource, "test-text", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(0.5, activity.GetTagItem(GenAiAttributes.RequestTemperature));
        Assert.Equal(500, activity.GetTagItem(GenAiAttributes.RequestMaxTokens));
    }

    #endregion

    #region CreateEmbeddingsActivity Tests

    [Fact]
    public void CreateEmbeddingsActivity_WithSimpleParams_SetsRequiredTags()
    {
        // Act
        using var activity =
            GenAiActivityHelper.CreateEmbeddingsActivity(_activitySource, "test-embed", "openai", "text-embedding-ada");

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("embeddings", activity.GetTagItem(GenAiAttributes.OperationName));
        Assert.Equal("openai", activity.GetTagItem(GenAiAttributes.ProviderName));
        Assert.Equal("text-embedding-ada", activity.GetTagItem(GenAiAttributes.RequestModel));
        Assert.Equal(LangfuseAttributes.ObservationTypeEmbedding,
            activity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateEmbeddingsActivity_WithConfig_SetsAllTags()
    {
        // Arrange
        var config = new GenAiEmbeddingsConfig
        {
            Model = "text-embedding-3-large",
            Provider = "openai",
            Dimensions = 1536,
            EncodingFormats = ["float", "base64"],
            ServerAddress = "api.openai.com",
            ServerPort = 443
        };

        // Act
        using var activity = GenAiActivityHelper.CreateEmbeddingsActivity(_activitySource, "test-embed", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(1536, activity.GetTagItem(GenAiAttributes.EmbeddingsDimensionCount));
        Assert.Equal("api.openai.com", activity.GetTagItem(GenAiAttributes.ServerAddress));
        Assert.Equal(443, activity.GetTagItem(GenAiAttributes.ServerPort));

        var encodingFormatsJson = activity.GetTagItem(GenAiAttributes.RequestEncodingFormats) as string;
        Assert.NotNull(encodingFormatsJson);
        Assert.Contains("float", encodingFormatsJson);
        Assert.Contains("base64", encodingFormatsJson);
    }

    #endregion

    #region CreateToolCallActivity Tests

    [Fact]
    public void CreateToolCallActivity_SetsRequiredTags()
    {
        // Act
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("execute_tool", activity.GetTagItem(GenAiAttributes.OperationName));
        Assert.Equal("get_weather", activity.GetTagItem(GenAiAttributes.ToolName));
        Assert.Equal("function", activity.GetTagItem(GenAiAttributes.ToolType));
        Assert.Equal(LangfuseAttributes.ObservationTypeTool, activity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateToolCallActivity_SetsOptionalParameters()
    {
        // Act
        using var activity = GenAiActivityHelper.CreateToolCallActivity(
            _activitySource,
            "test-tool",
            "get_weather",
            "Gets the current weather for a location",
            "api",
            "call-123");

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("Gets the current weather for a location", activity.GetTagItem(GenAiAttributes.ToolDescription));
        Assert.Equal("api", activity.GetTagItem(GenAiAttributes.ToolType));
        Assert.Equal("call-123", activity.GetTagItem(GenAiAttributes.ToolCallId));
    }

    #endregion

    #region CreateAgentActivity Tests

    [Fact]
    public void CreateAgentActivity_SetsRequiredTags()
    {
        // Arrange
        var config = new GenAiAgentConfig
        {
            Id = "agent-123",
            Name = "TestAgent"
        };

        // Act
        using var activity = GenAiActivityHelper.CreateAgentActivity(_activitySource, "test-agent", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("create_agent", activity.GetTagItem(GenAiAttributes.OperationName));
        Assert.Equal("agent-123", activity.GetTagItem(GenAiAttributes.AgentId));
        Assert.Equal("TestAgent", activity.GetTagItem(GenAiAttributes.AgentName));
        Assert.Equal(LangfuseAttributes.ObservationTypeAgent, activity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateAgentActivity_SetsOptionalDescription()
    {
        // Arrange
        var config = new GenAiAgentConfig
        {
            Id = "agent-123",
            Name = "TestAgent",
            Description = "A helpful assistant agent"
        };

        // Act
        using var activity = GenAiActivityHelper.CreateAgentActivity(_activitySource, "test-agent", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("A helpful assistant agent", activity.GetTagItem(GenAiAttributes.AgentDescription));
    }

    #endregion

    #region CreateInvokeAgentActivity Tests

    [Fact]
    public void CreateInvokeAgentActivity_SetsRequiredTags()
    {
        // Act
        using var activity = GenAiActivityHelper.CreateInvokeAgentActivity(_activitySource, "invoke-agent", "agent-456");

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("invoke_agent", activity.GetTagItem(GenAiAttributes.OperationName));
        Assert.Equal("agent-456", activity.GetTagItem(GenAiAttributes.AgentId));
        Assert.Equal(LangfuseAttributes.ObservationTypeAgent, activity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateInvokeAgentActivity_SetsOptionalAgentName()
    {
        // Act
        using var activity =
            GenAiActivityHelper.CreateInvokeAgentActivity(_activitySource, "invoke-agent", "agent-456", "MyAgent");

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("MyAgent", activity.GetTagItem(GenAiAttributes.AgentName));
    }

    #endregion

    #region CreateTraceActivity Tests

    [Fact]
    public void CreateTraceActivity_SetsTraceName()
    {
        // Arrange
        var config = new TraceConfig { Name = "my-trace" };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("my-trace", activity.GetTagItem(LangfuseAttributes.TraceName));
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalUserAndSessionId()
    {
        // Arrange
        var config = new TraceConfig
        {
            UserId = "user-123",
            SessionId = "session-456"
        };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("user-123", activity.GetTagItem(LangfuseAttributes.UserId));
        Assert.Equal("session-456", activity.GetTagItem(LangfuseAttributes.SessionId));
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalVersionAndRelease()
    {
        // Arrange
        var config = new TraceConfig
        {
            Version = "1.0.0",
            Release = "prod-release-1"
        };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("1.0.0", activity.GetTagItem(LangfuseAttributes.Version));
        Assert.Equal("prod-release-1", activity.GetTagItem(LangfuseAttributes.Release));
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalEnvironment()
    {
        // Arrange
        var config = new TraceConfig { Environment = "production" };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("production", activity.GetTagItem(LangfuseAttributes.Environment));
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalServiceInfo()
    {
        // Arrange
        var config = new TraceConfig
        {
            ServiceName = "my-service",
            ServiceVersion = "2.0.0"
        };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("my-service", activity.GetTagItem(GenAiAttributes.ServiceName));
        Assert.Equal("2.0.0", activity.GetTagItem(GenAiAttributes.ServiceVersion));
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalPublicFlag()
    {
        // Arrange
        var config = new TraceConfig { Public = true };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(true, activity.GetTagItem(LangfuseAttributes.TracePublic));
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalTags()
    {
        // Arrange
        var config = new TraceConfig { Tags = ["tag1", "tag2", "tag3"] };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        // Assert
        Assert.NotNull(activity);
        var tagsJson = activity.GetTagItem(LangfuseAttributes.TraceTags) as string;
        Assert.NotNull(tagsJson);
        Assert.Contains("tag1", tagsJson);
        Assert.Contains("tag2", tagsJson);
        Assert.Contains("tag3", tagsJson);
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalInputOutput()
    {
        // Arrange
        var config = new TraceConfig
        {
            Input = new { query = "test query" },
            Output = new { result = "test result" }
        };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        // Assert
        Assert.NotNull(activity);
        var inputJson = activity.GetTagItem(LangfuseAttributes.TraceInput) as string;
        var outputJson = activity.GetTagItem(LangfuseAttributes.TraceOutput) as string;
        Assert.NotNull(inputJson);
        Assert.NotNull(outputJson);
        Assert.Contains("test query", inputJson);
        Assert.Contains("test result", outputJson);
    }

    [Fact]
    public void CreateTraceActivity_SetsOptionalMetadata()
    {
        // Arrange
        var config = new TraceConfig
        {
            Metadata = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", 123 }
            }
        };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("value1", activity.GetTagItem($"{LangfuseAttributes.TraceMetadataPrefix}key1"));
        Assert.Equal(123, activity.GetTagItem($"{LangfuseAttributes.TraceMetadataPrefix}key2"));
    }

    [Fact]
    public void CreateTraceActivity_WithIsRoot_CreatesNewTraceId()
    {
        // Arrange
        var config = new TraceConfig { Name = "root-trace" };

        // Act
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace-op", config, isRoot: true);

        // Assert
        Assert.NotNull(activity);
        Assert.NotEqual(default, activity.TraceId);
    }

    #endregion

    #region CreateSpanActivity Tests

    [Fact]
    public void CreateSpanActivity_SetsObservationType()
    {
        // Arrange
        var config = new SpanConfig();

        // Act
        using var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, "test-span", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal(LangfuseAttributes.ObservationTypeSpan, activity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    [Fact]
    public void CreateSpanActivity_SetsOptionalSpanType()
    {
        // Arrange
        var config = new SpanConfig { SpanType = "retrieval" };

        // Act
        using var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, "test-span", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("retrieval", activity.GetTagItem("span.type"));
    }

    [Fact]
    public void CreateSpanActivity_SetsOptionalDescription()
    {
        // Arrange
        var config = new SpanConfig { Description = "Test span description" };

        // Act
        using var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, "test-span", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("Test span description", activity.GetTagItem("span.description"));
    }

    [Fact]
    public void CreateSpanActivity_SetsOptionalAttributes()
    {
        // Arrange
        var config = new SpanConfig
        {
            Attributes = new Dictionary<string, object>
            {
                { "custom.attr1", "value1" },
                { "custom.attr2", 42 }
            }
        };

        // Act
        using var activity = GenAiActivityHelper.CreateSpanActivity(_activitySource, "test-span", config);

        // Assert
        Assert.NotNull(activity);
        Assert.Equal("value1", activity.GetTagItem("custom.attr1"));
        Assert.Equal(42, activity.GetTagItem("custom.attr2"));
    }

    #endregion

    #region RecordResponse Tests

    [Fact]
    public void RecordResponse_SetsResponseId()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse { ResponseId = "resp-123" };

        // Act
        GenAiActivityHelper.RecordResponse(activity, response);

        // Assert
        Assert.Equal("resp-123", activity?.GetTagItem(GenAiAttributes.ResponseId));
    }

    [Fact]
    public void RecordResponse_SetsResponseModel()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse { Model = "gpt-4-0613" };

        // Act
        GenAiActivityHelper.RecordResponse(activity, response);

        // Assert
        Assert.Equal("gpt-4-0613", activity?.GetTagItem(GenAiAttributes.ResponseModel));
    }

    [Fact]
    public void RecordResponse_SetsFinishReasons()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse { FinishReasons = ["stop", "length"] };

        // Act
        GenAiActivityHelper.RecordResponse(activity, response);

        // Assert
        var finishReasonsJson = activity?.GetTagItem(GenAiAttributes.ResponseFinishReasons) as string;
        Assert.NotNull(finishReasonsJson);
        Assert.Contains("stop", finishReasonsJson);
        Assert.Contains("length", finishReasonsJson);
    }

    [Fact]
    public void RecordResponse_SetsTokenUsage()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse
        {
            InputTokens = 100,
            OutputTokens = 50
        };

        // Act
        GenAiActivityHelper.RecordResponse(activity, response);

        // Assert
        Assert.Equal(100, activity?.GetTagItem(GenAiAttributes.UsageInputTokens));
        Assert.Equal(50, activity?.GetTagItem(GenAiAttributes.UsageOutputTokens));
    }

    [Fact]
    public void RecordResponse_SetsUsageDetails()
    {
        // Arrange
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

        // Act
        GenAiActivityHelper.RecordResponse(activity, response);

        // Assert
        var usageDetailsJson = activity?.GetTagItem(LangfuseAttributes.ObservationUsageDetails) as string;
        Assert.NotNull(usageDetailsJson);
        Assert.Contains("cached_tokens", usageDetailsJson);
        Assert.Contains("50", usageDetailsJson);
    }

    [Fact]
    public void RecordResponse_SetsCostDetails()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse
        {
            InputCost = 0.01m,
            OutputCost = 0.02m,
            TotalCost = 0.03m
        };

        // Act
        GenAiActivityHelper.RecordResponse(activity, response);

        // Assert
        var costDetailsJson = activity?.GetTagItem(LangfuseAttributes.ObservationCostDetails) as string;
        Assert.NotNull(costDetailsJson);
        Assert.Contains("input", costDetailsJson);
        Assert.Contains("output", costDetailsJson);
        Assert.Contains("total", costDetailsJson);
    }

    [Fact]
    public void RecordResponse_SetsCompletionStartTime()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var startTime = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero);
        var response = new GenAiResponse { CompletionStartTime = startTime };

        // Act
        GenAiActivityHelper.RecordResponse(activity, response);

        // Assert
        var completionStartTime = activity?.GetTagItem(LangfuseAttributes.ObservationCompletionStartTime) as string;
        Assert.NotNull(completionStartTime);
        Assert.Contains("2024-01-15", completionStartTime);
    }

    [Fact]
    public void RecordResponse_WithCompletion_SetsOutputMessage()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var response = new GenAiResponse { Completion = "Hello, world!" };

        // Act
        GenAiActivityHelper.RecordResponse(activity, response);

        // Assert
        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        Assert.NotNull(completionJson);
        Assert.Contains("Hello, world!", completionJson);
        Assert.Contains("assistant", completionJson);
    }

    [Fact]
    public void RecordResponse_WithOutputMessages_SetsOutputMessages()
    {
        // Arrange
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

        // Act
        GenAiActivityHelper.RecordResponse(activity, response);

        // Assert
        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        Assert.NotNull(completionJson);
        Assert.Contains("Response 1", completionJson);
        Assert.Contains("Response 2", completionJson);
    }

    #endregion

    #region RecordError Tests

    [Fact]
    public void RecordError_SetsErrorStatus()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var exception = new InvalidOperationException("Test error message");

        // Act
        GenAiActivityHelper.RecordError(activity!, exception);

        // Assert
        Assert.Equal(ActivityStatusCode.Error, activity?.Status);
        Assert.Equal("Test error message", activity?.StatusDescription);
    }

    [Fact]
    public void RecordError_SetsErrorTags()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var exception = new ArgumentException("Invalid argument");

        // Act
        GenAiActivityHelper.RecordError(activity!, exception);

        // Assert
        Assert.Equal("System.ArgumentException", activity?.GetTagItem(GenAiAttributes.ErrorType));
        Assert.Equal("Invalid argument", activity?.GetTagItem(GenAiAttributes.ErrorMessage));
    }

    [Fact]
    public void RecordError_SetsStackTrace()
    {
        // Arrange
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

        // Act
        GenAiActivityHelper.RecordError(activity!, capturedException!);

        // Assert
        var stackTrace = activity?.GetTagItem(GenAiAttributes.ErrorStack) as string;
        Assert.NotNull(stackTrace);
        Assert.Contains("GenAiActivityHelperTests", stackTrace);
    }

    [Fact]
    public void RecordError_AddsExceptionEvent()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var exception = new Exception("Test exception");

        // Act
        GenAiActivityHelper.RecordError(activity!, exception);

        // Assert
        var exceptionEvent = activity?.Events.FirstOrDefault(e => e.Name == "exception");
        Assert.NotNull(exceptionEvent);
    }

    #endregion

    #region RecordInputMessages/OutputMessages Tests

    [Fact]
    public void RecordInputMessages_SerializesMessagesToJson()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var messages = new List<GenAiMessage>
        {
            new() { Role = "system", Content = "You are helpful." },
            new() { Role = "user", Content = "Hello!" }
        };

        // Act
        GenAiActivityHelper.RecordInputMessages(activity, messages);

        // Assert
        var promptJson = activity?.GetTagItem(GenAiAttributes.Prompt) as string;
        Assert.NotNull(promptJson);
        Assert.Contains("system", promptJson);
        Assert.Contains("You are helpful.", promptJson);
        Assert.Contains("user", promptJson);
        Assert.Contains("Hello!", promptJson);
    }

    [Fact]
    public void RecordInputMessages_WithEmptyList_DoesNotSetTag()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var messages = new List<GenAiMessage>();

        // Act
        GenAiActivityHelper.RecordInputMessages(activity, messages);

        // Assert
        Assert.Null(activity?.GetTagItem(GenAiAttributes.Prompt));
    }

    [Fact]
    public void RecordInputMessage_RecordsSingleMessage()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var message = new GenAiMessage { Role = "user", Content = "Test message" };

        // Act
        GenAiActivityHelper.RecordInputMessage(activity, message);

        // Assert
        var promptJson = activity?.GetTagItem(GenAiAttributes.Prompt) as string;
        Assert.NotNull(promptJson);
        Assert.Contains("Test message", promptJson);
    }

    [Fact]
    public void RecordPrompt_RecordsUserMessage()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.RecordPrompt(activity, "What is the weather?");

        // Assert
        var promptJson = activity?.GetTagItem(GenAiAttributes.Prompt) as string;
        Assert.NotNull(promptJson);
        Assert.Contains("user", promptJson);
        Assert.Contains("What is the weather?", promptJson);
    }

    [Fact]
    public void RecordOutputMessages_SerializesMessagesToJson()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var messages = new List<GenAiMessage>
        {
            new() { Role = "assistant", Content = "Here is my response." }
        };

        // Act
        GenAiActivityHelper.RecordOutputMessages(activity, messages);

        // Assert
        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        Assert.NotNull(completionJson);
        Assert.Contains("assistant", completionJson);
        Assert.Contains("Here is my response.", completionJson);
    }

    [Fact]
    public void RecordCompletion_RecordsAssistantMessage()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.RecordCompletion(activity, "The weather is sunny.");

        // Assert
        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        Assert.NotNull(completionJson);
        Assert.Contains("assistant", completionJson);
        Assert.Contains("The weather is sunny.", completionJson);
    }

    [Fact]
    public void RecordMessages_RecordsBothInputAndOutput()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var inputMessages = new List<GenAiMessage> { new() { Role = "user", Content = "Hi" } };
        var outputMessages = new List<GenAiMessage> { new() { Role = "assistant", Content = "Hello!" } };

        // Act
        GenAiActivityHelper.RecordMessages(activity, inputMessages, outputMessages);

        // Assert
        var promptJson = activity?.GetTagItem(GenAiAttributes.Prompt) as string;
        var completionJson = activity?.GetTagItem(GenAiAttributes.Completion) as string;
        Assert.NotNull(promptJson);
        Assert.NotNull(completionJson);
        Assert.Contains("Hi", promptJson);
        Assert.Contains("Hello!", completionJson);
    }

    #endregion

    #region Tool Call Recording Tests

    [Fact]
    public void RecordToolCallArguments_WithString_SetsArguments()
    {
        // Arrange
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        // Act
        GenAiActivityHelper.RecordToolCallArguments(activity, "{\"location\": \"NYC\"}");

        // Assert
        Assert.Equal("{\"location\": \"NYC\"}", activity?.GetTagItem(GenAiAttributes.ToolCallArguments));
    }

    [Fact]
    public void RecordToolCallArguments_WithObject_SerializesToJson()
    {
        // Arrange
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        // Act
        GenAiActivityHelper.RecordToolCallArguments(activity, new { location = "NYC", unit = "celsius" });

        // Assert
        var argsJson = activity?.GetTagItem(GenAiAttributes.ToolCallArguments) as string;
        Assert.NotNull(argsJson);
        Assert.Contains("NYC", argsJson);
        Assert.Contains("celsius", argsJson);
    }

    [Fact]
    public void RecordToolCallResult_WithString_SetsResult()
    {
        // Arrange
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        // Act
        GenAiActivityHelper.RecordToolCallResult(activity, "72°F, Sunny");

        // Assert
        Assert.Equal("72°F, Sunny", activity?.GetTagItem(GenAiAttributes.ToolCallResult));
    }

    [Fact]
    public void RecordToolCallResult_WithObject_SerializesToJson()
    {
        // Arrange
        using var activity =
            GenAiActivityHelper.CreateToolCallActivity(_activitySource, "test-tool", "get_weather");

        // Act
        GenAiActivityHelper.RecordToolCallResult(activity, new { temperature = 72, condition = "Sunny" });

        // Assert
        var resultJson = activity?.GetTagItem(GenAiAttributes.ToolCallResult) as string;
        Assert.NotNull(resultJson);
        Assert.Contains("72", resultJson);
        Assert.Contains("Sunny", resultJson);
    }

    #endregion

    #region Observation Helpers Tests

    [Fact]
    public void SetObservationInput_WithObject_SerializesToJson()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.SetObservationInput(activity, new { query = "test query" });

        // Assert
        var inputJson = activity?.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("test query", inputJson);
    }

    [Fact]
    public void SetObservationInput_WithString_PreservesString()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.SetObservationInput(activity, "plain text input");

        // Assert
        Assert.Equal("plain text input", activity?.GetTagItem(LangfuseAttributes.ObservationInput));
    }

    [Fact]
    public void SetObservationOutput_WithObject_SerializesToJson()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.SetObservationOutput(activity, new { result = "test result" });

        // Assert
        var outputJson = activity?.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(outputJson);
        Assert.Contains("test result", outputJson);
    }

    [Fact]
    public void SetObservationLevel_SetsLevelTag()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.SetObservationLevel(activity, LangfuseObservationLevel.Error);

        // Assert
        Assert.Equal("ERROR", activity?.GetTagItem(LangfuseAttributes.ObservationLevel));
    }

    [Fact]
    public void SetObservationMetadata_SetsMetadataTag()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.SetObservationMetadata(activity, "custom_key", "custom_value");

        // Assert
        Assert.Equal("custom_value",
            activity?.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key"));
    }

    [Fact]
    public void SetPromptReference_SetsPromptNameAndVersion()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.SetPromptReference(activity, "my-prompt", 3);

        // Assert
        Assert.Equal("my-prompt", activity?.GetTagItem(LangfuseAttributes.ObservationPromptName));
        Assert.Equal(3, activity?.GetTagItem(LangfuseAttributes.ObservationPromptVersion));
    }

    [Fact]
    public void SetPromptReference_WithoutVersion_OnlySetsName()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.SetPromptReference(activity, "my-prompt");

        // Assert
        Assert.Equal("my-prompt", activity?.GetTagItem(LangfuseAttributes.ObservationPromptName));
        Assert.Null(activity?.GetTagItem(LangfuseAttributes.ObservationPromptVersion));
    }

    [Fact]
    public void RecordCompletionStartTime_SetsTimestamp()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var startTime = new DateTimeOffset(2024, 6, 15, 14, 30, 0, TimeSpan.Zero);

        // Act
        GenAiActivityHelper.RecordCompletionStartTime(activity, startTime);

        // Assert
        var timestamp = activity?.GetTagItem(LangfuseAttributes.ObservationCompletionStartTime) as string;
        Assert.NotNull(timestamp);
        Assert.Contains("2024-06-15", timestamp);
    }

    [Fact]
    public void RecordCompletionStartTime_WithoutParam_UsesCurrentTime()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.RecordCompletionStartTime(activity);

        // Assert
        var timestamp = activity?.GetTagItem(LangfuseAttributes.ObservationCompletionStartTime) as string;
        Assert.NotNull(timestamp);
    }

    #endregion

    #region Trace Helpers Tests

    [Fact]
    public void SetTraceInput_SetsTraceAndObservationInput()
    {
        // Arrange
        var config = new TraceConfig { Name = "test-trace" };
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace", config);

        // Act
        GenAiActivityHelper.SetTraceInput(activity, new { query = "test" });

        // Assert
        var traceInput = activity?.GetTagItem(LangfuseAttributes.TraceInput) as string;
        var observationInput = activity?.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(traceInput);
        Assert.NotNull(observationInput);
        Assert.Equal(traceInput, observationInput);
    }

    [Fact]
    public void SetTraceOutput_SetsTraceAndObservationOutput()
    {
        // Arrange
        var config = new TraceConfig { Name = "test-trace" };
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace", config);

        // Act
        GenAiActivityHelper.SetTraceOutput(activity, new { result = "test" });

        // Assert
        var traceOutput = activity?.GetTagItem(LangfuseAttributes.TraceOutput) as string;
        var observationOutput = activity?.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(traceOutput);
        Assert.NotNull(observationOutput);
        Assert.Equal(traceOutput, observationOutput);
    }

    [Fact]
    public void SetTraceTags_SetsTagsAsJson()
    {
        // Arrange
        var config = new TraceConfig { Name = "test-trace" };
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace", config);

        // Act
        GenAiActivityHelper.SetTraceTags(activity, ["tag1", "tag2"]);

        // Assert
        var tagsJson = activity?.GetTagItem(LangfuseAttributes.TraceTags) as string;
        Assert.NotNull(tagsJson);
        Assert.Contains("tag1", tagsJson);
        Assert.Contains("tag2", tagsJson);
    }

    [Fact]
    public void SetTraceTags_WithEmptyList_DoesNotSetTag()
    {
        // Arrange
        var config = new TraceConfig { Name = "test-trace" };
        using var activity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "trace", config);

        // Act
        GenAiActivityHelper.SetTraceTags(activity, []);

        // Assert - trace already has tags from config, so check it's not overwritten with empty
        // We need a fresh activity without tags
        var freshConfig = new TraceConfig { Name = "fresh-trace" };
        using var freshActivity = GenAiActivityHelper.CreateTraceActivity(_activitySource, "fresh", freshConfig);
        GenAiActivityHelper.SetTraceTags(freshActivity, []);

        Assert.Null(freshActivity?.GetTagItem(LangfuseAttributes.TraceTags));
    }

    #endregion

    #region DataSource and Evaluation Tests

    [Fact]
    public void SetDataSource_SetsDataSourceId()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.SetDataSource(activity, "datasource-123");

        // Assert
        Assert.Equal("datasource-123", activity?.GetTagItem(GenAiAttributes.DataSourceId));
    }

    [Fact]
    public void RecordEvaluation_WithObject_SetsEvaluationTags()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);
        var evaluation = new GenAiEvaluation
        {
            Name = "accuracy",
            ScoreValue = 0.95,
            ScoreLabel = "high",
            Explanation = "Very accurate response"
        };

        // Act
        GenAiActivityHelper.RecordEvaluation(activity, evaluation);

        // Assert
        Assert.Equal("accuracy", activity?.GetTagItem(GenAiAttributes.EvaluationName));
        Assert.Equal(0.95, activity?.GetTagItem(GenAiAttributes.EvaluationScoreValue));
        Assert.Equal("high", activity?.GetTagItem(GenAiAttributes.EvaluationScoreLabel));
        Assert.Equal("Very accurate response", activity?.GetTagItem(GenAiAttributes.EvaluationExplanation));
    }

    [Fact]
    public void RecordEvaluation_WithConvenienceOverload_SetsEvaluationTags()
    {
        // Arrange
        var config = new GenAiChatCompletionConfig { Model = "gpt-4", Provider = "openai" };
        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(_activitySource, "test", config);

        // Act
        GenAiActivityHelper.RecordEvaluation(activity, "relevance", 0.8, "medium", "Somewhat relevant");

        // Assert
        Assert.Equal("relevance", activity?.GetTagItem(GenAiAttributes.EvaluationName));
        Assert.Equal(0.8, activity?.GetTagItem(GenAiAttributes.EvaluationScoreValue));
        Assert.Equal("medium", activity?.GetTagItem(GenAiAttributes.EvaluationScoreLabel));
        Assert.Equal("Somewhat relevant", activity?.GetTagItem(GenAiAttributes.EvaluationExplanation));
    }

    #endregion

    #region Null Activity Handling Tests

    [Fact]
    public void RecordResponse_WithNullActivity_DoesNotThrow()
    {
        // Arrange
        var response = new GenAiResponse { ResponseId = "resp-123" };

        // Act & Assert - should not throw
        GenAiActivityHelper.RecordResponse(null, response);
    }

    [Fact]
    public void RecordInputMessages_WithNullActivity_DoesNotThrow()
    {
        // Arrange
        var messages = new List<GenAiMessage> { new() { Role = "user", Content = "Test" } };

        // Act & Assert - should not throw
        GenAiActivityHelper.RecordInputMessages(null, messages);
    }

    [Fact]
    public void SetObservationInput_WithNullActivity_DoesNotThrow()
    {
        // Act & Assert - should not throw
        GenAiActivityHelper.SetObservationInput(null, new { test = "value" });
    }

    [Fact]
    public void SetObservationOutput_WithNullActivity_DoesNotThrow()
    {
        // Act & Assert - should not throw
        GenAiActivityHelper.SetObservationOutput(null, new { test = "value" });
    }

    [Fact]
    public void RecordToolCallArguments_WithNullActivity_DoesNotThrow()
    {
        // Act & Assert - should not throw
        GenAiActivityHelper.RecordToolCallArguments(null, "test");
        GenAiActivityHelper.RecordToolCallArguments(null, new { test = "value" });
    }

    [Fact]
    public void RecordToolCallResult_WithNullActivity_DoesNotThrow()
    {
        // Act & Assert - should not throw
        GenAiActivityHelper.RecordToolCallResult(null, "test");
        GenAiActivityHelper.RecordToolCallResult(null, new { test = "value" });
    }

    #endregion
}
