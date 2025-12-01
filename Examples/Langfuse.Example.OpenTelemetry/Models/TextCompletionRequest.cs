namespace Langfuse.Example.OpenTelemetry.Models;

public record TextCompletionRequest(
    string Model,
    string? Provider = null,
    double? Temperature = null,
    int? MaxTokens = null);
