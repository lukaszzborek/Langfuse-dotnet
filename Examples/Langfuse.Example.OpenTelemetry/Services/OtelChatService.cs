using Langfuse.Example.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace Langfuse.Example.OpenTelemetry.Services;

/// <summary>
///     Chat service that demonstrates multi-service tracing with OpenTelemetry.
///     Similar to ChatService but using IOtelLangfuseTraceContext.
/// </summary>
public class OtelChatService
{
    private readonly IOtelLangfuseTraceContext _traceContext;
    private readonly OtelOpenAiService _openAiService;
    private readonly OtelDataService _dataService;

    public OtelChatService(
        IOtelLangfuseTraceContext traceContext,
        OtelOpenAiService openAiService,
        OtelDataService dataService)
    {
        _traceContext = traceContext;
        _openAiService = openAiService;
        _dataService = dataService;
    }

    public async Task<ChatResponse> ChatAsync(
        ChatCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        // Start the trace - this initializes the context for all services
        using var _ = _traceContext.StartTrace("chat-with-rag", new TraceConfig
        {
            UserId = "user-123",
            SessionId = $"session-{Guid.NewGuid():N}",
            Tags = ["chat", "rag", "otel-example"],
            Metadata = new Dictionary<string, object>
            {
                ["source"] = "otel-webapi",
                ["model"] = request.Model
            }
        });

        _traceContext.SetInput(new { prompt = request.Prompt, model = request.Model });

        // Create a scoped span for the entire chat flow
        using (var chatSpan = _traceContext.CreateSpanScoped("chat-flow", new SpanConfig
        {
            SpanType = "workflow",
            Description = "Main chat processing flow"
        }))
        {
            chatSpan.SetInput(request.Prompt);

            // Step 1: Retrieve relevant data (uses OtelDataService)
            var contextData = await _dataService.GetRelevantDataAsync(
                request.Prompt ?? "Hello",
                cancellationToken);

            // Step 2: Build the prompt with context
            var augmentedPrompt = BuildAugmentedPrompt(request.Prompt ?? "Hello", contextData);

            // Step 3: Get completion from OpenAI (uses OtelOpenAiService)
            var completion = await _openAiService.GetChatCompletionAsync(
                request.Model,
                augmentedPrompt,
                cancellationToken);

            var response = new ChatResponse
            {
                Content = completion?.Content ?? "No response",
                Model = completion?.Model ?? request.Model,
                InputTokens = completion?.InputTokens ?? 0,
                OutputTokens = completion?.OutputTokens ?? 0,
                ContextUsed = contextData.Documents.Count > 0
            };

            chatSpan.SetOutput(response);
            _traceContext.SetOutput(response);

            // Log completion event
            using var completionEvent = _traceContext.CreateEvent("chat-completed",
                input: new { prompt = request.Prompt },
                output: new { responseLength = response.Content.Length });

            return response;
        }
    }

    private static string BuildAugmentedPrompt(string userPrompt, RetrievalResult context)
    {
        if (context.Documents.Count == 0)
        {
            return userPrompt;
        }

        return $"""
                <context>
                {string.Join("\n", context.Documents.Select(d => $"- {d}"))}
                </context>

                <user_query>
                {userPrompt}
                </user_query>

                Please answer the user's query using the provided context.
                """;
    }
}

/// <summary>
///     Data service that demonstrates tracing for data retrieval operations.
/// </summary>
public class OtelDataService
{
    private readonly IOtelLangfuseTraceContext _traceContext;
    private readonly OtelOpenAiService _openAiService;

    public OtelDataService(
        IOtelLangfuseTraceContext traceContext,
        OtelOpenAiService openAiService)
    {
        _traceContext = traceContext;
        _openAiService = openAiService;
    }

    public async Task<RetrievalResult> GetRelevantDataAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        // Create a scoped span for retrieval
        using var retrievalSpan = _traceContext.CreateSpanScoped("data-retrieval", new SpanConfig
        {
            SpanType = "retrieval",
            Description = "Retrieve relevant documents from knowledge base"
        });

        retrievalSpan.SetInput(query);

        // Step 1: Generate embeddings for the query
        var embeddings = await _openAiService.GetEmbeddingsAsync(query, cancellationToken: cancellationToken);

        // Step 2: Simulate vector search
        using (var searchSpan = _traceContext.CreateSpan("vector-search", new SpanConfig
        {
            SpanType = "search",
            Description = "Search vector database"
        }))
        {
            searchSpan.SetInput(new { query, embeddingDimensions = embeddings.Dimensions });
            await Task.Delay(100, cancellationToken); // Simulate search

            searchSpan.SetOutput(new { resultsCount = 3 });
        }

        // Step 3: Create event for results
        using var resultsEvent = _traceContext.CreateEvent("documents-retrieved",
            input: new { query },
            output: new { count = 3, source = "vector-db" });

        var result = new RetrievalResult
        {
            Documents =
            [
                "Document 1: Relevant information about the topic...",
                "Document 2: Additional context for the query...",
                "Document 3: Supporting details..."
            ],
            Scores = [0.95, 0.87, 0.82]
        };

        retrievalSpan.SetOutput(result);

        return result;
    }
}

public class ChatResponse
{
    public string Content { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public bool ContextUsed { get; set; }
}

public class RetrievalResult
{
    public List<string> Documents { get; set; } = [];
    public List<double> Scores { get; set; } = [];
}
