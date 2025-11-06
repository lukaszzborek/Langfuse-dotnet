using System.Diagnostics;

namespace Langfuse.Example.OpenTelemetry;

/// <summary>
///     Examples demonstrating Gen AI semantic conventions with OpenTelemetry.
/// </summary>
public static class GenAiExamples
{
    /// <summary>
    ///     Example: Simple chat completion with OpenAI
    /// </summary>
    public static async Task ChatCompletionExample(ActivitySource activitySource)
    {
        Console.WriteLine("\n=== Chat Completion Example ===");

        var config = new GenAiChatCompletionConfig
        {
            Provider = "openai",
            Model = "gpt-4",
            Temperature = 0.7,
            MaxTokens = 1000,
            TopP = 1.0,
            ConversationId = "conv-123"
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
            activitySource,
            "chat-completion",
            config);

        if (activity == null)
        {
            Console.WriteLine("Failed to create activity");
            return;
        }

        try
        {
            Console.WriteLine($"  Starting chat completion with {config.Model}...");

            // Simulate API call
            await Task.Delay(500);

            // Record successful response
            var response = new GenAiResponse
            {
                ResponseId = "chatcmpl-123456",
                Model = "gpt-4-0613",
                InputTokens = 25,
                OutputTokens = 150,
                FinishReasons = new[] { "stop" }
            };

            GenAiActivityHelper.RecordResponse(activity, response);

            Console.WriteLine(
                $"  ✓ Completed: {response.InputTokens} input tokens, {response.OutputTokens} output tokens");
            Console.WriteLine($"  Activity ID: {activity.Id}");
        }
        catch (Exception ex)
        {
            GenAiActivityHelper.RecordError(activity, ex);
            Console.WriteLine($"  ✗ Error: {ex.Message}");
        }
    }

    /// <summary>
    ///     Example: Chat completion with multiple choices
    /// </summary>
    public static async Task ChatCompletionWithMultipleChoicesExample(ActivitySource activitySource)
    {
        Console.WriteLine("\n=== Chat Completion with Multiple Choices ===");

        var config = new GenAiChatCompletionConfig
        {
            Provider = "openai",
            Model = "gpt-3.5-turbo",
            Temperature = 1.0,
            MaxTokens = 500,
            ChoiceCount = 3,
            ConversationId = "conv-456"
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
            activitySource,
            "chat-completion-multiple",
            config);

        if (activity == null)
        {
            return;
        }

        Console.WriteLine($"  Requesting {config.ChoiceCount} completions...");
        await Task.Delay(800);

        var response = new GenAiResponse
        {
            ResponseId = "chatcmpl-789012",
            Model = "gpt-3.5-turbo-0125",
            InputTokens = 30,
            OutputTokens = 420,
            FinishReasons = new[] { "stop", "stop", "length" }
        };

        GenAiActivityHelper.RecordResponse(activity, response);
        Console.WriteLine($"  ✓ Generated {config.ChoiceCount} choices");
        Console.WriteLine($"  Finish reasons: {string.Join(", ", response.FinishReasons)}");
    }

    /// <summary>
    ///     Example: Text completion (legacy style)
    /// </summary>
    public static async Task TextCompletionExample(ActivitySource activitySource)
    {
        Console.WriteLine("\n=== Text Completion Example ===");

        var config = new GenAiTextCompletionConfig
        {
            Provider = "openai",
            Model = "gpt-3.5-turbo-instruct",
            Temperature = 0.5,
            MaxTokens = 256
        };

        using var activity = GenAiActivityHelper.CreateTextCompletionActivity(
            activitySource,
            "text-completion",
            config);

        if (activity == null)
        {
            return;
        }

        Console.WriteLine("  Generating text completion...");
        await Task.Delay(400);

        var response = new GenAiResponse
        {
            ResponseId = "cmpl-345678",
            Model = config.Model,
            InputTokens = 15,
            OutputTokens = 89,
            FinishReasons = new[] { "stop" }
        };

        GenAiActivityHelper.RecordResponse(activity, response);
        Console.WriteLine($"  ✓ Completed: {response.OutputTokens} tokens generated");
    }

