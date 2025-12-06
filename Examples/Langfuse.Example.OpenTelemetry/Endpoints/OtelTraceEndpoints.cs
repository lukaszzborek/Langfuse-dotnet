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

        return app;
    }

    private static async Task<IResult> OtelTraceExample([FromBody] ChatCompletionRequest request)
    {
        using var trace = new OtelLangfuseTrace("customer-support-conversation", new TraceConfig
        {
            UserId = "user-123",
            SessionId = "session-456",
            Tags = ["support", "billing"],
            Metadata = new Dictionary<string, object>
            {
                ["source"] = "web-chat",
                ["priority"] = "high"
            }
        });

        using (var generation = trace.CreateGenerationScoped("intent-classification", new GenAiChatCompletionConfig
        {
            Provider = request.Provider ?? "openai",
            Model = request.Model,
            Temperature = 0.3,
            MaxTokens = 100
        }))
        {
            generation.SetInputMessages(new List<GenAiMessage>
            {
                new() { Role = "system", Content = "Classify the user intent." },
                new() { Role = "user", Content = request.Prompt }
            });

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

        using (var toolCall = trace.CreateToolCallScoped("lookup-account", "get_account_info",
                   "Retrieves customer account information"))
        {
            toolCall.SetArguments(new { customer_id = "cust-789" });
            await Task.Delay(100);
            toolCall.SetResult(new { balance = 125.50, status = "active" });
        }

        using (var responseGen = trace.CreateGeneration("generate-response", new GenAiChatCompletionConfig
        {
            Provider = request.Provider ?? "openai",
            Model = request.Model,
            Temperature = 0.7,
            MaxTokens = 500
        }))
        {
            responseGen.SetInputMessages(new List<GenAiMessage>
            {
                new() { Role = "system", Content = "You are a helpful support agent." },
                new() { Role = "user", Content = request.Prompt },
                new() { Role = "assistant", Content = "intent: billing_inquiry" },
                new() { Role = "user", Content = "Account balance: $125.50" }
            });

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
            traceId = trace.TraceActivity?.TraceId.ToString(),
            collectedInput = trace.CollectedInput,
            collectedOutput = trace.CollectedOutput
        });
    }

    private static async Task<IResult> OtelNestedExample([FromBody] ChatCompletionRequest request)
    {
        using var trace = new OtelLangfuseTrace("rag-pipeline", new TraceConfig
        {
            UserId = "user-456",
            Tags = ["rag", "knowledge-base"]
        });

        trace.SetInput(new { query = request.Prompt });

        using (var retrievalSpan = trace.CreateSpanScoped("document-retrieval", new SpanConfig
        {
            SpanType = "retrieval",
            Description = "Retrieve relevant documents from vector store"
        }))
        {
            retrievalSpan.SetInput(request.Prompt);
            await Task.Delay(150);
            retrievalSpan.SetOutput(new { documents = new[] { "doc1.pdf", "doc2.pdf" }, count = 2 });

            using (var embedding = trace.CreateEmbedding("query-embedding", new GenAiEmbeddingsConfig
            {
                Provider = "openai",
                Model = "text-embedding-3-small",
                Dimensions = 1536
            }))
            {
                embedding.SetText(request.Prompt);
                await Task.Delay(50);
                embedding.SetResponse(new GenAiResponse { InputTokens = 15 });
            }
        }

        using var logEvent = trace.CreateEvent("documents-ranked",
            new { query = request.Prompt },
            new { topDocId = "doc1.pdf", score = 0.95 });

        using (var generation = trace.CreateGeneration("answer-generation", new GenAiChatCompletionConfig
        {
            Provider = request.Provider ?? "openai",
            Model = request.Model,
            Temperature = 0.5
        }))
        {
            generation.SetInputMessages(new List<GenAiMessage>
            {
                new() { Role = "system", Content = "Answer based on the retrieved documents." },
                new() { Role = "user", Content = request.Prompt }
            });

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
        using var trace = new OtelLangfuseTrace("research-agent", new TraceConfig
        {
            UserId = "user-789",
            Tags = ["agent", "research"]
        });

        using (var agent = trace.CreateAgentScoped("research-assistant", new GenAiAgentConfig
        {
            Id = "agent-001",
            Name = "Research Assistant",
            Description = "An agent that researches topics and provides summaries"
        }))
        {
            agent.SetInput(request.Prompt);

            for (var i = 1; i <= 3; i++)
            {
                using (var toolCall = trace.CreateToolCall($"search-step-{i}", "web_search",
                           "Search the web for information"))
                {
                    toolCall.SetArguments(new { query = $"{request.Prompt} - aspect {i}" });
                    await Task.Delay(100);
                    toolCall.SetResult(new { results = new[] { $"Result {i}a", $"Result {i}b" } });
                }
            }

            using (var generation = trace.CreateGeneration("synthesize-results", new GenAiChatCompletionConfig
            {
                Provider = request.Provider ?? "openai",
                Model = request.Model,
                Temperature = 0.7
            }))
            {
                generation.SetPrompt($"Synthesize research findings about: {request.Prompt}");
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
}
