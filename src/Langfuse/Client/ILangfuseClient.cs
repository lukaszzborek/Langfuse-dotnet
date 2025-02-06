using zborek.Langfuse.Models;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

/// <summary>
/// Represents a client for interacting with Langfuse api
/// </summary>
public interface ILangfuseClient
{
    /// <summary>
    /// Ingests the specified ingestion event
    /// </summary>
    /// <param name="ingestionEvent">Langfuse event</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task IngestAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Ingests the specified whole langfuse trace
    /// </summary>
    /// <param name="langfuseTrace">Langfuse trace with events, spans, generations</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task IngestAsync(LangfuseTrace langfuseTrace, CancellationToken cancellationToken = default);
}