    /// <summary>
    ///     Example: Embeddings generation
    /// </summary>
    public static async Task EmbeddingsExample(ActivitySource activitySource)
    {
        Console.WriteLine("\n=== Embeddings Example ===");

        using var activity = GenAiActivityHelper.CreateEmbeddingsActivity(
            activitySource,
            "generate-embeddings",
            "openai",
            "text-embedding-ada-002");

        if (activity == null)
        {
            return;
        }

        Console.WriteLine("  Generating embeddings for text...");
        await Task.Delay(200);

        activity?.SetTag("gen_ai.embeddings.dimension.count", 1536);

        var response = new GenAiResponse
        {
            InputTokens = 8,
            OutputTokens = 0
        };

        GenAiActivityHelper.RecordResponse(activity, response);
        Console.WriteLine("  ✓ Generated embeddings (1536 dimensions)");
    }

    /// <summary>
    ///     Example: Chat with function/tool calling
    /// </summary>
    public static async Task FunctionCallingExample(ActivitySource activitySource)
    {
        Console.WriteLine("\n=== Function Calling Example ===");

        var config = new GenAiChatCompletionConfig
        {
            Provider = "openai",
            Model = "gpt-4",
            Temperature = 0.3,
            ConversationId = "conv-789"
        };

        using var chatActivity = GenAiActivityHelper.CreateChatCompletionActivity(
            activitySource,
            "chat-with-functions",
            config);

        if (chatActivity == null)
        {
            return;
        }

        Console.WriteLine("  Sending chat request with available functions...");
        await Task.Delay(300);

        // Model decides to call a function
        var response = new GenAiResponse
        {
            ResponseId = "chatcmpl-func-123",
            Model = "gpt-4-0613",
            InputTokens = 120,
            OutputTokens = 30,
            FinishReasons = new[] { "tool_calls" }
        };

        GenAiActivityHelper.RecordResponse(chatActivity, response);
        Console.WriteLine("  Model requested tool call (finish reason: tool_calls)");

        // Execute the tool call as a child activity
        using (var toolActivity = GenAiActivityHelper.CreateToolCallActivity(
                   activitySource,
                   "execute-get-weather",
                   "get_weather",
                   "function",
                   "call_abc123"))
        {
            if (toolActivity != null)
            {
                Console.WriteLine("    Executing get_weather function...");
                toolActivity.SetTag("gen_ai.tool.call.arguments",
                    "{\"location\":\"San Francisco\",\"unit\":\"celsius\"}");
                await Task.Delay(100);

                toolActivity.SetTag("gen_ai.tool.call.result", "{\"temperature\":18,\"condition\":\"sunny\"}");
                Console.WriteLine("    ✓ Function executed successfully");
            }
        }

        // Send function result back to model
        Console.WriteLine("  Sending function result back to model...");
        await Task.Delay(300);

        var finalResponse = new GenAiResponse
        {
            ResponseId = "chatcmpl-func-456",
            Model = "gpt-4-0613",
            InputTokens = 150,
            OutputTokens = 45,
            FinishReasons = new[] { "stop" }
        };

        GenAiActivityHelper.RecordResponse(chatActivity, finalResponse);
        Console.WriteLine("  ✓ Final response generated");
        Console.WriteLine($"  Total tokens: {120 + 150} input, {30 + 45} output");
    }

    /// <summary>
    ///     Example: Conversation with multiple turns
    /// </summary>
    public static async Task ConversationExample(ActivitySource activitySource)
    {
        Console.WriteLine("\n=== Multi-Turn Conversation Example ===");

        var conversationId = $"conv-{Guid.NewGuid():N}";
        Console.WriteLine($"  Conversation ID: {conversationId}");

        // Turn 1
        await ExecuteChatTurn(activitySource, conversationId, 1, 20, 80);

        // Turn 2
        await ExecuteChatTurn(activitySource, conversationId, 2, 110, 95);

        // Turn 3
        await ExecuteChatTurn(activitySource, conversationId, 3, 215, 120);

        Console.WriteLine("  ✓ Conversation completed (3 turns)");
    }

