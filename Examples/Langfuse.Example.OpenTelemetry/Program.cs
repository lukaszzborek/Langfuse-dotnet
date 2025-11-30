using System.Diagnostics;
using Langfuse.Example.OpenTelemetry;
using Langfuse.Example.OpenTelemetry.Models;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Resources;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Models;

Environment.SetEnvironmentVariable("OTEL_LOG_LEVEL", "debug");
Environment.SetEnvironmentVariable("OTEL_DOTNET_AUTO_LOG_DIRECTORY", "E:\\logs");

var builder = WebApplication.CreateBuilder(args);

// Configure OpenTelemetry with Langfuse exporter
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("langfuse-otel-webapi-example", serviceVersion: "1.0.0"))
    .WithTracing(tracing =>
    {
        // Add Langfuse exporter using configuration
        tracing.AddLangfuseExporter(builder.Configuration.GetSection("Langfuse"));
        //tracing.AddConsoleExporter();
        tracing.AddSource("Langfuse.Example.OpenTelemetry");
    });

// Register ActivitySource as singleton
builder.Services.AddSingleton(new ActivitySource("Langfuse.Example.OpenTelemetry"));

var app = builder.Build();

// Simple chat completion endpoint
app.MapPost("/chat-completion",
    async ([FromServices] ActivitySource activitySource, [FromBody] ChatCompletionRequest request) =>
    {
        try
        {
            using var trace =
                GenAiActivityHelper.CreateTraceActivity(activitySource, "chat-completion-example", new TraceConfig());

            var config = new GenAiChatCompletionConfig
            {
                Provider = request.Provider ?? "openai",
                Model = request.Model,
                Temperature = request.Temperature ?? 0.7,
                MaxTokens = request.MaxTokens ?? 1000,
                TopP = request.TopP,
                ConversationId = request.ConversationId
            };

            using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
                activitySource,
                "chat-completion",
                config);

            if (activity == null)
            {
                return Results.BadRequest(new { error = "Failed to create activity" });
            }

            using var tool =
                GenAiActivityHelper.CreateToolCallActivity(activitySource, "tool-call", "tool-name", "function",
                    "tool-id");

            // Record input messages (chat history)
            var inputMessages = new List<GenAiMessage>
            {
                new() { Role = "system", Content = "You are a helpful assistant." },
                new() { Role = "user", Content = request.Prompt }
            };
            GenAiActivityHelper.RecordInputMessages(activity, inputMessages);

            // Simulate API call
            await Task.Delay(500);

            // Simulated completion from the model
            var completionText = "I'm doing well, thank you for asking! How can I assist you today?";

            // Record response with completion included
            var response = new GenAiResponse
            {
                ResponseId = $"chatcmpl-{Guid.NewGuid():N}",
                Model = config.Model,
                InputTokens = 25,
                OutputTokens = 150,
                FinishReasons = new[] { "stop" },
                Completion = completionText // Output message included in response
            };

            GenAiActivityHelper.RecordResponse(activity, response);

            return Results.Ok(new
            {
                success = true,
                activityId = activity.Id,
                model = response.Model,
                completion = completionText,
                inputTokens = response.InputTokens,
                outputTokens = response.OutputTokens,
                finishReasons = response.FinishReasons
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    });

// Text completion endpoint
app.MapPost("/text-completion",
    async ([FromServices] ActivitySource activitySource, [FromBody] TextCompletionRequest request) =>
    {
        try
        {
            var config = new GenAiTextCompletionConfig
            {
                Provider = request.Provider ?? "openai",
                Model = request.Model,
                Temperature = request.Temperature ?? 0.5,
                MaxTokens = request.MaxTokens ?? 256
            };

            using var activity = GenAiActivityHelper.CreateTextCompletionActivity(
                activitySource,
                "text-completion",
                config);

            if (activity == null)
            {
                return Results.BadRequest(new { error = "Failed to create activity" });
            }

            await Task.Delay(400);

            var response = new GenAiResponse
            {
                ResponseId = $"cmpl-{Guid.NewGuid():N}",
                Model = config.Model,
                InputTokens = 15,
                OutputTokens = 89,
                FinishReasons = new[] { "stop" }
            };

            GenAiActivityHelper.RecordResponse(activity, response);

            return Results.Ok(new
            {
                success = true,
                activityId = activity.Id,
                model = response.Model,
                tokensGenerated = response.OutputTokens
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    });

// Embeddings endpoint
app.MapPost("/embeddings", async ([FromServices] ActivitySource activitySource, [FromBody] EmbeddingsRequest request) =>
{
    try
    {
        using var activity = GenAiActivityHelper.CreateEmbeddingsActivity(
            activitySource,
            "generate-embeddings",
            request.Provider ?? "openai",
            request.Model);

        if (activity == null)
        {
            return Results.BadRequest(new { error = "Failed to create activity" });
        }

        await Task.Delay(200);

        activity?.SetTag("gen_ai.embeddings.dimension.count", 1536);

        var response = new GenAiResponse
        {
            InputTokens = 8,
            OutputTokens = 0
        };

        GenAiActivityHelper.RecordResponse(activity, response);

        return Results.Ok(new
        {
            success = true,
            activityId = activity.Id,
            dimensions = 1536,
            inputTokens = response.InputTokens
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Function calling endpoint
app.MapPost("/function-calling",
    async ([FromServices] ActivitySource activitySource, [FromBody] FunctionCallingRequest request) =>
    {
        try
        {
            var config = new GenAiChatCompletionConfig
            {
                Provider = request.Provider ?? "openai",
                Model = request.Model,
                Temperature = request.Temperature ?? 0.3,
                ConversationId = request.ConversationId
            };

            using var chatActivity = GenAiActivityHelper.CreateChatCompletionActivity(
                activitySource,
                "chat-with-functions",
                config);

            if (chatActivity == null)
            {
                return Results.BadRequest(new { error = "Failed to create activity" });
            }

            // Record input messages
            var inputMessages = new List<GenAiMessage>
            {
                new() { Role = "system", Content = "You are a helpful assistant with access to weather information." },
                new() { Role = "user", Content = "What's the weather like in San Francisco?" }
            };
            GenAiActivityHelper.RecordInputMessages(chatActivity, inputMessages);

            await Task.Delay(300);

            // Model decides to call a function - record response with tool call output
            var assistantMessageWithToolCall = new GenAiMessage
            {
                Role = "assistant",
                Content = null,
                ToolCalls = new List<GenAiToolCall>
                {
                    new()
                    {
                        Id = "call_abc123",
                        Type = "function",
                        Function = new GenAiToolCallFunction
                        {
                            Name = "get_weather",
                            Arguments = "{\"location\":\"San Francisco\",\"unit\":\"celsius\"}"
                        }
                    }
                }
            };

            var response = new GenAiResponse
            {
                ResponseId = $"chatcmpl-func-{Guid.NewGuid():N}",
                Model = config.Model,
                InputTokens = 120,
                OutputTokens = 30,
                FinishReasons = new[] { "tool_calls" },
                OutputMessages = new List<GenAiMessage> { assistantMessageWithToolCall }
            };
            GenAiActivityHelper.RecordResponse(chatActivity, response);

            // Execute the tool call as a child activity
            string? toolResult = null;
            using (var toolActivity = GenAiActivityHelper.CreateToolCallActivity(
                       activitySource,
                       "execute-get-weather",
                       "get_weather",
                       "function",
                       "call_abc123"))
            {
                if (toolActivity != null)
                {
                    toolActivity.SetTag("gen_ai.tool.call.arguments",
                        "{\"location\":\"San Francisco\",\"unit\":\"celsius\"}");
                    await Task.Delay(100);

                    toolResult = "{\"temperature\":18,\"condition\":\"sunny\"}";
                    toolActivity.SetTag("gen_ai.tool.call.result", toolResult);
                }
            }

            // Send function result back to model (second turn)
            await Task.Delay(300);

            // Final assistant response after tool execution
            var finalCompletion = "The weather in San Francisco is currently sunny with a temperature of 18°C.";

            var finalResponse = new GenAiResponse
            {
                ResponseId = $"chatcmpl-func-{Guid.NewGuid():N}",
                Model = config.Model,
                InputTokens = 150,
                OutputTokens = 45,
                FinishReasons = new[] { "stop" },
                Completion = finalCompletion
            };
            GenAiActivityHelper.RecordResponse(chatActivity, finalResponse);

            return Results.Ok(new
            {
                success = true,
                activityId = chatActivity.Id,
                toolCalled = "get_weather",
                toolResult,
                finalResponse = finalCompletion,
                totalInputTokens = 120 + 150,
                totalOutputTokens = 30 + 45
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    });

// Multi-turn conversation endpoint
app.MapPost("/conversation",
    async ([FromServices] ActivitySource activitySource, [FromBody] ConversationRequest request) =>
    {
        try
        {
            var conversationId = request.ConversationId ?? $"conv-{Guid.NewGuid():N}";
            var turns = request.Turns ?? 3;
            var results = new List<object>();

            for (var i = 1; i <= turns; i++)
            {
                var config = new GenAiChatCompletionConfig
                {
                    Provider = request.Provider ?? "openai",
                    Model = request.Model,
                    Temperature = 0.7,
                    ConversationId = conversationId
                };

                using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
                    activitySource,
                    $"chat-turn-{i}",
                    config);

                if (activity != null)
                {
                    activity.SetTag("conversation.turn", i);
                    await Task.Delay(400);

                    var inputTokens = 20 + (i - 1) * 90;
                    var outputTokens = 80 + (i - 1) * 20;

                    var response = new GenAiResponse
                    {
                        ResponseId = $"chatcmpl-turn-{i}",
                        Model = config.Model,
                        InputTokens = inputTokens,
                        OutputTokens = outputTokens,
                        FinishReasons = new[] { "stop" }
                    };

                    GenAiActivityHelper.RecordResponse(activity, response);

                    results.Add(new
                    {
                        turn = i,
                        inputTokens,
                        outputTokens
                    });
                }
            }

            return Results.Ok(new
            {
                success = true,
                conversationId,
                totalTurns = turns,
                turns = results
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    });

// Run all examples endpoint
app.MapPost("/run-all-examples", async ([FromServices] ActivitySource activitySource) =>
{
    try
    {
        await GenAiExamples.RunAllExamples(activitySource);

        return Results.Ok(new
        {
            success = true,
            message = "All examples completed successfully"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();