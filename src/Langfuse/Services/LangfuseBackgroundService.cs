using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using zborek.Langfuse.Client;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services;

internal class LangfuseBackgroundService : BackgroundService
{
    private readonly Channel<IIngestionEvent> _channel;
    private readonly ILangfuseClient _client;
    private readonly IOptions<LangfuseConfig> _config;
    private readonly ILogger<LangfuseBackgroundService> _logger;
    private readonly PeriodicTimer _timer;

    public LangfuseBackgroundService(Channel<IIngestionEvent> channel, ILangfuseClient client,
        IOptions<LangfuseConfig> config, ILogger<LangfuseBackgroundService> logger)
    {
        _channel = channel;
        _client = client;
        _config = config;
        _logger = logger;
        _timer = new PeriodicTimer(_config.Value.BatchWaitTimeSeconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_config.Value.BatchMode)
        {
            return;
        }

        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                var list = new List<IIngestionEvent>();
                while (_channel.Reader.TryRead(out var item))
                {
                    list.Add(item);
                }

                if (list.Count == 0)
                {
                    continue;
                }

                var ingestionRequest = new IngestionRequest
                {
                    Batch = list.ToArray()
                };

                var response = await ((LangfuseClient)_client).IngestInternalAsync(ingestionRequest);

                if (response.Errors is { Length: 0 })
                {
                    continue;
                }

                foreach (var error in response.Errors)
                {
                    _logger.LogWarning("Failed to send event: Id={Id} Status={Status} Message={Message}",
                        error.Id, error.Status, error.Message);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to send events");
            }
        }
    }
}