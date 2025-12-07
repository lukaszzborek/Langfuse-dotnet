using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace Langfuse.Example.OpenTelemetry.Services;

/// <summary>
///     OpenAI service that uses IOtelLangfuseTraceContext for tracing.
///     Using the simplified API with inline params.
/// </summary>
public class OtelOpenAiService
{
    private const string BaseUrl = "https://api.openai.com/v1";
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;
    private readonly IOtelLangfuseTraceContext _traceContext;

    public OtelOpenAiService(
        HttpClient httpClient,
        IOtelLangfuseTraceContext traceContext,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _traceContext = traceContext;
        _apiKey = configuration["OpenAI:ApiKey"] ?? "demo-key";
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<ChatCompletionResult?> GetChatCompletionAsync(
        string model,
        string prompt,
        CancellationToken cancellationToken = default)
    {
        // Create a generation observation with new simplified API
        using var generation = _traceContext.CreateGeneration("openai-chat-completion",
            model,
            "openai",
            new { prompt },
            g =>
            {
                g.SetInputMessages(new List<GenAiMessage>
                {
                    new() { Role = "user", Content = prompt }
                });
            });

        // Simulate API call (in real implementation, call OpenAI API)
        await Task.Delay(Random.Shared.Next(500, 4000), cancellationToken);

        // Simulated response
        var result = new ChatCompletionResult
        {
            Model = model,
            Content = $"This is a simulated response to: {prompt.Substring(0, Math.Min(50, prompt.Length))}...",
            InputTokens = prompt.Length / 4,
            OutputTokens = 50,
            FinishReason = "stop"
        };

        generation.SetResponse(new GenAiResponse
        {
            Model = model,
            InputTokens = result.InputTokens,
            OutputTokens = result.OutputTokens,
            FinishReasons = [result.FinishReason],
            Completion = result.Content
        });

        return result;
    }

    public async Task<EmbeddingResult> GetEmbeddingsAsync(
        string text,
        string model = "text-embedding-3-small",
        CancellationToken cancellationToken = default)
    {
        // Create embedding with new simplified API
        using var embedding = _traceContext.CreateEmbedding("openai-embedding",
            model,
            "openai",
            text);

        // Simulate API call
        await Task.Delay(200, cancellationToken);

        var result = new EmbeddingResult
        {
            Model = model,
            Dimensions = 1536,
            InputTokens = text.Length / 4
        };

        embedding.SetResponse(new GenAiResponse
        {
            Model = model,
            InputTokens = result.InputTokens
        });

        return result;
    }
}

public class ChatCompletionResult
{
    public string Model { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public string FinishReason { get; set; } = string.Empty;
}

public class EmbeddingResult
{
    public string Model { get; set; } = string.Empty;
    public int Dimensions { get; set; }
    public int InputTokens { get; set; }
}