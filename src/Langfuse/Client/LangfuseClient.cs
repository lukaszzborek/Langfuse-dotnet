using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.Extensions.Options;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal class LangfuseClient : ILangfuseClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.KebabCaseUpper) }
    };

    private readonly Channel<IIngestionEvent> _channel;
    private readonly IOptions<LangfuseConfig> _config;
    private readonly HttpClient _httpClient;

    public LangfuseClient(HttpClient httpClient, Channel<IIngestionEvent> channel, IOptions<LangfuseConfig> config)
    {
        _httpClient = httpClient;
        _channel = channel;
        _config = config;
    }

    public async Task IngestAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken = default)
    {
        await IngestInternalAsync(ingestionEvent, cancellationToken);
    }

    public async Task IngestAsync(LangfuseTrace langfuseTrace, CancellationToken cancellationToken = default)
    {
        var events = langfuseTrace.GetEvents();
        foreach (var @event in events)
        {
            await IngestAsync(@event, cancellationToken);
        }
    }

    private async Task IngestInternalAsync(IIngestionEvent ingestionEvent, CancellationToken cancellationToken)
    {
        if (_config.Value.BatchMode)
        {
            await _channel.Writer.WriteAsync(ingestionEvent, cancellationToken);
        }
        else
        {
            var ingestionRequest = new IngestionRequest
            {
                Batch = [ingestionEvent]
            };
            await IngestInternalAsync(ingestionRequest);
        }
    }

    // TODO: For optimalization
    internal async Task<IngestionResponse> IngestInternalAsync(IngestionRequest request)
    {
        const int maxBatchSizeBytes = 3_500_000; // 3.5MB in bytes

        // First check if the entire request is under the limit
        var json = JsonSerializer.Serialize(request, SerializerOptions);
        if (Encoding.UTF8.GetByteCount(json) <= maxBatchSizeBytes)
        {
            // If under limit, process normally
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/public/ingestion", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to ingest event");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var ingestionResponse = JsonSerializer.Deserialize<IngestionResponse>(responseJson);

            if (ingestionResponse == null)
            {
                throw new Exception("Failed to parse ingestion response");
            }

            return ingestionResponse;
        }

        // If over limit, split into smaller batches
        var allEvents = request.Batch.ToList();
        var currentBatch = new List<object>();

        // save all results
        var successResponses = new List<IngestionSuccessResponse>();
        var errorResponses = new List<IngestionErrorResponse>();

        foreach (var eventItem in allEvents)
        {
            currentBatch.Add(eventItem);

            // Check if adding this event would exceed the size limit
            var batchRequest = new IngestionRequest { Batch = currentBatch.ToArray() };
            var batchJson = JsonSerializer.Serialize(batchRequest, SerializerOptions);

            if (Encoding.UTF8.GetByteCount(batchJson) <= maxBatchSizeBytes)
            {
                continue;
            }

            // Remove the last added event as it caused overflow
            currentBatch.RemoveAt(currentBatch.Count - 1);

            // Process current batch
            if (currentBatch.Count > 0)
            {
                var batchIngestionRequest = new IngestionRequest { Batch = currentBatch.ToArray() };
                var lastResponse = await IngestInternalAsync(batchIngestionRequest);
                successResponses.AddRange(lastResponse?.Successes ?? []);
                errorResponses.AddRange(lastResponse?.Errors ?? []);
            }

            // Start new batch with the overflow event
            currentBatch.Clear();
            currentBatch.Add(eventItem);
        }

        // Process any remaining events in the last batch
        if (currentBatch.Count > 0)
        {
            var finalBatchRequest = new IngestionRequest { Batch = currentBatch.ToArray() };
            var lastResponse = await IngestInternalAsync(finalBatchRequest);
            successResponses.AddRange(lastResponse?.Successes ?? []);
            errorResponses.AddRange(lastResponse?.Errors ?? []);
        }

        return new IngestionResponse
        {
            Successes = successResponses.ToArray(),
            Errors = errorResponses.ToArray()
        } ?? throw new Exception("No events were processed");
    }
}