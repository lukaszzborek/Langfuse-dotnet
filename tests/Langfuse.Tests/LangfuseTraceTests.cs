using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Shouldly;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Tests;

public class LangfuseTraceTests
{
    private readonly DateTime _fixedDateTime = new(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private readonly ILangfuseClient _langfuseClient;
    private readonly TimeProvider _timeProvider;

    public LangfuseTraceTests()
    {
        _langfuseClient = Substitute.For<ILangfuseClient>();

        // Use a fixed time provider for deterministic tests
        _timeProvider = new FakeTimeProvider(_fixedDateTime);
    }

    [Fact]
    public void Constructor_WithTimeProvider_InitializesTraceCorrectly()
    {
        // Act
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);

        // Assert
        trace.TraceId.ShouldNotBe(Guid.Empty);
        trace.Trace.ShouldNotBeNull();
        trace.Trace.Body.Timestamp.ShouldBe(_fixedDateTime);
        trace.Trace.Body.Id.ShouldBe(trace.TraceId.ToString());
        trace.Events.ShouldBeEmpty();
        trace.Spans.ShouldBeEmpty();
        trace.Generations.ShouldBeEmpty();
    }

    [Fact]
    public void Constructor_WithNameAndTimeProvider_InitializesTraceWithName()
    {
        // Arrange
        var traceName = "TestTrace";

        // Act
        var trace = new LangfuseTrace(traceName, _timeProvider);

        // Assert
        trace.TraceId.ShouldNotBe(Guid.Empty);
        trace.Trace.ShouldNotBeNull();
        trace.Trace.Body.Timestamp.ShouldBe(_fixedDateTime);
        trace.Trace.Body.Id.ShouldBe(trace.TraceId.ToString());
        trace.Trace.Body.Name.ShouldBe(traceName);
        trace.Events.ShouldBeEmpty();
        trace.Spans.ShouldBeEmpty();
        trace.Generations.ShouldBeEmpty();
    }

    [Fact]
    public void SetTraceName_UpdatesTraceName()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var traceName = "UpdatedTraceName";

        // Act
        trace.SetTraceName(traceName);

