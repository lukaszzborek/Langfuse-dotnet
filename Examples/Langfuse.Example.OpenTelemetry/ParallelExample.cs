using System.Diagnostics;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Models;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace Langfuse.Example.OpenTelemetry;

public class ParallelSummarizationService
{
    private readonly ILogger<ParallelSummarizationService> _logger;

    public ParallelSummarizationService(ILogger<ParallelSummarizationService> logger)
    {
        _logger = logger;
    }

    public async Task SummarizeArticlesInParallel(List<Article> articles)
    {
        _logger.LogInformation("Starting parallel summarization for {ArticleCount} articles", articles.Count);

        var tasks = new List<Task>();

        foreach (var article in articles)
        {
            tasks.Add(Task.Run(async () =>
            {
                // Create detached trace for parallel processing (new root, independent of parent context)
                using var trace = OtelLangfuseTrace.CreateDetachedTrace($"ArticleSummary - {article.Id}",
                    "user-parallel-123",
                    tags: ["parallel", "summarization"]);

                _logger.LogInformation("Trace '{TraceName}' started for Article {ArticleId}",
                    trace.TraceActivity?.DisplayName, article.Id);

                try
                {
                    // LLM generation for summary
                    using var generation = trace.CreateGeneration("LLM Call - Summary",
                        "gpt-3.5-turbo",
                        "OpenAI",
                        configure: g =>
                        {
                            g.SetTemperature(0.7);
                            g.SetInputMessages(new List<GenAiMessage>
                            {
                                new()
                                {
                                    Role = "user",
                                    Content =
                                        $"Summarize: {article.Content.Substring(0, Math.Min(100, article.Content.Length))}..."
                                }
                            });
                        });

                    await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 3)));

                    generation.SetOutput("This is a simulated summary of the article content.");
                    generation.SetResponse(new GenAiResponse
                    {
                        ResponseId = Guid.NewGuid().ToString(),
                        Model = "gpt-3.5-turbo",
                        InputTokens = 150,
                        OutputTokens = 50,
                        TotalCost = 0.0002m
                    });

                    _logger.LogInformation("Generation for Article {ArticleId} completed", article.Id);

                    // RAG lookup span - automatically becomes child of generation via Activity.Current
                    using (var ragSpan = trace.CreateSpan("RAG Lookup",
                               "retrieval",
                               input: "keywords for RAG: " + article.Keywords))
                    {
                        _logger.LogInformation("Span 'RAG Lookup' started for Article {ArticleId}", article.Id);
                        await Task.Delay(500);
                        ragSpan.SetOutput("Retrieved 3 relevant documents.");
                        _logger.LogInformation("Span 'RAG Lookup' completed for Article {ArticleId}", article.Id);
                    }

                    _logger.LogInformation("Trace '{TraceName}' completed for Article {ArticleId}",
                        trace.TraceActivity?.DisplayName, article.Id);
                }
                catch (Exception ex)
                {
                    trace.TraceActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    GenAiActivityHelper.RecordError(trace.TraceActivity!, ex);
                    _logger.LogError(ex, "Trace for Article {ArticleId} encountered an error", article.Id);
                }
            }));
        }

        await Task.WhenAll(tasks);
        _logger.LogInformation("All parallel summarizations finished");
    }
}

public class Article
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Keywords { get; set; } = string.Empty;
}