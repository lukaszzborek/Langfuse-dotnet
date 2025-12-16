using Langfuse.Example.OpenTelemetry.Models;
using Microsoft.AspNetCore.Mvc;
using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace Langfuse.Example.OpenTelemetry.Endpoints;

public static class OtelTraceEndpoints
{
    public static IEndpointRouteBuilder MapOtelTraceExamples(this IEndpointRouteBuilder app)
    {
        app.MapPost("/otel-trace-example", OtelTraceExample);
        app.MapPost("/otel-nested-example", OtelNestedExample);
        app.MapPost("/otel-agent-example", OtelAgentExample);
        app.MapPost("/otel-skip-span-example", OtelSkipSpanExample);
        app.MapPost("/otel-skip-trace-example", OtelSkipTraceExample);

        return app;
    }

    private static async Task<IResult> OtelTraceExample([FromBody] ChatCompletionRequest request)
    {
        using var trace = new OtelLangfuseTrace("customer-support-conversation",
            "user-123",
            "session-456",
            tags: ["support", "billing"],
            input: new { source = "web-chat", priority = "high" });

        // Intent classification generation
        using (var generation = trace.CreateGeneration("intent-classification",
                   request.Model,
                   request.Provider ?? "openai",
                   configure: g =>
                   {
                       g.SetTemperature(0.3);
                       g.SetMaxTokens(100);
                       g.SetInputMessages(new List<GenAiMessage>
                       {
                           new() { Role = "system", Content = "Classify the user intent." },
                           new() { Role = "user", Content = request.Prompt }
                       });
                   }))
        {
            await Task.Delay(200);

            generation.SetResponse(new GenAiResponse
            {
                Model = request.Model,
                InputTokens = 50,
                OutputTokens = 10,
                FinishReasons = ["stop"],
                Completion = "intent: billing_inquiry"
            });
        }

        // Tool call for account lookup
        using (var toolCall = trace.CreateToolCall("lookup-account",
                   "get_account_info",
                   "Retrieves customer account information",
                   input: new { customer_id = "cust-789" }))
        {
            await Task.Delay(100);
            toolCall.SetResult(new { balance = 125.50, status = "active" });
        }

        // Response generation
        using (var responseGen = trace.CreateGeneration("generate-response",
                   request.Model,
                   request.Provider ?? "openai",
                   configure: g =>
                   {
                       g.SetTemperature(0.7);
                       g.SetMaxTokens(500);
                       g.SetInputMessages(new List<GenAiMessage>
                       {
                           new() { Role = "system", Content = "You are a helpful support agent." },
                           new() { Role = "user", Content = request.Prompt },
                           new() { Role = "assistant", Content = "intent: billing_inquiry" },
                           new() { Role = "user", Content = "Account balance: $125.50" }
                       });
                   }))
        {
            await Task.Delay(300);

            var completion = "I can see your account is active with a balance of $125.50. How can I help you today?";
            responseGen.SetResponse(new GenAiResponse
            {
                Model = request.Model,
                InputTokens = 120,
                OutputTokens = 25,
                FinishReasons = ["stop"],
                Completion = completion
            });
        }

        return Results.Ok(new
        {
            success = true,
            traceId = trace.TraceActivity?.TraceId.ToString()
        });
    }

