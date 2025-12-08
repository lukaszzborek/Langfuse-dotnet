using Langfuse.Example.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace Langfuse.Example.OpenTelemetry.Services;

/// <summary>
///     Chat service that demonstrates multi-service tracing with OpenTelemetry.
///     Using the simplified OtelLangfuseTrace API.
/// </summary>
public class OtelChatService
{
    private readonly OtelDataService _dataService;
    private readonly OtelOpenAiService _openAiService;
    private readonly OtelLangfuseTrace _trace;

    public OtelChatService(
        OtelLangfuseTrace trace,
        OtelOpenAiService openAiService,
        OtelDataService dataService)
    {
        _trace = trace;
        _openAiService = openAiService;
        _dataService = dataService;
    }

    public async Task<ChatResponse> ChatAsync(
        ChatCompletionRequest request,
        CancellationToken cancellationToken = default)
    {
        // Start the trace with inline params
        _trace.StartTrace("chat-with-rag",
            "user-123",
            $"session-{Guid.NewGuid():N}",
            tags: ["chat", "rag", "otel-example"],
            input: new { prompt = request.Prompt, model = request.Model });

        // Create a span for the entire chat flow
        using (var chatSpan = _trace.CreateSpan("chat-flow",
                   "workflow",
                   "Main chat processing flow",
                   request.Prompt))
        {
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
            _trace.SetOutput(response);

            // Log completion event
            using var completionEvent = _trace.CreateEvent("chat-completed",
                new { prompt = request.Prompt },
                new { responseLength = response.Content.Length });

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
    private readonly OtelOpenAiService _openAiService;
    private readonly OtelLangfuseTrace _trace;

    public OtelDataService(
        OtelLangfuseTrace trace,
        OtelOpenAiService openAiService)
    {
        _trace = trace;
        _openAiService = openAiService;
    }

    public async Task<RetrievalResult> GetRelevantDataAsync(
        string query,
        CancellationToken cancellationToken = default)
    {
        // Create a span for retrieval
        using var retrievalSpan = _trace.CreateSpan("data-retrieval",
            "retrieval",
            "Retrieve relevant documents from knowledge base",
            query);

        // Step 1: Generate embeddings for the query
        var embeddings = await _openAiService.GetEmbeddingsAsync(query, cancellationToken: cancellationToken);

        // Step 2: Simulate vector search
        using (var searchSpan = _trace.CreateSpan("vector-search",
                   "search",
                   "Search vector database",
                   new { query, embeddingDimensions = embeddings.Dimensions }))
        {
            await Task.Delay(100, cancellationToken); // Simulate search
            searchSpan.SetOutput(new { resultsCount = 3 });
        }

        // Step 3: Create event for results
        var resultsEvent = _trace.CreateEvent("documents-retrieved",
            new { query },
            new { count = 3, source = "vector-db" });

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