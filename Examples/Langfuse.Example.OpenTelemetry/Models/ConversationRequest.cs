namespace Langfuse.Example.OpenTelemetry.Models;

public record ConversationRequest(
    string Model,
    string? Provider = null,
    int? Turns = null,
    string? ConversationId = null);
