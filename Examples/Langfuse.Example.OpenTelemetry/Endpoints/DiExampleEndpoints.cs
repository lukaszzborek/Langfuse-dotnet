using Langfuse.Example.OpenTelemetry.Models;
using Langfuse.Example.OpenTelemetry.Services;
using Microsoft.AspNetCore.Mvc;

namespace Langfuse.Example.OpenTelemetry.Endpoints;

public static class DiExampleEndpoints
{
    public static IEndpointRouteBuilder MapDiExamples(this IEndpointRouteBuilder app)
    {
        app.MapPost("/chatDi", ChatDi);
        app.MapPost("/run-parallel-summary", RunParallelSummary);

        return app;
    }

    private static async Task<IResult> ChatDi(
        [FromServices] OtelChatService chatService,
        [FromBody] ChatCompletionRequest request)
    {
        try
        {
            var response = await chatService.ChatAsync(request);

            return Results.Ok(new
            {
                success = true,
                response.Content,
                response.Model,
                response.InputTokens,
                response.OutputTokens,
                response.ContextUsed
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    }

    private static async Task<IResult> RunParallelSummary(
        [FromServices] ParallelSummarizationService parallelService)
    {
        try
        {
            var articlesToSummarize = new List<Article>
            {
                new()
                {
                    Id = 1,
                    Title = "AI in Healthcare",
                    Content = "The role of artificial intelligence in revolutionizing medical diagnostics and patient care is growing rapidly...",
                    Keywords = "AI, Healthcare, Diagnostics"
                },
                new()
                {
                    Id = 2,
                    Title = "Quantum Computing Future",
                    Content = "Quantum computing promises to solve problems currently intractable for classical computers, impacting cryptography and materials science...",
                    Keywords = "Quantum, Computing, Cryptography"
                },
                new()
                {
                    Id = 3,
                    Title = "Sustainable Energy Solutions",
                    Content = "Innovations in solar, wind, and geothermal energy are key to combating climate change and ensuring a sustainable future...",
                    Keywords = "Energy, Sustainability, Climate"
                }
            };

            await parallelService.SummarizeArticlesInParallel(articlesToSummarize);

            return Results.Ok(new
            {
                success = true,
                message = "Parallel summarization example initiated. Check console for output and Langfuse for traces."
            });
        }
        catch (Exception ex)
        {
            return Results.BadRequest(new { error = $"Failed to run parallel summarization: {ex.Message}" });
        }
    }
}
