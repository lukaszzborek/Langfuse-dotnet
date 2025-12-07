using System.Collections.Concurrent;
using System.Diagnostics;
using OpenTelemetry;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class OtelObservationTests : IDisposable
{
    private readonly ActivityListener _listener;
    private readonly ConcurrentBag<Activity> _capturedActivities;

    public OtelObservationTests()
    {
        _capturedActivities = new ConcurrentBag<Activity>();

        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == "Langfuse",
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStarted = activity => _capturedActivities.Add(activity)
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public void Dispose()
    {
        Baggage.ClearBaggage();
        _listener.Dispose();
    }

    #region OtelObservation Base Class Tests

    [Fact]
    public void SetInput_SetsObservationInputTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        // Act
        span.SetInput(new { data = "test input" });

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        var inputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("test input", inputJson);
    }

    [Fact]
    public void SetOutput_SetsObservationOutputTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        // Act
        span.SetOutput(new { result = "test output" });

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        var outputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(outputJson);
        Assert.Contains("test output", outputJson);
    }

    [Fact]
    public void SetMetadata_SetsObservationMetadataTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        // Act
        span.SetMetadata("custom_key", "custom_value");
        span.SetMetadata("number_key", 42);

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("custom_value",
            spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key"));
        Assert.Equal(42, spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}number_key"));
    }

    [Fact]
    public void SetLevel_SetsObservationLevelTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        // Act
        span.SetLevel(LangfuseObservationLevel.Warning);

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("WARNING", spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel));
    }

    [Fact]
    public void SetLevel_Debug_SetsCorrectValue()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        // Act
        span.SetLevel(LangfuseObservationLevel.Debug);

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("DEBUG", spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel));
    }

    [Fact]
    public void SetLevel_Error_SetsCorrectValue()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        // Act
        span.SetLevel(LangfuseObservationLevel.Error);

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.Equal("ERROR", spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel));
    }

    [Fact]
    public void SetTag_SetsCustomTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        // Act
        span.SetTag("custom.tag.key", "custom_value");

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        var customTag = spanActivity.Tags.FirstOrDefault(t => t.Key == "custom.tag.key");
        Assert.Equal("custom_value", customTag.Value);
    }

    [Fact]
    public void Dispose_StopsActivity()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        var span = trace.CreateSpan("test-span");

        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);
        Assert.False(spanActivity.IsStopped);

        // Act
        span.Dispose();

        // Assert
        Assert.True(spanActivity.IsStopped);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        var span = trace.CreateSpan("test-span");

        // Act & Assert - should not throw
        span.Dispose();
        span.Dispose();
        span.Dispose();
    }

    #endregion

    #region OtelGeneration Tests

    [Fact]
    public void OtelGeneration_SetInputMessages_SetsPromptTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetInputMessages(
        [
            new GenAiMessage { Role = "system", Content = "You are helpful." },
            new GenAiMessage { Role = "user", Content = "Hello!" }
        ]);

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        var promptJson = genActivity.GetTagItem(GenAiAttributes.Prompt) as string;
        Assert.NotNull(promptJson);
        Assert.Contains("system", promptJson);
        Assert.Contains("You are helpful.", promptJson);
        Assert.Contains("user", promptJson);
        Assert.Contains("Hello!", promptJson);
    }

    [Fact]
    public void OtelGeneration_SetPrompt_SetsUserMessage()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetPrompt("What is the weather?");

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        var promptJson = genActivity.GetTagItem(GenAiAttributes.Prompt) as string;
        Assert.NotNull(promptJson);
        Assert.Contains("user", promptJson);
        Assert.Contains("What is the weather?", promptJson);
    }

    [Fact]
    public void OtelGeneration_SetResponse_DoesNotThrow()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act & Assert - should not throw
        var exception = Record.Exception(() =>
            generation.SetResponse(new GenAiResponse
            {
                ResponseId = "resp-123",
                Model = "gpt-4-0613",
                InputTokens = 100,
                OutputTokens = 50,
                FinishReasons = ["stop"]
            }));
        Assert.Null(exception);
    }

    [Fact]
    public void OtelGeneration_SetCompletion_SetsAssistantMessage()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetCompletion("Here is my response.");

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        var completionJson = genActivity.GetTagItem(GenAiAttributes.Completion) as string;
        Assert.NotNull(completionJson);
        Assert.Contains("assistant", completionJson);
        Assert.Contains("Here is my response.", completionJson);
    }

    [Fact]
    public void OtelGeneration_SetPromptReference_SetsPromptTags()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetPromptReference("my-prompt", 3);

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal("my-prompt", genActivity.GetTagItem(LangfuseAttributes.ObservationPromptName));
        Assert.Equal(3, genActivity.GetTagItem(LangfuseAttributes.ObservationPromptVersion));
    }

    [Fact]
    public void OtelGeneration_RecordCompletionStartTime_SetsTimestamp()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");
        var startTime = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero);

        // Act
        generation.RecordCompletionStartTime(startTime);

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        var timestamp = genActivity.GetTagItem(LangfuseAttributes.ObservationCompletionStartTime) as string;
        Assert.NotNull(timestamp);
        Assert.Contains("2024-06-15", timestamp);
    }

    [Fact]
    public void OtelGeneration_SetTemperature_SetsTemperatureTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetTemperature(0.7);

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal(0.7, genActivity.GetTagItem(GenAiAttributes.RequestTemperature));
    }

    [Fact]
    public void OtelGeneration_SetMaxTokens_SetsMaxTokensTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetMaxTokens(1000);

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal(1000, genActivity.GetTagItem(GenAiAttributes.RequestMaxTokens));
    }

    [Fact]
    public void OtelGeneration_SetTopP_SetsTopPTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetTopP(0.9);

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal(0.9, genActivity.GetTagItem(GenAiAttributes.RequestTopP));
    }

    [Fact]
    public void OtelGeneration_SetTopK_SetsTopKTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetTopK(50);

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal(50, genActivity.GetTagItem(GenAiAttributes.RequestTopK));
    }

    [Fact]
    public void OtelGeneration_SetFrequencyPenalty_SetsTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetFrequencyPenalty(0.5);

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal(0.5, genActivity.GetTagItem(GenAiAttributes.RequestFrequencyPenalty));
    }

    [Fact]
    public void OtelGeneration_SetPresencePenalty_SetsTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        // Act
        generation.SetPresencePenalty(0.3);

        // Assert
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-generation");
        Assert.NotNull(genActivity);
        Assert.Equal(0.3, genActivity.GetTagItem(GenAiAttributes.RequestPresencePenalty));
    }

    #endregion

    #region OtelToolCall Tests

    [Fact]
    public void OtelToolCall_SetArguments_DoesNotThrow()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        // Act & Assert - should not throw
        var exception = Record.Exception(() =>
            toolCall.SetArguments(new { location = "NYC" }));
        Assert.Null(exception);
    }

    [Fact]
    public void OtelToolCall_SetResult_DoesNotThrow()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        // Act & Assert - should not throw
        var exception = Record.Exception(() =>
            toolCall.SetResult(new { temperature = 72, condition = "Sunny" }));
        Assert.Null(exception);
    }

    [Fact]
    public void OtelToolCall_InheritsBaseObservationMethods()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        // Act - use base class methods
        toolCall.SetInput(new { query = "NYC weather" });
        toolCall.SetOutput(new { result = "72F" });
        toolCall.SetMetadata("source", "weather_api");
        toolCall.SetLevel(LangfuseObservationLevel.Default);

        // Assert
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-tool-call");
        Assert.NotNull(toolActivity);

        var inputJson = toolActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        var outputJson = toolActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(inputJson);
        Assert.NotNull(outputJson);
        Assert.Contains("NYC weather", inputJson);
        Assert.Contains("72F", outputJson);
        Assert.Equal("weather_api",
            toolActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}source"));
    }

    #endregion

    #region OtelEmbedding Tests

    [Fact]
    public void OtelEmbedding_SetText_SetsObservationInput()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        // Act
        embedding.SetText("Text to embed");

        // Assert
        var embedActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-embedding");
        Assert.NotNull(embedActivity);
        var inputJson = embedActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("Text to embed", inputJson);
    }

    [Fact]
    public void OtelEmbedding_SetResponse_SetsResponseTags()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        // Act
        embedding.SetResponse(new GenAiResponse
        {
            InputTokens = 50,
            Model = "text-embedding-3-large"
        });

        // Assert
        var embedActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-embedding");
        Assert.NotNull(embedActivity);
        Assert.Equal(50, embedActivity.GetTagItem(GenAiAttributes.UsageInputTokens));
        Assert.Equal("text-embedding-3-large", embedActivity.GetTagItem(GenAiAttributes.ResponseModel));
    }

    [Fact]
    public void OtelEmbedding_SetDimensions_SetsDimensionsTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        // Act
        embedding.SetDimensions(1536);

        // Assert
        var embedActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-embedding");
        Assert.NotNull(embedActivity);
        Assert.Equal(1536, embedActivity.GetTagItem(GenAiAttributes.EmbeddingsDimensionCount));
    }

    [Fact]
    public void OtelEmbedding_InheritsBaseObservationMethods()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        // Act & Assert - verify base class methods don't throw
        var exception = Record.Exception(() =>
        {
            embedding.SetOutput(new { embedding_vector = new[] { 0.1, 0.2, 0.3 } });
            embedding.SetMetadata("batch_size", 1);
        });
        Assert.Null(exception);
    }

    #endregion

    #region OtelAgent Tests

    [Fact]
    public void OtelAgent_SetDataSource_SetsDataSourceIdTag()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        // Act
        agent.SetDataSource("datasource-456");

        // Assert
        var agentActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-agent");
        Assert.NotNull(agentActivity);
        Assert.Equal("datasource-456", agentActivity.GetTagItem(GenAiAttributes.DataSourceId));
    }

    [Fact]
    public void OtelAgent_InheritsBaseObservationMethods()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        // Act & Assert - verify base class methods don't throw
        var exception = Record.Exception(() =>
        {
            agent.SetInput(new { task = "summarize document" });
            agent.SetOutput(new { summary = "Document summary..." });
            agent.SetMetadata("agent_type", "summarizer");
            agent.SetLevel(LangfuseObservationLevel.Default);
        });
        Assert.Null(exception);
    }

    #endregion

    #region OtelSpan Tests

    [Fact]
    public void OtelSpan_InheritsBaseObservationMethods()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        // Act
        span.SetInput(new { query = "test" });
        span.SetOutput(new { result = "success" });
        span.SetMetadata("key", "value");
        span.SetLevel(LangfuseObservationLevel.Debug);
        span.SetTag("custom.tag", "custom_value");

        // Assert
        var spanActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-span");
        Assert.NotNull(spanActivity);

        var inputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        var outputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(inputJson);
        Assert.NotNull(outputJson);
        Assert.Equal("value", spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}key"));
        Assert.Equal("DEBUG", spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel));
        var customTag = spanActivity.Tags.FirstOrDefault(t => t.Key == "custom.tag");
        Assert.Equal("custom_value", customTag.Value);
    }

    #endregion

    #region OtelEvent Tests

    [Fact]
    public void OtelEvent_InheritsBaseObservationMethods()
    {
        // Arrange
        using var trace = new OtelLangfuseTrace("test-trace");
        using var otelEvent = trace.CreateEvent("test-event");

        // Act
        otelEvent.SetInput(new { trigger = "user_action" });
        otelEvent.SetOutput(new { logged = true });
        otelEvent.SetMetadata("event_type", "click");

        // Assert
        var eventActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "test-event");
        Assert.NotNull(eventActivity);

        var inputJson = eventActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("user_action", inputJson);
    }

    #endregion

    #region Integration Tests - Full Workflow

    [Fact]
    public void FullWorkflow_GenerationWithInputOutputAndResponse()
    {
        // Arrange & Act
        using var trace = new OtelLangfuseTrace("chat-completion", userId: "user-123");
        trace.SetInput(new { query = "What is AI?" });

        using (var generation = trace.CreateGeneration("gpt-4-call", "gpt-4", provider: "openai"))
        {
            generation.SetInputMessages(
            [
                new GenAiMessage { Role = "system", Content = "You are a helpful assistant." },
                new GenAiMessage { Role = "user", Content = "What is AI?" }
            ]);

            generation.SetTemperature(0.7);
            generation.SetMaxTokens(500);

            // Simulate response
            generation.SetResponse(new GenAiResponse
            {
                ResponseId = "chatcmpl-abc123",
                Model = "gpt-4-0613",
                InputTokens = 25,
                OutputTokens = 150,
                FinishReasons = ["stop"],
                Completion = "AI is artificial intelligence..."
            });
        }

        trace.SetOutput(new { response = "AI is artificial intelligence..." });

        // Assert
        var traceActivity = _capturedActivities.ToList().FirstOrDefault(a =>
            a.GetTagItem(LangfuseAttributes.TraceName)?.ToString() == "chat-completion");
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "gpt-4-call");

        Assert.NotNull(traceActivity);
        Assert.NotNull(genActivity);

        Assert.Equal("user-123", traceActivity.GetTagItem(LangfuseAttributes.UserId));
        Assert.Equal("gpt-4", genActivity.GetTagItem(GenAiAttributes.RequestModel));
        Assert.Equal("openai", genActivity.GetTagItem(GenAiAttributes.ProviderName));
        Assert.Equal(0.7, genActivity.GetTagItem(GenAiAttributes.RequestTemperature));
        Assert.Equal(500, genActivity.GetTagItem(GenAiAttributes.RequestMaxTokens));
        Assert.Equal(25, genActivity.GetTagItem(GenAiAttributes.UsageInputTokens));
        Assert.Equal(150, genActivity.GetTagItem(GenAiAttributes.UsageOutputTokens));
    }

    [Fact]
    public void FullWorkflow_ToolCallWithArgumendtsAndResult()
    {
        // Arrange & Act
        using var trace = new OtelLangfuseTrace("tool-use-trace");

        using (var toolCall = trace.CreateToolCall("weather-lookup", "get_weather",
                   toolDescription: "Get current weather"))
        {
            toolCall.SetArguments(new { location = "San Francisco", unit = "celsius" });
            toolCall.SetResult(new { temperature = 18, condition = "Foggy", humidity = 80 });
            toolCall.SetMetadata("api_version", "v2");
        }

        // Assert
        var toolActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "weather-lookup");
        Assert.NotNull(toolActivity);

        Assert.Equal("get_weather", toolActivity.GetTagItem(GenAiAttributes.ToolName));
        Assert.Equal("Get current weather", toolActivity.GetTagItem(GenAiAttributes.ToolDescription));

        var argsJson = toolActivity.GetTagItem(GenAiAttributes.ToolCallArguments) as string;
        var resultJson = toolActivity.GetTagItem(GenAiAttributes.ToolCallResult) as string;

        Assert.Contains("San Francisco", argsJson);
        Assert.Contains("Foggy", resultJson);
    }

    [Fact]
    public void FullWorkflow_NestedSpansAndEvents()
    {
        // Arrange & Act
        using var trace = new OtelLangfuseTrace("rag-pipeline", userId: "user-456");

        using (var retrievalSpan = trace.CreateSpan("document-retrieval", type: "retrieval"))
        {
            retrievalSpan.SetInput(new { query = "machine learning basics" });

            using (var searchEvent = trace.CreateEvent("vector-search"))
            {
                searchEvent.SetInput(new { embedding_model = "text-embedding-ada" });
                searchEvent.SetOutput(new { documents_found = 5 });
            }

            retrievalSpan.SetOutput(new { documents = new[] { "doc1", "doc2", "doc3" } });
        }

        using (var generation = trace.CreateGeneration("answer-generation", "gpt-4"))
        {
            generation.SetPrompt("Based on the documents, explain machine learning basics.");
            generation.SetCompletion("Machine learning is a subset of AI...");
        }

        // Assert
        var retrievalActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "document-retrieval");
        var searchActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "vector-search");
        var genActivity = _capturedActivities.ToList().FirstOrDefault(a => a.DisplayName == "answer-generation");

        Assert.NotNull(retrievalActivity);
        Assert.NotNull(searchActivity);
        Assert.NotNull(genActivity);

        Assert.Equal("retrieval", retrievalActivity.GetTagItem("span.type"));
        Assert.Equal(LangfuseAttributes.ObservationTypeEvent, searchActivity.GetTagItem(LangfuseAttributes.ObservationType));
        Assert.Equal(LangfuseAttributes.ObservationTypeGeneration, genActivity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    #endregion
}
