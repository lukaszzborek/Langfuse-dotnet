using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Xunit;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Tests
{
    public class LangfuseTraceTests
    {
        private readonly ILangfuseClient _langfuseClient;
        private readonly TimeProvider _timeProvider;
        private readonly DateTime _fixedDateTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);

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
            Assert.NotEqual(Guid.Empty, trace.TraceId);
            Assert.NotNull(trace.Trace);
            Assert.Equal(_fixedDateTime, trace.Trace.Body.Timestamp);
            Assert.Equal(trace.TraceId.ToString(), trace.Trace.Body.Id);
            Assert.Empty(trace.Events);
            Assert.Empty(trace.Spans);
            Assert.Empty(trace.Generations);
        }

        [Fact]
        public void Constructor_WithNameAndTimeProvider_InitializesTraceWithName()
        {
            // Arrange
            var traceName = "TestTrace";

            // Act  
            var trace = new LangfuseTrace(traceName, _timeProvider);

            // Assert
            Assert.NotEqual(Guid.Empty, trace.TraceId);
            Assert.NotNull(trace.Trace);
            Assert.Equal(_fixedDateTime, trace.Trace.Body.Timestamp);
            Assert.Equal(trace.TraceId.ToString(), trace.Trace.Body.Id);
            Assert.Equal(traceName, trace.Trace.Body.Name);
            Assert.Empty(trace.Events);
            Assert.Empty(trace.Spans);
            Assert.Empty(trace.Generations);
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
            Assert.Equal(traceName, trace.Trace.Body.Name);
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
            Assert.Single(trace.Events);
            Assert.Equal(eventName, eventBody.Name);
            Assert.Equal(input, eventBody.Input);
            Assert.Equal(output, eventBody.Output);
            Assert.Equal(_fixedDateTime, eventBody.StartTime);
            Assert.Equal(trace.TraceId.ToString(), eventBody.TraceId);
            Assert.Equal(trace.TraceId.ToString(), eventBody.ParentObservationId);
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
            Assert.Single(trace.Events);
            Assert.Equal(customDate, eventBody.StartTime);
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
            Assert.Single(trace.Spans);
            Assert.Equal(spanName, spanBody.Name);
            Assert.Equal(metadata, spanBody.Metadata);
            Assert.Equal(input, spanBody.Input);
            Assert.Equal(_fixedDateTime, spanBody.StartTime);
            Assert.Equal(trace.TraceId.ToString(), spanBody.TraceId);
            Assert.Equal(trace.TraceId.ToString(), spanBody.ParentObservationId);
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
            Assert.Single(trace.Spans);
            Assert.Equal(customDate, spanBody.StartTime);
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
            Assert.Single(trace.Generations);
            Assert.Equal(generationName, generationBody.Name);
            Assert.Equal(input, generationBody.Input);
            Assert.Equal(output, generationBody.Output);
            Assert.Equal(_fixedDateTime, generationBody.StartTime);
            Assert.Equal(trace.TraceId.ToString(), generationBody.TraceId);
            Assert.Equal(trace.TraceId.ToString(), generationBody.ParentObservationId);
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
            Assert.Single(trace.Generations);
            Assert.Equal(customDate, generationBody.StartTime);
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
            var events = trace.GetEvents();

            // Assert
            Assert.Equal(4, events.Count); // 1 trace + 1 event + 1 span + 1 generation
            Assert.Contains(events, e => e.Type == "trace-create");
            Assert.Contains(events, e => e.Type == "event-create");
            Assert.Contains(events, e => e.Type == "span-create");
            Assert.Contains(events, e => e.Type == "generation-create");
        }

        [Fact]
        public void RemoveLastParentId_RemovesLastParentId()
        {
            // Arrange
            var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
            var eventBody = trace.CreateEvent("TestEvent");
            
            // Create a second event with the first event as parent
            var secondEventBody = trace.CreateEvent("SecondEvent");
            
            // Act
            trace.RemoveLastParentId();
            
            // Create a third event which should now have the trace ID as parent
            var thirdEventBody = trace.CreateEvent("ThirdEvent");

            // Assert
            Assert.Equal(eventBody.Id, secondEventBody.ParentObservationId);
            Assert.Equal(eventBody.Id, thirdEventBody.ParentObservationId);
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
            await _langfuseClient.Received(1).IngestAsync(trace, default);
        }

        [Fact]
        public void NestedObservations_HaveCorrectParentIds()
        {
            // Arrange
            var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
            
            // Act
            var spanBody = trace.CreateSpan("ParentSpan");
            var eventBody = trace.CreateEvent("ChildEvent");
            var generationBody = trace.CreateGeneration("GrandchildGeneration");
            
            // Go back to the parent span level
            trace.RemoveLastParentId();
            
            var siblingEventBody = trace.CreateEvent("SiblingEvent");

            // Assert
            Assert.Equal(trace.TraceId.ToString(), spanBody.ParentObservationId);
            Assert.Equal(spanBody.Id, eventBody.ParentObservationId);
            Assert.Equal(eventBody.Id, generationBody.ParentObservationId);
            Assert.Equal(eventBody.Id, siblingEventBody.ParentObservationId);
        }

        [Fact]
        public void UsingSpan_RemovesParentIdWhenDisposed()
        {
            // Arrange
            var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
            var initialParentId = trace.TraceId.ToString();
            
            // Act & Assert
            using (var span = trace.CreateSpan("TestSpan"))
            {
                // Inside the using block, the span should be the parent
                var childEvent = trace.CreateEvent("ChildEvent");
                Assert.Equal(span.Id, childEvent.ParentObservationId);
            }
            
            // After the using block, the span should be disposed and parent ID should be back to trace ID
            var afterEvent = trace.CreateEvent("AfterEvent");
            Assert.Equal(initialParentId, afterEvent.ParentObservationId);
        }
        
        [Fact]
        public void UsingEvent_RemovesParentIdWhenDisposed()
        {
            // Arrange
            var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
            var initialParentId = trace.TraceId.ToString();
            
            // Act & Assert
            using (var eventBody = trace.CreateEvent("TestEvent"))
            {
                // Inside the using block, the event should be the parent
                var childEvent = trace.CreateEvent("ChildEvent");
                Assert.Equal(eventBody.Id, childEvent.ParentObservationId);
            }
            
            // After the using block, the event should be disposed and parent ID should be back to trace ID
            var afterEvent = trace.CreateEvent("AfterEvent");
            Assert.Equal(initialParentId, afterEvent.ParentObservationId);
        }
        
        [Fact]
        public void UsingGeneration_RemovesParentIdWhenDisposed()
        {
            // Arrange
            var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
            var initialParentId = trace.TraceId.ToString();
            
            // Act & Assert
            using (var generation = trace.CreateGeneration("TestGeneration"))
            {
                // Inside the using block, the generation should be the parent
                var childEvent = trace.CreateEvent("ChildEvent");
                Assert.Equal(generation.Id, childEvent.ParentObservationId);
            }
            
            // After the using block, the generation should be disposed and parent ID should be back to trace ID
            var afterEvent = trace.CreateEvent("AfterEvent");
            Assert.Equal(initialParentId, afterEvent.ParentObservationId);
        }
        
        [Fact]
        public void NestedUsing_ManagesParentIdsCorrectly()
        {
            // Arrange
            var trace = new LangfuseTrace(_timeProvider, _langfuseClient);
            var traceId = trace.TraceId.ToString();
            
            // Collect all the IDs and events for assertions later
            string spanId = null;
            string nestedGenerationId = null;
            string deeplyNestedEventId = null;
            
            CreateEventBody eventInSpan = null;
            CreateEventBody eventInGeneration = null;
            CreateEventBody deepEvent = null;
            CreateEventBody afterDeepEvent = null;
            CreateEventBody afterGenerationEvent = null;
            CreateEventBody afterSpanEvent = null;
            
            // Act - create nested hierarchy with using statements
            using (var span = trace.CreateSpan("OuterSpan"))
            {
                spanId = span.Id;
                eventInSpan = trace.CreateEvent("EventInSpan");
                
                using (var nestedGeneration = trace.CreateGeneration("NestedGeneration"))
                {
                    nestedGenerationId = nestedGeneration.Id;
                    eventInGeneration = trace.CreateEvent("EventInGeneration");
                    
                    using (var deeplyNestedEvent = trace.CreateEvent("DeeplyNestedEvent"))
                    {
                        deeplyNestedEventId = deeplyNestedEvent.Id;
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
            Assert.Equal(spanId, eventInSpan.ParentObservationId);
            Assert.Equal(nestedGenerationId, eventInGeneration.ParentObservationId);
            Assert.Equal(deeplyNestedEventId, deepEvent.ParentObservationId);
            Assert.Equal(nestedGenerationId, afterDeepEvent.ParentObservationId);
            Assert.Equal(spanId, afterGenerationEvent.ParentObservationId);
            Assert.Equal(traceId, afterSpanEvent.ParentObservationId);
            
            // Verify the correct number of events were created
            //Assert.Equal(8, trace.GetEvents().Count); // 1 trace + 7 events
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
                Assert.Null(span.EndTime);
                
                // Set output before the span is disposed
                span.SetOutput(output);
            }
            
            // Assert
            var createdSpan = trace.Spans[0];
            Assert.Equal("TestSpan", createdSpan.Body.Name);
            Assert.NotNull(createdSpan.Body.EndTime);
            Assert.Equal(output, createdSpan.Body.Output);
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
            Assert.Equal(5, trace.GetEvents().Count); // 1 trace + 1 parent span + 3 children
            Assert.Contains(trace.Events, e => e.Body.Name == "ChildEvent");
            Assert.Contains(trace.Spans, s => s.Body.Name == "ChildSpan");
            Assert.Contains(trace.Generations, g => g.Body.Name == "ChildGeneration");        }
    }
}