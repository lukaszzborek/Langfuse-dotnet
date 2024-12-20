using Langfuse.Models;
using Langfuse.Services;

namespace Langfuse.Client;

public interface ILangfuseClient
{
    Task IngestAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken = default);
    Task IngestAsync(LangfuseTrace langfuseTrace, CancellationToken cancellationToken = default);
}