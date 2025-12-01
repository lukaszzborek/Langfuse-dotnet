namespace Langfuse.Example.OpenTelemetry.Models;

public record FunctionCallingRequest(
    string Model,
    string? Provider = null,
    double? Temperature = null,
    string? ConversationId = null);