    private static async Task ExecuteChatTurn(
        ActivitySource activitySource,
        string conversationId,
        int turnNumber,
        int inputTokens,
        int outputTokens)
    {
        var config = new GenAiChatCompletionConfig
        {
            Provider = "openai",
            Model = "gpt-4",
            Temperature = 0.7,
            ConversationId = conversationId
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
            activitySource,
            $"chat-turn-{turnNumber}",
            config);

        if (activity == null)
        {
            return;
        }

        activity?.SetTag("conversation.turn", turnNumber);
        await Task.Delay(400);

        var response = new GenAiResponse
        {
            ResponseId = $"chatcmpl-turn-{turnNumber}",
            Model = "gpt-4-0613",
            InputTokens = inputTokens,
            OutputTokens = outputTokens,
            FinishReasons = new[] { "stop" }
        };

        GenAiActivityHelper.RecordResponse(activity, response);
        Console.WriteLine($"    Turn {turnNumber}: {inputTokens} → {outputTokens} tokens");
    }

    /// <summary>
    ///     Example: Error handling in Gen AI operations
    /// </summary>
    public static async Task ErrorHandlingExample(ActivitySource activitySource)
    {
        Console.WriteLine("\n=== Error Handling Example ===");

        var config = new GenAiChatCompletionConfig
        {
            Provider = "openai",
            Model = "gpt-4",
            Temperature = 0.7
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
            activitySource,
            "chat-with-error",
            config);

        if (activity == null)
        {
            return;
        }

        try
        {
            Console.WriteLine("  Attempting chat completion...");
            await Task.Delay(100);

            // Simulate various error scenarios
            throw new InvalidOperationException("Rate limit exceeded: 429 Too Many Requests");
        }
        catch (Exception ex)
        {
            GenAiActivityHelper.RecordError(activity, ex);
            Console.WriteLine($"  ✗ Error occurred: {ex.Message}");
            Console.WriteLine("  Error details recorded in activity");
        }
    }

    /// <summary>
    ///     Example: Claude (Anthropic) chat completion
    /// </summary>
    public static async Task ClaudeChatExample(ActivitySource activitySource)
    {
        Console.WriteLine("\n=== Claude Chat Example ===");

        var config = new GenAiChatCompletionConfig
        {
            Provider = "anthropic",
            Model = "claude-3-5-sonnet-20241022",
            Temperature = 1.0,
            MaxTokens = 1024,
            TopP = 0.95,
            TopK = 40
        };

        using var activity = GenAiActivityHelper.CreateChatCompletionActivity(
            activitySource,
            "claude-chat",
            config);

        if (activity == null)
        {
            return;
        }

        Console.WriteLine($"  Sending request to Claude ({config.Model})...");
        await Task.Delay(600);

        var response = new GenAiResponse
        {
            ResponseId = "msg_123abc",
            Model = config.Model,
            InputTokens = 45,
            OutputTokens = 280,
            FinishReasons = new[] { "end_turn" }
        };

        GenAiActivityHelper.RecordResponse(activity, response);
        Console.WriteLine($"  ✓ Completed: {response.InputTokens} input, {response.OutputTokens} output tokens");
    }

    /// <summary>
    ///     Run all Gen AI examples
    /// </summary>
    public static async Task RunAllExamples(ActivitySource activitySource)
    {
        Console.WriteLine("\n╔════════════════════════════════════════════╗");
        Console.WriteLine("║  Gen AI OpenTelemetry Examples             ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");

        await ChatCompletionExample(activitySource);
        await ChatCompletionWithMultipleChoicesExample(activitySource);
        await TextCompletionExample(activitySource);
        await EmbeddingsExample(activitySource);
        await FunctionCallingExample(activitySource);
        await ConversationExample(activitySource);
        await ClaudeChatExample(activitySource);
        await ErrorHandlingExample(activitySource);

        Console.WriteLine("\n╔════════════════════════════════════════════╗");
        Console.WriteLine("║  All examples completed!                   ║");
        Console.WriteLine("╚════════════════════════════════════════════╝");
    }
}