    private static async Task<IResult> OtelNestedExample([FromBody] ChatCompletionRequest request)
    {
        using var trace = new OtelLangfuseTrace("rag-pipeline",
            "user-456",
            tags: ["rag", "knowledge-base"],
            input: new { query = request.Prompt });

        // Document retrieval span
        using (var retrievalSpan = trace.CreateSpan("document-retrieval",
                   "retrieval",
                   "Retrieve relevant documents from vector store",
                   request.Prompt))
        {
            // Nested embedding
            using (var embedding = trace.CreateEmbedding("query-embedding",
                       "text-embedding-3-small",
                       "openai",
                       request.Prompt,
                       e => e.SetDimensions(1536)))
            {
                await Task.Delay(50);
                embedding.SetResponse(new GenAiResponse { InputTokens = 15 });
            }

            await Task.Delay(150);
            retrievalSpan.SetOutput(new { documents = new[] { "doc1.pdf", "doc2.pdf" }, count = 2 });
        }

        // Event for documents ranked
        using var logEvent = trace.CreateEvent("documents-ranked",
            new { query = request.Prompt },
            new { topDocId = "doc1.pdf", score = 0.95 });

        // Answer generation
        using (var generation = trace.CreateGeneration("answer-generation",
                   request.Model,
                   request.Provider ?? "openai",
                   configure: g =>
                   {
                       g.SetTemperature(0.5);
                       g.SetInputMessages(new List<GenAiMessage>
                       {
                           new() { Role = "system", Content = "Answer based on the retrieved documents." },
                           new() { Role = "user", Content = request.Prompt }
                       });
                   }))
        {
            await Task.Delay(400);

            var answer = "Based on the documentation, here is your answer...";
            generation.SetResponse(new GenAiResponse
            {
                Model = request.Model,
                InputTokens = 85,
                OutputTokens = 45,
                FinishReasons = ["stop"],
                Completion = answer
            });

            trace.SetOutput(new { answer, sources = new[] { "doc1.pdf" } });
        }

        return Results.Ok(new
        {
            success = true,
            traceId = trace.TraceActivity?.TraceId.ToString()
        });
    }

    private static async Task<IResult> OtelAgentExample([FromBody] ChatCompletionRequest request)
    {
        using var trace = new OtelLangfuseTrace("research-agent",
            "user-789",
            tags: ["agent", "research"]);

        // Agent observation
        using (var agent = trace.CreateAgent("research-assistant",
                   "agent-001",
                   "An agent that researches topics and provides summaries",
                   request.Prompt))
        {
            // Multiple search tool calls
            for (var i = 1; i <= 3; i++)
            {
                using (var toolCall = trace.CreateToolCall($"search-step-{i}",
                           "web_search",
                           "Search the web for information",
                           input: new { query = $"{request.Prompt} - aspect {i}" }))
                {
                    await Task.Delay(100);
                    toolCall.SetResult(new { results = new[] { $"Result {i}a", $"Result {i}b" } });
                }
            }

            // Synthesis generation
            using (var generation = trace.CreateGeneration("synthesize-results",
                       request.Model,
                       request.Provider ?? "openai",
                       configure: g =>
                       {
                           g.SetTemperature(0.7);
                           g.SetPrompt($"Synthesize research findings about: {request.Prompt}");
                       }))
            {
                await Task.Delay(300);

                var synthesis = "Based on my research, here are the key findings...";
                generation.SetResponse(new GenAiResponse
                {
                    Model = request.Model,
                    InputTokens = 150,
                    OutputTokens = 80,
                    FinishReasons = ["stop"],
                    Completion = synthesis
                });
                agent.SetOutput(synthesis);
            }
        }

        return Results.Ok(new
        {
            success = true,
            traceId = trace.TraceActivity?.TraceId.ToString()
        });
    }