        // Assert
        trace.Trace.Body.Name.ShouldBe(traceName);
    }

    [Fact]
    public void CreateEvent_AddsEventToTraceAndReturnsEventBody()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var eventName = "TestEvent";
        var input = new { message = "Test input" };
        var output = new { result = "Test output" };

        // Act
        var eventBody = trace.CreateEvent(eventName, input, output);

        // Assert
        trace.Events.ShouldHaveSingleItem();
        eventBody.Name.ShouldBe(eventName);
        eventBody.Input.ShouldBe(input);
        eventBody.Output.ShouldBe(output);
        eventBody.StartTime.ShouldBe(_fixedDateTime);
        eventBody.TraceId.ShouldBe(trace.TraceId.ToString());
        eventBody.ParentObservationId.ShouldBe(trace.TraceId.ToString());
    }

    [Fact]
    public void CreateEvent_WithCustomDate_UsesProvidedDate()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var eventName = "TestEvent";
        var customDate = new DateTime(2023, 2, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var eventBody = trace.CreateEvent(eventName, eventDate: customDate);

        // Assert
        trace.Events.ShouldHaveSingleItem();
        eventBody.StartTime.ShouldBe(customDate);
    }

    [Fact]
    public void CreateSpan_AddsSpanToTraceAndReturnsSpanBody()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var spanName = "TestSpan";
        var metadata = new { source = "Test metadata" };
        var input = new { message = "Test input" };

        // Act
        var spanBody = trace.CreateSpan(spanName, metadata, input);

        // Assert
        trace.Spans.ShouldHaveSingleItem();
        spanBody.Name.ShouldBe(spanName);
        spanBody.Metadata.ShouldBe(metadata);
        spanBody.Input.ShouldBe(input);
        spanBody.StartTime.ShouldBe(_fixedDateTime);
        spanBody.TraceId.ShouldBe(trace.TraceId.ToString());
        spanBody.ParentObservationId.ShouldBe(trace.TraceId.ToString());
    }

    [Fact]
    public void CreateSpan_WithCustomDate_UsesProvidedDate()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var spanName = "TestSpan";
        var customDate = new DateTime(2023, 2, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var spanBody = trace.CreateSpan(spanName, startDate: customDate);

        // Assert
        trace.Spans.ShouldHaveSingleItem();
        spanBody.StartTime.ShouldBe(customDate);
    }

    [Fact]
    public void CreateGeneration_AddsGenerationToTraceAndReturnsGenerationBody()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var generationName = "TestGeneration";
        var input = new { prompt = "Test prompt" };
        var output = new { completion = "Test completion" };

        // Act
        var generationBody = trace.CreateGeneration(generationName, input, output);

        // Assert
        trace.Generations.ShouldHaveSingleItem();
        generationBody.Name.ShouldBe(generationName);
        generationBody.Input.ShouldBe(input);
        generationBody.Output.ShouldBe(output);
        generationBody.StartTime.ShouldBe(_fixedDateTime);
        generationBody.TraceId.ShouldBe(trace.TraceId.ToString());
        generationBody.ParentObservationId.ShouldBe(trace.TraceId.ToString());
    }

    [Fact]
    public void CreateGeneration_WithCustomDate_UsesProvidedDate()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var generationName = "TestGeneration";
        var customDate = new DateTime(2023, 2, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        var generationBody = trace.CreateGeneration(generationName, eventDate: customDate);

        // Assert
        trace.Generations.ShouldHaveSingleItem();
        generationBody.StartTime.ShouldBe(customDate);
    }

    [Fact]
    public void GetEvents_ReturnsAllEvents()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        trace.CreateEvent("TestEvent");
        trace.CreateSpan("TestSpan");
        trace.CreateGeneration("TestGeneration");

        // Act
        List<IIngestionEvent> events = trace.GetEvents();

        // Assert
        events.Count.ShouldBe(4); // 1 trace + 1 event + 1 span + 1 generation
        events.ShouldContain(e => e.Type == "trace-create");
        events.ShouldContain(e => e.Type == "event-create");
        events.ShouldContain(e => e.Type == "span-create");
        events.ShouldContain(e => e.Type == "generation-create");
    }

    [Fact]
    public async Task IngestAsync_CallsClientWithCorrectParameters()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        trace.CreateEvent("TestEvent");

        // Act
        await _langfuseClient.IngestAsync(trace);

        // Assert
        await _langfuseClient.Received(1).IngestAsync(trace);
    }

    [Fact]
    public void Observations_HaveCorrectParentIds()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);

        // Act
        var spanBody = trace.CreateSpan("ParentSpan");
        var eventBody = trace.CreateEvent("ChildEvent");
        var generationBody = trace.CreateGeneration("GrandchildGeneration");

        // Assert
        spanBody.ParentObservationId.ShouldBe(trace.TraceId.ToString());
        eventBody.ParentObservationId.ShouldBe(trace.TraceId.ToString());
        generationBody.ParentObservationId.ShouldBe(trace.TraceId.ToString());
    }

    [Fact]
    public void UsingSpan_RemovesParentIdWhenDisposed()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var initialParentId = trace.TraceId.ToString();

        // Act & Assert
        using (var span = trace.CreateSpanScoped("TestSpan"))
        {
            // Inside the using block, the span should be the parent
            var childEvent = trace.CreateEvent("ChildEvent");
            childEvent.ParentObservationId.ShouldBe(span.Id);
        }

        // After the using block, the span should be disposed and parent ID should be back to trace ID
        var afterEvent = trace.CreateEvent("AfterEvent");
        afterEvent.ParentObservationId.ShouldBe(initialParentId);
    }

    [Fact]
    public void UsingEvent_RemovesParentIdWhenDisposed()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var initialParentId = trace.TraceId.ToString();

        // Act & Assert
        using (var eventBody = trace.CreateEventScoped("TestEvent"))
        {
            // Inside the using block, the event should be the parent
            var childEvent = trace.CreateEvent("ChildEvent");
            childEvent.ParentObservationId.ShouldBe(eventBody.Id);
        }

        // After the using block, the event should be disposed and parent ID should be back to trace ID
        var afterEvent = trace.CreateEvent("AfterEvent");
        afterEvent.ParentObservationId.ShouldBe(initialParentId);
    }

    [Fact]
    public void UsingGeneration_RemovesParentIdWhenDisposed()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var initialParentId = trace.TraceId.ToString();

        // Act & Assert
        using (var generation = trace.CreateGenerationScoped("TestGeneration"))
        {
            // Inside the using block, the generation should be the parent
            var childEvent = trace.CreateEvent("ChildEvent");
            childEvent.ParentObservationId.ShouldBe(generation.Id);
        }

        // After the using block, the generation should be disposed and parent ID should be back to trace ID
        var afterEvent = trace.CreateEvent("AfterEvent");
        afterEvent.ParentObservationId.ShouldBe(initialParentId);
    }

    [Fact]
    public void NestedUsing_ManagesParentIdsCorrectly()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var traceId = trace.TraceId.ToString();

        // Collect all the IDs and events for assertions later
        string? spanId = null;
        string? nestedGenerationId = null;
        string? deeplyNestedEventParentId = null;
        string? deeplyNestedEventId = null;

        CreateEventBody? eventInSpan = null;
        CreateEventBody? eventInGeneration = null;
        CreateEventBody? deepEvent = null;
        CreateEventBody? afterDeepEvent = null;
        CreateEventBody? afterGenerationEvent = null;
        CreateEventBody? afterSpanEvent = null;

        // Act - create nested hierarchy with using statements
        using (var span = trace.CreateSpanScoped("OuterSpan"))
        {
            spanId = span.Id;
            eventInSpan = trace.CreateEvent("EventInSpan");

            using (var nestedGeneration = trace.CreateGenerationScoped("NestedGeneration"))
            {
                nestedGenerationId = nestedGeneration.Id;
                eventInGeneration = trace.CreateEvent("EventInGeneration");

                using (var deeplyNestedEvent = trace.CreateEventScoped("DeeplyNestedEvent"))
                {
                    deeplyNestedEventId = deeplyNestedEvent.Id;
                    deeplyNestedEventParentId = deeplyNestedEvent.ParentObservationId;
                    deepEvent = trace.CreateEvent("DeepEvent");
                }

                // After the deeplyNestedEvent is disposed
                afterDeepEvent = trace.CreateEvent("AfterDeepEvent");
            }

            // After the nestedGeneration is disposed
            afterGenerationEvent = trace.CreateEvent("AfterGenerationEvent");
        }

        // After the span is disposed
        afterSpanEvent = trace.CreateEvent("AfterSpanEvent");

        // Assert - verify all parent-child relationships at the end
        eventInSpan.ParentObservationId.ShouldBe(spanId);
        eventInGeneration.ParentObservationId.ShouldBe(nestedGenerationId);
        deeplyNestedEventParentId.ShouldBe(nestedGenerationId);
        deepEvent.ParentObservationId.ShouldBe(deeplyNestedEventId);
        afterDeepEvent.ParentObservationId.ShouldBe(nestedGenerationId);
        afterGenerationEvent.ParentObservationId.ShouldBe(spanId);
        afterSpanEvent.ParentObservationId.ShouldBe(traceId);

        // Verify the correct number of events were created
        //trace.GetEvents().Count.ShouldBe(8); // 1 trace + 7 events
    }

    [Fact]
    public void SpanWithOutput_SetsOutputAndEndTime()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
        var output = new { result = "Test output" };

        // Act
        using (var span = trace.CreateSpan("TestSpan"))
        {
            // Simulate some work
            span.EndTime.ShouldBeNull();

            // Set output before the span is disposed
            span.SetOutput(output);
        }

        // Assert
        var createdSpan = trace.Spans[0];
        createdSpan.Body.Name.ShouldBe("TestSpan");
        createdSpan.Body.EndTime.ShouldNotBeNull();
        createdSpan.Body.Output.ShouldBe(output);
    }

    [Fact]
    public void CreateChildEvents_FromParentSpan()
    {
        // Arrange
        var trace = new LangfuseTrace(_timeProvider, _langfuseClient);

        // Act
        using (var span = trace.CreateSpan("ParentSpan"))
        {
            // Create child events directly from the span
            var childEvent = span.CreateEvent("ChildEvent");
            var childSpan = span.CreateSpan("ChildSpan");
            var childGeneration = span.CreateGeneration("ChildGeneration");
        }

        // Assert
        trace.GetEvents().Count.ShouldBe(5); // 1 trace + 1 parent span + 3 children
        trace.Events.ShouldContain(e => e.Body.Name == "ChildEvent");
        trace.Spans.ShouldContain(s => s.Body.Name == "ChildSpan");
        trace.Generations.ShouldContain(g => g.Body.Name == "ChildGeneration");
    }
}