namespace Langfuse.Example.OpenTelemetry.Models;

public record ChatCompletionRequest(
    string Model,
    string? Provider = null,
    double? Temperature = null,
    int? MaxTokens = null,
    double? TopP = null,
    string? ConversationId = null);
