using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

/// <summary>
///     Represents a client for interacting with Langfuse api
/// </summary>
public partial interface ILangfuseClient
{
    /// <summary>
    ///     Ingests the specified ingestion event
    /// </summary>
    /// <param name="ingestionEvent">Langfuse event</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("This method uses the legacy ingestion endpoint. On Langfuse Cloud, from November 16, 2026 it accepts only score-create events and rejects all other event types (self-hosted deployments behave the same once v4-only write mode is enabled). Send traces and observations via the OpenTelemetry endpoint instead: https://langfuse.com/integrations/native/opentelemetry. Write scores via CreateScoreAsync (POST /api/public/scores).")]
    Task IngestAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Ingests the specified whole langfuse trace
    /// </summary>
    /// <param name="langfuseTrace">Langfuse trace with events, spans, generations</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Obsolete("This method uses the legacy ingestion endpoint. On Langfuse Cloud, from November 16, 2026 it accepts only score-create events and rejects all other event types (self-hosted deployments behave the same once v4-only write mode is enabled). Send traces and observations via the OpenTelemetry endpoint instead: https://langfuse.com/integrations/native/opentelemetry. Write scores via CreateScoreAsync (POST /api/public/scores).")]
    Task IngestAsync(LangfuseTrace langfuseTrace, CancellationToken cancellationToken = default);
}