    /// <summary>
    /// Example demonstrating how to skip individual spans/observations.
    /// Useful when a task turns out to be unnecessary (e.g., cached result, record already exists).
    /// The skipped span won't be sent to Langfuse, but the trace and other observations will be.
    /// </summary>
    private static async Task<IResult> OtelSkipSpanExample([FromBody] ChatCompletionRequest request)
    {
        // Simulate a cache lookup
        var cachedResult = request.Prompt.Contains("cached") ? "This is a cached response" : null;

        using var trace = new OtelLangfuseTrace("processing-with-cache",
            "user-123",
            tags: ["cache-example"],
            input: new { query = request.Prompt });

        // Span for cache check - always runs
        using (var cacheCheckSpan = trace.CreateSpan("cache-check",
                   "cache",
                   "Check if result is cached"))
        {
            await Task.Delay(10);
            cacheCheckSpan.SetOutput(new { cacheHit = cachedResult != null });
        }

        // Span for LLM processing - will be skipped if we have a cached result
        using (var processingSpan = trace.CreateSpan("llm-processing",
                   "llm",
                   "Process with LLM if not cached"))
        {
            if (cachedResult != null)
            {
                // Skip this span - it won't be sent to Langfuse
                processingSpan.Skip();

                trace.SetOutput(new { result = cachedResult, source = "cache" });

                return Results.Ok(new
                {
                    success = true,
                    result = cachedResult,
                    source = "cache",
                    traceId = trace.TraceActivity?.TraceId.ToString(),
                    message = "LLM processing span was skipped (not sent to Langfuse)"
                });
            }

            // If not cached, proceed with LLM call
            using (var generation = trace.CreateGeneration("generate-response",
                       request.Model,
                       request.Provider ?? "openai",
                       configure: g =>
                       {
                           g.SetTemperature(0.7);
                           g.SetPrompt(request.Prompt);
                       }))
            {
                await Task.Delay(200);

                var response = $"Generated response for: {request.Prompt}";
                generation.SetResponse(new GenAiResponse
                {
                    Model = request.Model,
                    InputTokens = 20,
                    OutputTokens = 15,
                    FinishReasons = ["stop"],
                    Completion = response
                });

                processingSpan.SetOutput(new { response });
                trace.SetOutput(new { result = response, source = "llm" });

                return Results.Ok(new
                {
                    success = true,
                    result = response,
                    source = "llm",
                    traceId = trace.TraceActivity?.TraceId.ToString(),
                    message = "LLM processing span was recorded"
                });
            }
        }
    }

    /// <summary>
    /// Example demonstrating how to skip an entire trace.
    /// Useful when the entire operation should be discarded (e.g., duplicate request, invalid input).
    /// Nothing will be sent to Langfuse when the trace is skipped.
    /// </summary>
    private static async Task<IResult> OtelSkipTraceExample([FromBody] ChatCompletionRequest request)
    {
        // Simulate a duplicate request check
        var isDuplicateRequest = request.Prompt.Contains("duplicate");

        using var trace = new OtelLangfuseTrace("request-processing",
            "user-456",
            tags: ["skip-trace-example"],
            input: new { query = request.Prompt });

        using (var t = trace.CreateEmbedding("query-embedding", "text-embedding-3-small", "openai", request.Prompt))
        {
            await Task.Delay(10);
            t.SetResponse(new GenAiResponse { InputTokens = 15 });
        }

        // Initial validation span
        using (var validationSpan = trace.CreateSpan("validate-request",
                   "validation",
                   "Validate the incoming request"))
        {
            await Task.Delay(10);

            if (isDuplicateRequest)
            {
                validationSpan.SetOutput(new { valid = false, reason = "duplicate_request" });

                // Skip the entire trace - nothing will be sent to Langfuse
                trace.SkipTrace();

                return Results.Ok(new
                {
                    success = false,
                    error = "Duplicate request detected",
                    traceId = trace.TraceActivity?.TraceId.ToString(),
                    message = "Entire trace was skipped (nothing sent to Langfuse)"
                });
            }

            validationSpan.SetOutput(new { valid = true });
        }

        // Process the valid request
        using (var generation = trace.CreateGeneration("process-request",
                   request.Model,
                   request.Provider ?? "openai",
                   configure: g =>
                   {
                       g.SetTemperature(0.7);
                       g.SetPrompt(request.Prompt);
                   }))
        {
            await Task.Delay(200);

            var response = $"Processed: {request.Prompt}";
            generation.SetResponse(new GenAiResponse
            {
                Model = request.Model,
                InputTokens = 20,
                OutputTokens = 15,
                FinishReasons = ["stop"],
                Completion = response
            });

            trace.SetOutput(new { result = response });

            return Results.Ok(new
            {
                success = true,
                result = response,
                traceId = trace.TraceActivity?.TraceId.ToString(),
                message = "Trace was recorded normally"
            });
        }
    }
}