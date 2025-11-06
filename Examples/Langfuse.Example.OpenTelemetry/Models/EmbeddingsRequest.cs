namespace Langfuse.Example.OpenTelemetry.Models;

public record EmbeddingsRequest(
    string Model,
    string? Provider = null);
