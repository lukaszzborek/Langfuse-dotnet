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

    internal async Task<IngestionResponse> IngestInternalAsync(IngestionRequest request)
    {
        // TODO: Batch request by max 3.5MB (limit by api)

        var json = JsonSerializer.Serialize(request, SerializerOptions);
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
}