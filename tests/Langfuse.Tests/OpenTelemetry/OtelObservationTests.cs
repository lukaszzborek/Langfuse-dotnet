using System.Collections.Concurrent;
using System.Diagnostics;
using OpenTelemetry;
using Shouldly;
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
        spanActivity.ShouldNotBeNull();
        var inputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        inputJson.ShouldNotBeNull();
        inputJson.ShouldContain("test input");
    }

    [Fact]
    public void SetOutput_SetsObservationOutputTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetOutput(new { result = "test output" });

        var spanActivity = span.Activity;
        spanActivity.ShouldNotBeNull();
        var outputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        outputJson.ShouldNotBeNull();
        outputJson.ShouldContain("test output");
    }

    [Fact]
    public void SetMetadata_SetsObservationMetadataTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetMetadata("custom_key", "custom_value");
        span.SetMetadata("number_key", 42);

        var spanActivity = span.Activity;
        spanActivity.ShouldNotBeNull();
        spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}custom_key").ShouldBe("custom_value");
        spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}number_key").ShouldBe(42);
    }

    [Fact]
    public void SetLevel_SetsObservationLevelTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetLevel(LangfuseObservationLevel.Warning);

        var spanActivity = span.Activity;
        spanActivity.ShouldNotBeNull();
        spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel).ShouldBe("WARNING");
    }

    [Fact]
    public void SetLevel_Debug_SetsCorrectValue()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetLevel(LangfuseObservationLevel.Debug);

        var spanActivity = span.Activity;
        spanActivity.ShouldNotBeNull();
        spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel).ShouldBe("DEBUG");
    }

    [Fact]
    public void SetLevel_Error_SetsCorrectValue()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetLevel(LangfuseObservationLevel.Error);

        var spanActivity = span.Activity;
        spanActivity.ShouldNotBeNull();
        spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel).ShouldBe("ERROR");
    }

    [Fact]
    public void SetTag_SetsCustomTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetTag("custom.tag.key", "custom_value");

        var spanActivity = span.Activity;
        spanActivity.ShouldNotBeNull();
        KeyValuePair<string, string?> customTag = spanActivity.Tags.FirstOrDefault(t => t.Key == "custom.tag.key");
        customTag.Value.ShouldBe("custom_value");
    }

    [Fact]
    public void Dispose_StopsActivity()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        var span = trace.CreateSpan("test-span");

        var spanActivity = span.Activity;
        spanActivity.ShouldNotBeNull();
        spanActivity.IsStopped.ShouldBeFalse();

        span.Dispose();

        spanActivity.IsStopped.ShouldBeTrue();
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
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetInputMessages(
        [
            new GenAiMessage { Role = "system", Content = "You are helpful." },
            new GenAiMessage { Role = "user", Content = "Hello!" }
        ]);

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        var promptJson = genActivity.GetTagItem(GenAiAttributes.Prompt) as string;
        promptJson.ShouldNotBeNull();
        promptJson.ShouldContain("system");
        promptJson.ShouldContain("You are helpful.");
        promptJson.ShouldContain("user");
        promptJson.ShouldContain("Hello!");
    }

    [Fact]
    public void OtelGeneration_SetPrompt_SetsUserMessage()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetPrompt("What is the weather?");

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        var promptJson = genActivity.GetTagItem(GenAiAttributes.Prompt) as string;
        promptJson.ShouldNotBeNull();
        promptJson.ShouldContain("user");
        promptJson.ShouldContain("What is the weather?");
    }

    [Fact]
    public void OtelGeneration_SetResponse_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        var exception = Record.Exception(() =>
            generation.SetResponse(new GenAiResponse
            {
                ResponseId = "resp-123",
                Model = "gpt-4-0613",
                InputTokens = 100,
                OutputTokens = 50,
                FinishReasons = ["stop"]
            }));
        exception.ShouldBeNull();
    }

    [Fact]
    public void OtelGeneration_SetCompletion_SetsAssistantMessage()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetCompletion("Here is my response.");

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        var completionJson = genActivity.GetTagItem(GenAiAttributes.Completion) as string;
        completionJson.ShouldNotBeNull();
        completionJson.ShouldContain("assistant");
        completionJson.ShouldContain("Here is my response.");
    }

    [Fact]
    public void OtelGeneration_SetPromptReference_SetsPromptTags()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetPromptReference("my-prompt", 3);

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(LangfuseAttributes.ObservationPromptName).ShouldBe("my-prompt");
        genActivity.GetTagItem(LangfuseAttributes.ObservationPromptVersion).ShouldBe(3);
    }

    [Fact]
    public void OtelGeneration_RecordCompletionStartTime_SetsTimestamp()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");
        var startTime = new DateTimeOffset(2024, 6, 15, 10, 30, 0, TimeSpan.Zero);

        generation.RecordCompletionStartTime(startTime);

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        var timestamp = genActivity.GetTagItem(LangfuseAttributes.ObservationCompletionStartTime) as string;
        timestamp.ShouldNotBeNull();
        timestamp.ShouldContain("2024-06-15");
    }

    [Fact]
    public void OtelGeneration_SetTemperature_SetsTemperatureTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetTemperature(0.7);

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(GenAiAttributes.RequestTemperature).ShouldBe(0.7);
    }

    [Fact]
    public void OtelGeneration_SetMaxTokens_SetsMaxTokensTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetMaxTokens(1000);

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(GenAiAttributes.RequestMaxTokens).ShouldBe(1000);
    }

    [Fact]
    public void OtelGeneration_SetTopP_SetsTopPTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetTopP(0.9);

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(GenAiAttributes.RequestTopP).ShouldBe(0.9);
    }

    [Fact]
    public void OtelGeneration_SetTopK_SetsTopKTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetTopK(50);

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(GenAiAttributes.RequestTopK).ShouldBe(50);
    }

    [Fact]
    public void OtelGeneration_SetFrequencyPenalty_SetsTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetFrequencyPenalty(0.5);

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(GenAiAttributes.RequestFrequencyPenalty).ShouldBe(0.5);
    }

    [Fact]
    public void OtelGeneration_SetPresencePenalty_SetsTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-generation", "gpt-4", "test-provider");

        generation.SetPresencePenalty(0.3);

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        genActivity.GetTagItem(GenAiAttributes.RequestPresencePenalty).ShouldBe(0.3);
    }

    [Fact]
    public void OtelToolCall_SetArguments_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        var exception = Record.Exception(() =>
            toolCall.SetArguments(new { location = "NYC" }));
        exception.ShouldBeNull();
    }

    [Fact]
    public void OtelToolCall_SetResult_DoesNotThrow()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool-call", "get_weather");

        var exception = Record.Exception(() =>
            toolCall.SetResult(new { temperature = 72, condition = "Sunny" }));
        exception.ShouldBeNull();
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
        toolActivity.ShouldNotBeNull();

        var inputJson = toolActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        var outputJson = toolActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        inputJson.ShouldNotBeNull();
        outputJson.ShouldNotBeNull();
        inputJson.ShouldContain("NYC weather");
        outputJson.ShouldContain("72F");
        toolActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}source").ShouldBe("weather_api");
    }

    [Fact]
    public void OtelEmbedding_SetText_SetsObservationInput()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", "test-provider");

        embedding.SetText("Text to embed");

        var embedActivity = embedding.Activity;
        embedActivity.ShouldNotBeNull();
        var inputJson = embedActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        inputJson.ShouldNotBeNull();
        inputJson.ShouldContain("Text to embed");
    }

    [Fact]
    public void OtelEmbedding_SetResponse_SetsResponseTags()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", "test-provider");

        embedding.SetResponse(new GenAiResponse
        {
            InputTokens = 50,
            Model = "text-embedding-3-large"
        });

        var embedActivity = embedding.Activity;
        embedActivity.ShouldNotBeNull();
        embedActivity.GetTagItem(GenAiAttributes.UsageInputTokens).ShouldBe(50);
        embedActivity.GetTagItem(GenAiAttributes.ResponseModel).ShouldBe("text-embedding-3-large");
    }

    [Fact]
    public void OtelEmbedding_SetDimensions_SetsDimensionsTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", "test-provider");

        embedding.SetDimensions(1536);

        var embedActivity = embedding.Activity;
        embedActivity.ShouldNotBeNull();
        embedActivity.GetTagItem(GenAiAttributes.EmbeddingsDimensionCount).ShouldBe(1536);
    }

    [Fact]
    public void OtelEmbedding_InheritsBaseObservationMethods()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embedding", "text-embedding-ada", "test-provider");

        var exception = Record.Exception(() =>
        {
            embedding.SetOutput(new { embedding_vector = new[] { 0.1, 0.2, 0.3 } });
            embedding.SetMetadata("batch_size", 1);
        });
        exception.ShouldBeNull();
    }

    [Fact]
    public void OtelAgent_SetDataSource_SetsDataSourceIdTag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        agent.SetDataSource("datasource-456");

        var agentActivity = agent.Activity;
        agentActivity.ShouldNotBeNull();
        agentActivity.GetTagItem(GenAiAttributes.DataSourceId).ShouldBe("datasource-456");
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
        exception.ShouldBeNull();
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
        spanActivity.ShouldNotBeNull();

        var inputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        var outputJson = spanActivity.GetTagItem(LangfuseAttributes.ObservationOutput) as string;
        inputJson.ShouldNotBeNull();
        outputJson.ShouldNotBeNull();
        spanActivity.GetTagItem($"{LangfuseAttributes.ObservationMetadataPrefix}key").ShouldBe("value");
        spanActivity.GetTagItem(LangfuseAttributes.ObservationLevel).ShouldBe("DEBUG");
        KeyValuePair<string, string?> customTag = spanActivity.Tags.FirstOrDefault(t => t.Key == "custom.tag");
        customTag.Value.ShouldBe("custom_value");
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
        eventActivity.ShouldNotBeNull();

        var inputJson = eventActivity.GetTagItem(LangfuseAttributes.ObservationInput) as string;
        inputJson.ShouldNotBeNull();
        inputJson.ShouldContain("user_action");
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

        traceActivity.ShouldNotBeNull();
        genActivity.ShouldNotBeNull();

        traceActivity.GetTagItem(LangfuseAttributes.UserId).ShouldBe("user-123");
        genActivity.GetTagItem(GenAiAttributes.RequestModel).ShouldBe("gpt-4");
        genActivity.GetTagItem(GenAiAttributes.ProviderName).ShouldBe("openai");
        genActivity.GetTagItem(GenAiAttributes.RequestTemperature).ShouldBe(0.7);
        genActivity.GetTagItem(GenAiAttributes.RequestMaxTokens).ShouldBe(500);
        genActivity.GetTagItem(GenAiAttributes.UsageInputTokens).ShouldBe(25);
        genActivity.GetTagItem(GenAiAttributes.UsageOutputTokens).ShouldBe(150);
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
        toolActivity.ShouldNotBeNull();

        toolActivity.GetTagItem(GenAiAttributes.ToolName).ShouldBe("get_weather");
        toolActivity.GetTagItem(GenAiAttributes.ToolDescription).ShouldBe("Get current weather");

        var argsJson = toolActivity.GetTagItem(GenAiAttributes.ToolCallArguments) as string;
        var resultJson = toolActivity.GetTagItem(GenAiAttributes.ToolCallResult) as string;

        argsJson.ShouldNotBeNull();
        resultJson.ShouldNotBeNull();
        argsJson.ShouldContain("San Francisco");
        resultJson.ShouldContain("Foggy");
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

        using var generation = trace.CreateGeneration("answer-generation", "gpt-4", "test-provider");
        generation.SetPrompt("Based on the documents, explain machine learning basics.");
        generation.SetCompletion("Machine learning is a subset of AI...");

        var retrievalActivity = retrievalSpan.Activity;
        var searchActivity = searchEvent.Activity;
        var genActivity = generation.Activity;

        retrievalActivity.ShouldNotBeNull();
        searchActivity.ShouldNotBeNull();
        genActivity.ShouldNotBeNull();

        retrievalActivity.GetTagItem("span.type").ShouldBe("retrieval");
        searchActivity.GetTagItem(LangfuseAttributes.ObservationType).ShouldBe(LangfuseAttributes.ObservationTypeEvent);
        genActivity.GetTagItem(LangfuseAttributes.ObservationType)
            .ShouldBe(LangfuseAttributes.ObservationTypeGeneration);
    }

    [Fact]
    public void Skip_ClearsRecordedFlag()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        var spanActivity = span.Activity;
        spanActivity.ShouldNotBeNull();
        spanActivity.Recorded.ShouldBeTrue();

        span.Skip();

        spanActivity.Recorded.ShouldBeFalse();
        spanActivity.IsAllDataRequested.ShouldBeFalse();
    }

    [Fact]
    public void Skip_SetsIsSkippedToTrue()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.IsSkipped.ShouldBeFalse();
        span.Skip();
        span.IsSkipped.ShouldBeTrue();
    }

    [Fact]
    public void IsSkipped_FalseByDefault()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.IsSkipped.ShouldBeFalse();
    }

    [Fact]
    public void Skip_WorksOnGeneration()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var generation = trace.CreateGeneration("test-gen", "gpt-4", "test-provider");

        generation.IsSkipped.ShouldBeFalse();
        generation.Skip();
        generation.IsSkipped.ShouldBeTrue();

        var genActivity = generation.Activity;
        genActivity.ShouldNotBeNull();
        genActivity.Recorded.ShouldBeFalse();
    }

    [Fact]
    public void Skip_WorksOnEvent()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var evt = trace.CreateEvent("test-event");

        evt.IsSkipped.ShouldBeFalse();
        evt.Skip();
        evt.IsSkipped.ShouldBeTrue();

        var eventActivity = evt.Activity;
        eventActivity.ShouldNotBeNull();
        eventActivity.Recorded.ShouldBeFalse();
    }

    [Fact]
    public void Skip_WorksOnToolCall()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var toolCall = trace.CreateToolCall("test-tool", "get_weather");

        toolCall.IsSkipped.ShouldBeFalse();
        toolCall.Skip();
        toolCall.IsSkipped.ShouldBeTrue();
    }

    [Fact]
    public void Skip_WorksOnEmbedding()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var embedding = trace.CreateEmbedding("test-embed", "text-embedding-ada", "test-provider");

        embedding.IsSkipped.ShouldBeFalse();
        embedding.Skip();
        embedding.IsSkipped.ShouldBeTrue();
    }

    [Fact]
    public void Skip_WorksOnAgent()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var agent = trace.CreateAgent("test-agent", "agent-123");

        agent.IsSkipped.ShouldBeFalse();
        agent.Skip();
        agent.IsSkipped.ShouldBeTrue();
    }

    [Fact]
    public void Skip_CanBeCalledMultipleTimes()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.Skip();
        span.Skip();
        span.Skip();

        span.IsSkipped.ShouldBeTrue();
    }

    [Fact]
    public void Skip_WithNullActivity_DoesNotThrow()
    {
        // Using NullOtelLangfuseTrace creates observations with null activity
        var trace = NullOtelLangfuseTrace.Instance;
        var span = trace.CreateSpan("test-span");

        var exception = Record.Exception(() => span.Skip());
        exception.ShouldBeNull();
        span.IsSkipped.ShouldBeFalse();
    }

    [Fact]
    public void Skip_AfterSettingOutputStillWorks()
    {
        using var trace = new OtelLangfuseTrace("test-trace");
        using var span = trace.CreateSpan("test-span");

        span.SetOutput(new { result = "done" });
        span.Skip();

        span.IsSkipped.ShouldBeTrue();
    }
}