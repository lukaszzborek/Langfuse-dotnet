using System.Collections.Concurrent;
using System.Diagnostics;
using OpenTelemetry;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class OtelObservationTests : IDisposable
{
    private readonly ConcurrentBag<Activity> _capturedActivities;
    private readonly ActivityListener _listener;

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


    [Fact]
    public void SetInput_SetsObservationInputTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetInput(new { data = "test input" });

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);
        var inputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("test input", inputJson);
    }

    [Fact]
    public void SetOutput_SetsObservationOutputTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetOutput(new { result = "test output" });

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);
        var outputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(outputJson);
        Assert.Contains("test output", outputJson);
    }

    [Fact]
    public void SetMetadata_SetsObservationMetadataTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetMetadata("custom_key", "custom_value");
        span.SetMetadata("number_key", 42);

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);
        Assert.Equal("custom_value",
            spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key"));
        Assert.Equal(42, spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}number_key"));
    }

    [Fact]
    public void SetLevel_SetsObservationLevelTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetLevel(LangfuseObservationLevel.Warning);

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);
        Assert.Equal("WARNING", spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel));
    }

    [Fact]
    public void SetLevel_Debug_SetsCorrectValue()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetLevel(LangfuseObservationLevel.Debug);

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);
        Assert.Equal("DEBUG", spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel));
    }

    [Fact]
    public void SetLevel_Error_SetsCorrectValue()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetLevel(LangfuseObservationLevel.Error);

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);
        Assert.Equal("ERROR", spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel));
    }

    [Fact]
    public void SetTag_SetsCustomTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetTag("custom.tag.key", "custom_value");

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);
        KeyValuePair<string, string?> customTag = spanActivity.Tags.FirstOrDefault(t => t.Key == "custom.tag.key");
        Assert.Equal("custom_value", customTag.Value);
    }

    [Fact]
    public void Dispose_StopsActivity()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        var span = trace.CreateSpan("test-span");

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);
        Assert.False(spanActivity.IsStopped);

        span.Dispose();

        Assert.True(spanActivity.IsStopped);
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        var span = trace.CreateSpan("test-span");

        span.Dispose();
        span.Dispose();
        span.Dispose();
    }

    [Fact]
    public void OtelGeneration_SetInputMessages_SetsPromptTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetInputMessages(
        [
            new GenAiMessage { Role = "system", Content = "You are helpful." },
            new GenAiMessage { Role = "user", Content = "Hello!" }
        ]);

        var genActivity = generation.Activity;
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
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetPrompt("What is the weather?");

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        var promptJson = genActivity.GetTagItem(GenAiAttributes.Prompt) as string;
        Assert.NotNull(promptJson);
        Assert.Contains("user", promptJson);
        Assert.Contains("What is the weather?", promptJson);
    }

    [Fact]
    public void OtelGeneration_SetResponse_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

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
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetCompletion("Here is my response.");

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        var completionJson = genActivity.GetTagItem(GenAiAttributes.Completion) as string;
        Assert.NotNull(completionJson);
        Assert.Contains("assistant", completionJson);
        Assert.Contains("Here is my response.", completionJson);
    }

    [Fact]
    public void OtelGeneration_SetPromptReference_SetsPromptTags()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetPromptReference("my-prompt", 3);

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        Assert.Equal("my-prompt", genActivity.GetTagItem(LangfuseAttributes.ObservationPromptName));
        Assert.Equal(3, genActivity.GetTagItem(LangfuseAttributes.ObservationPromptVersion));
    }

    [Fact]
    public void OtelGeneration_RecordCompletionStartTime_SetsTimestamp()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");
        var startTime = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero);

        generation.RecordCompletionStartTime(startTime);

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        var timestamp = genActivity.GetTagItem(LangfuseAttributes.ObservationCompletionStartTime) as string;
        Assert.NotNull(timestamp);
        Assert.Contains("2024-06-15", timestamp);
    }

    [Fact]
    public void OtelGeneration_SetTemperature_SetsTemperatureTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetTemperature(0.7);

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        Assert.Equal(0.7, genActivity.GetTagItem(GenAiAttributes.RequestTemperature));
    }

    [Fact]
    public void OtelGeneration_SetMaxTokens_SetsMaxTokensTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetMaxTokens(1000);

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        Assert.Equal(1000, genActivity.GetTagItem(GenAiAttributes.RequestMaxTokens));
    }

    [Fact]
    public void OtelGeneration_SetTopP_SetsTopPTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetTopP(0.9);

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        Assert.Equal(0.9, genActivity.GetTagItem(GenAiAttributes.RequestTopP));
    }

    [Fact]
    public void OtelGeneration_SetTopK_SetsTopKTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetTopK(50);

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        Assert.Equal(50, genActivity.GetTagItem(GenAiAttributes.RequestTopK));
    }

    [Fact]
    public void OtelGeneration_SetFrequencyPenalty_SetsTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetFrequencyPenalty(0.5);

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        Assert.Equal(0.5, genActivity.GetTagItem(GenAiAttributes.RequestFrequencyPenalty));
    }

    [Fact]
    public void OtelGeneration_SetPresencePenalty_SetsTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4");

        generation.SetPresencePenalty(0.3);

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        Assert.Equal(0.3, genActivity.GetTagItem(GenAiAttributes.RequestPresencePenalty));
    }

    [Fact]
    public void OtelToolCall_SetArguments_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        var exception = Record.Exception(() =>
            toolCall.SetArguments(new { location = "NYC" }));
        Assert.Null(exception);
    }

    [Fact]
    public void OtelToolCall_SetResult_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        var exception = Record.Exception(() =>
            toolCall.SetResult(new { temperature = 72, condition = "Sunny" }));
        Assert.Null(exception);
    }

    [Fact]
    public void OtelToolCall_InheritsBaseObservationMethods()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        toolCall.SetInput(new { query = "NYC weather" });
        toolCall.SetOutput(new { result = "72F" });
        toolCall.SetMetadata("source", "weather_api");
        toolCall.SetLevel(LangfuseObservationLevel.Default);

        var toolActivity = toolCall.Activity;
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

    [Fact]
    public void OtelEmbedding_SetText_SetsObservationInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        embedding.SetText("Text to embed");

        var embedActivity = embedding.Activity;
        Assert.NotNull(embedActivity);
        var inputJson = embedActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("Text to embed", inputJson);
    }

    [Fact]
    public void OtelEmbedding_SetResponse_SetsResponseTags()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        embedding.SetResponse(new GenAiResponse
        {
            InputTokens = 50,
            Model = "text-embedding-3-large"
        });

        var embedActivity = embedding.Activity;
        Assert.NotNull(embedActivity);
        Assert.Equal(50, embedActivity.GetTagItem(GenAiAttributes.UsageInputTokens));
        Assert.Equal("text-embedding-3-large", embedActivity.GetTagItem(GenAiAttributes.ResponseModel));
    }

    [Fact]
    public void OtelEmbedding_SetDimensions_SetsDimensionsTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        embedding.SetDimensions(1536);

        var embedActivity = embedding.Activity;
        Assert.NotNull(embedActivity);
        Assert.Equal(1536, embedActivity.GetTagItem(GenAiAttributes.EmbeddingsDimensionCount));
    }

    [Fact]
    public void OtelEmbedding_InheritsBaseObservationMethods()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada");

        var exception = Record.Exception(() =>
        {
            embedding.SetOutput(new { embedding_vector = new[] { 0.1, 0.2, 0.3 } });
            embedding.SetMetadata("batch_size", 1);
        });
        Assert.Null(exception);
    }

    [Fact]
    public void OtelAgent_SetDataSource_SetsDataSourceIdTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        agent.SetDataSource("datasource-456");

        var agentActivity = agent.Activity;
        Assert.NotNull(agentActivity);
        Assert.Equal("datasource-456", agentActivity.GetTagItem(GenAiAttributes.DataSourceId));
    }

    [Fact]
    public void OtelAgent_InheritsBaseObservationMethods()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        var exception = Record.Exception(() =>
        {
            agent.SetInput(new { task = "summarize document" });
            agent.SetOutput(new { summary = "Document summary..." });
            agent.SetMetadata("agent_type", "summarizer");
            agent.SetLevel(LangfuseObservationLevel.Default);
        });
        Assert.Null(exception);
    }

    [Fact]
    public void OtelSpan_InheritsBaseObservationMethods()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetInput(new { query = "test" });
        span.SetOutput(new { result = "success" });
        span.SetMetadata("key", "value");
        span.SetLevel(LangfuseObservationLevel.Debug);
        span.SetTag("custom.tag", "custom_value");

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);

        var inputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        var outputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        Assert.NotNull(inputJson);
        Assert.NotNull(outputJson);
        Assert.Equal("value", spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}key"));
        Assert.Equal("DEBUG", spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel));
        KeyValuePair<string, string?> customTag = spanActivity.Tags.FirstOrDefault(t => t.Key == "custom.tag");
        Assert.Equal("custom_value", customTag.Value);
    }

    [Fact]
    public void OtelEvent_InheritsBaseObservationMethods()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var otelEvent = trace.CreateEvent("test-event");

        otelEvent.SetInput(new { trigger = "user_action" });
        otelEvent.SetOutput(new { logged = true });
        otelEvent.SetMetadata("event_type", "click");

        var eventActivity = otelEvent.Activity;
        Assert.NotNull(eventActivity);

        var inputJson = eventActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        Assert.NotNull(inputJson);
        Assert.Contains("user_action", inputJson);
    }

    [Fact]
    public void FullWorkflow_GenerationWithInputOutputAndResponse()
    {
        using var trace = new OtelLangfuseTrace("chat-completion", "user-123");
        trace.SetInput(new { query = "What is AI?" });
        using var generation = trace.CreateGeneration("gpt-4-call", "gpt-4", "openai");

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

        trace.SetOutput(new { response = "AI is artificial intelligence..." });

        var traceActivity = trace.TraceActivity;
        var genActivity = generation.Activity;

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
        using var trace = new OtelLangfuseTrace("tool-use-trace");
        using var toolCall = trace.CreateToolCall("weather-lookup", "get_weather",
            "Get current weather");

        toolCall.SetArguments(new { location = "San Francisco", unit = "celsius" });
        toolCall.SetResult(new { temperature = 18, condition = "Foggy", humidity = 80 });
        toolCall.SetMetadata("api_version", "v2");

        var toolActivity = toolCall.Activity;
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
        using var trace = new OtelLangfuseTrace("rag-pipeline", "user-456");
        using var retrievalSpan = trace.CreateSpan("document-retrieval", "retrieval");
        retrievalSpan.SetInput(new { query = "machine learning basics" });

        using var searchEvent = trace.CreateEvent("vector-search");
        searchEvent.SetInput(new { embedding_model = "text-embedding-ada" });
        searchEvent.SetOutput(new { documents_found = 5 });

        retrievalSpan.SetOutput(new { documents = new[] { "doc1", "doc2", "doc3" } });

        using var generation = trace.CreateGeneration("answer-generation", "gpt-4");
        generation.SetPrompt("Based on the documents, explain machine learning basics.");
        generation.SetCompletion("Machine learning is a subset of AI...");

        var retrievalActivity = retrievalSpan.Activity;
        var searchActivity = searchEvent.Activity;
        var genActivity = generation.Activity;

        Assert.NotNull(retrievalActivity);
        Assert.NotNull(searchActivity);
        Assert.NotNull(genActivity);

        Assert.Equal("retrieval", retrievalActivity.GetTagItem("span.type"));
        Assert.Equal(LangfuseAttributes.ObservationTypeEvent,
            searchActivity.GetTagItem(LangfuseAttributes.ObservationType));
        Assert.Equal(LangfuseAttributes.ObservationTypeGeneration,
            genActivity.GetTagItem(LangfuseAttributes.ObservationType));
    }

    #region Skip Functionality Tests

    [Fact]
    public void Skip_ClearsRecordedFlag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        var spanActivity = span.Activity;
        Assert.NotNull(spanActivity);
        Assert.True(spanActivity.Recorded);

        span.Skip();

        Assert.False(spanActivity.Recorded);
        Assert.False(spanActivity.IsAllDataRequested);
    }

    [Fact]
    public void Skip_SetsIsSkippedToTrue()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        Assert.False(span.IsSkipped);
        span.Skip();
        Assert.True(span.IsSkipped);
    }

    [Fact]
    public void IsSkipped_FalseByDefault()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        Assert.False(span.IsSkipped);
    }

    [Fact]
    public void Skip_WorksOnGeneration()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-gen", "gpt-4");

        Assert.False(generation.IsSkipped);
        generation.Skip();
        Assert.True(generation.IsSkipped);

        var genActivity = generation.Activity;
        Assert.NotNull(genActivity);
        Assert.False(genActivity.Recorded);
    }

    [Fact]
    public void Skip_WorksOnEvent()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var evt = trace.CreateEvent("test-event");

        Assert.False(evt.IsSkipped);
        evt.Skip();
        Assert.True(evt.IsSkipped);

        var eventActivity = evt.Activity;
        Assert.NotNull(eventActivity);
        Assert.False(eventActivity.Recorded);
    }

    [Fact]
    public void Skip_WorksOnToolCall()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool", "get_weather");

        Assert.False(toolCall.IsSkipped);
        toolCall.Skip();
        Assert.True(toolCall.IsSkipped);
    }

    [Fact]
    public void Skip_WorksOnEmbedding()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embed", "text-embedding-ada");

        Assert.False(embedding.IsSkipped);
        embedding.Skip();
        Assert.True(embedding.IsSkipped);
    }

    [Fact]
    public void Skip_WorksOnAgent()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        Assert.False(agent.IsSkipped);
        agent.Skip();
        Assert.True(agent.IsSkipped);
    }

    [Fact]
    public void Skip_CanBeCalledMultipleTimes()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.Skip();
        span.Skip();
        span.Skip();

        Assert.True(span.IsSkipped);
    }

    [Fact]
    public void Skip_WithNullActivity_DoesNotThrow()
    {
        // Using NullOtelLangfuseTrace creates observations with null activity
        var trace = NullOtelLangfuseTrace.Instance;
        var span = trace.CreateSpan("test-span");

        var exception = Record.Exception(() => span.Skip());
        Assert.Null(exception);
        Assert.False(span.IsSkipped);
    }

    [Fact]
    public void Skip_AfterSettingOutputStillWorks()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetOutput(new { result = "done" });
        span.Skip();

        Assert.True(span.IsSkipped);
    }

    #endregion
}