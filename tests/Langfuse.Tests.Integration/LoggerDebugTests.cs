using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Shouldly;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Prompt;
using zborek.Langfuse.Models.Score;
using zborek.Langfuse.Models.Trace;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class LoggerDebugTests
{
    private readonly LangfuseTestFixture _fixture;

    public LoggerDebugTests(LangfuseTestFixture fixture)
    {
        _fixture = fixture;
    }

    private (ILangfuseClient Client, FakeLogCollector Collector, ServiceProvider Provider) CreateClientWithFakeLogger(
        LogLevel minLevel = LogLevel.Debug)
    {
        var services = new ServiceCollection();
        services.AddLogging(builder =>
        {
            builder.SetMinimumLevel(minLevel);
            builder.AddFakeLogging();
        });
        services.AddLangfuse(config =>
        {
            config.Url = _fixture.LangfuseBaseUrl;
            config.PublicKey = _fixture.PublicKey;
            config.SecretKey = _fixture.SecretKey;
            config.BatchMode = false;
        });

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<ILangfuseClient>();
        var collector = provider.GetFakeLogCollector();

        return (client, collector, provider);
    }

    private TraceTestHelper CreateTraceHelper(ILangfuseClient client)
    {
        return new TraceTestHelper(client, _fixture);
    }

    [Fact]
    public async Task GetAsync_WithDebugEnabled_LogsStartingAndCompletionMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var health = await client.GetHealthAsync();

            health.ShouldNotBeNull();

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Starting") &&
                l.Message.Contains("Get Health Status") &&
                l.Message.Contains("/api/public/health"));

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Get Health Status") &&
                l.Message.Contains("/api/public/health"));
        }
    }

    [Fact]
    public async Task GetAsync_WithDebugEnabled_LogsResponseData()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            await client.GetHealthAsync();

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            var completionLog = logs.FirstOrDefault(l =>
                l.Level == LogLevel.Debug &&
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Get Health Status"));

            completionLog.ShouldNotBeNull();
            completionLog.Message.ShouldContain("Response:");
        }
    }

    [Fact]
    public async Task GetAsync_WithDebugEnabled_DeserializesResponseCorrectly()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var health = await client.GetHealthAsync();

            health.ShouldNotBeNull();
            health.Status.ShouldBe("OK");
            health.Version.ShouldNotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task GetAsync_WithInfoLevel_DoesNotLogDebugMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger(LogLevel.Information);
        await using (provider)
        {
            await client.GetHealthAsync();

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldBeEmpty();
        }
    }

    [Fact]
    public async Task GetAsync_WithInfoLevel_DeserializesResponseCorrectly()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger(LogLevel.Information);
        await using (provider)
        {
            var health = await client.GetHealthAsync();

            health.ShouldNotBeNull();
            health.Status.ShouldBe("OK");
            health.Version.ShouldNotBeNullOrEmpty();
        }
    }

    [Fact]
    public async Task PostAsync_WithDebugEnabled_LogsStartingAndCompletionMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var traceHelper = CreateTraceHelper(client);
            var traceId = traceHelper.CreateTrace();
            await traceHelper.WaitForTraceAsync(traceId);

            collector.Clear();

            await client.CreateScoreAsync(new ScoreCreateRequest
            {
                TraceId = traceId,
                Name = $"debug-post-{Guid.NewGuid():N}"[..20],
                Value = 0.5,
                DataType = ScoreDataType.Numeric
            });

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Starting") &&
                l.Message.Contains("Create Score") &&
                l.Message.Contains("/api/public/scores"));

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Create Score") &&
                l.Message.Contains("/api/public/scores"));
        }
    }

    [Fact]
    public async Task PostAsync_WithDebugEnabled_LogsRequestData()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var traceHelper = CreateTraceHelper(client);
            var traceId = traceHelper.CreateTrace();
            await traceHelper.WaitForTraceAsync(traceId);

            collector.Clear();

            await client.CreateScoreAsync(new ScoreCreateRequest
            {
                TraceId = traceId,
                Name = $"debug-req-{Guid.NewGuid():N}"[..20],
                Value = 0.7,
                DataType = ScoreDataType.Numeric
            });

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            var startLog = logs.FirstOrDefault(l =>
                l.Level == LogLevel.Debug &&
                l.Message.Contains("Starting") &&
                l.Message.Contains("Create Score"));

            startLog.ShouldNotBeNull();
            startLog.Message.ShouldContain("Request data:");
        }
    }

    [Fact]
    public async Task PostAsync_WithInfoLevel_DoesNotLogDebugMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger(LogLevel.Information);
        await using (provider)
        {
            var traceHelper = CreateTraceHelper(client);
            var traceId = traceHelper.CreateTrace();
            await traceHelper.WaitForTraceAsync(traceId);

            collector.Clear();

            await client.CreateScoreAsync(new ScoreCreateRequest
            {
                TraceId = traceId,
                Name = $"info-post-{Guid.NewGuid():N}"[..20],
                Value = 0.5,
                DataType = ScoreDataType.Numeric
            });

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldBeEmpty();
        }
    }

    [Fact]
    public async Task PatchAsync_WithDebugEnabled_LogsStartingAndCompletionMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            // Create a score config first so we can patch it
            var configName = $"patch-cfg-{Guid.NewGuid():N}"[..20];
            var config = await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
            {
                Name = configName,
                DataType = ScoreConfigDataType.Numeric
            });

            collector.Clear();

            await client.UpdateScoreConfigAsync(config.Id, new UpdateScoreConfigRequest
            {
                IsArchived = true
            });

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Starting") &&
                l.Message.Contains("Update Score Config"));

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Update Score Config"));
        }
    }

    [Fact]
    public async Task PatchAsync_WithDebugEnabled_DeserializesResponseCorrectly()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var configName = $"patch-deser-{Guid.NewGuid():N}"[..20];
            var config = await client.CreateScoreConfigAsync(new CreateScoreConfigRequest
            {
                Name = configName,
                DataType = ScoreConfigDataType.Numeric
            });

            var updated = await client.UpdateScoreConfigAsync(config.Id, new UpdateScoreConfigRequest
            {
                IsArchived = true
            });

            updated.ShouldNotBeNull();
            updated.Id.ShouldBe(config.Id);
            updated.IsArchived.ShouldBeTrue();
        }
    }

    [Fact]
    public async Task DeleteAsyncVoid_WithDebugEnabled_LogsStartingAndCompletionMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var traceHelper = CreateTraceHelper(client);
            var traceId = traceHelper.CreateTrace();
            await traceHelper.WaitForTraceAsync(traceId);

            var score = await client.CreateScoreAsync(new ScoreCreateRequest
            {
                TraceId = traceId,
                Name = $"del-void-{Guid.NewGuid():N}"[..20],
                Value = 0.5,
                DataType = ScoreDataType.Numeric
            });

            collector.Clear();

            await client.DeleteScoreAsync(score.Id);

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Starting") &&
                l.Message.Contains("Delete Score"));

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Delete Score"));
        }
    }

    [Fact]
    public async Task DeleteAsyncWithResponse_WithDebugEnabled_LogsStartingAndCompletionMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var traceHelper = CreateTraceHelper(client);
            var traceId = traceHelper.CreateTrace();
            await traceHelper.WaitForTraceAsync(traceId);

            collector.Clear();

            var result = await client.DeleteTraceAsync(traceId);

            result.ShouldNotBeNull();

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Starting") &&
                l.Message.Contains("Delete Trace"));

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Delete Trace"));
        }
    }

    [Fact]
    public async Task DeleteAsyncWithResponse_WithDebugEnabled_LogsResponseData()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var traceHelper = CreateTraceHelper(client);
            var traceId = traceHelper.CreateTrace();
            await traceHelper.WaitForTraceAsync(traceId);

            collector.Clear();

            await client.DeleteTraceAsync(traceId);

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            var completionLog = logs.FirstOrDefault(l =>
                l.Level == LogLevel.Debug &&
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Delete Trace"));

            completionLog.ShouldNotBeNull();
            completionLog.Message.ShouldContain("Response:");
        }
    }

    [Fact]
    public async Task DeleteWithBodyAsync_WithDebugEnabled_LogsStartingAndCompletionMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var traceHelper = CreateTraceHelper(client);
            var traceId1 = traceHelper.CreateTrace();
            var traceId2 = traceHelper.CreateTrace();
            await traceHelper.WaitForTraceAsync(traceId1);
            await traceHelper.WaitForTraceAsync(traceId2);

            collector.Clear();

            var result = await client.DeleteTraceManyAsync(new DeleteTraceManyRequest
            {
                TraceIds = [traceId1, traceId2]
            });

            result.ShouldNotBeNull();

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Starting") &&
                l.Message.Contains("Delete Multiple Traces"));

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Delete Multiple Traces"));
        }
    }

    [Fact]
    public async Task GetAsync_NotFound_WithDebugEnabled_LogsStartingBeforeError()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var nonExistentId = Guid.NewGuid().ToString();

            var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
                await client.GetScoreAsync(nonExistentId));

            exception.StatusCode.ShouldBe(404);

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Starting") &&
                l.Message.Contains("Get Score"));
        }
    }

    [Fact]
    public async Task GetAsync_Cancelled_LogsWarning()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            await Should.ThrowAsync<TaskCanceledException>(async () =>
                await client.GetHealthAsync(cts.Token));

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> warningLogs = logs.Where(l => l.Level == LogLevel.Warning).ToList();

            warningLogs.ShouldContain(l =>
                l.Message.Contains("cancelled") &&
                l.Message.Contains("Get Health Status"));
        }
    }

    [Fact]
    public async Task PatchAsyncPrompt_WithDebugEnabled_LogsStartingAndCompletionMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var promptName = $"patch-prompt-{Guid.NewGuid():N}"[..20];
            var created = await client.CreatePromptAsync(new CreateTextPromptRequest
            {
                Name = promptName,
                Prompt = "Hello {{name}}"
            });

            collector.Clear();

            var updated = await client.UpdatePromptVersionAsync(promptName, created.Version,
                new UpdatePromptVersionRequest
                {
                    NewLabels = ["staging"]
                });

            updated.ShouldNotBeNull();

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Starting") &&
                l.Message.Contains("Update Prompt Version"));

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Update Prompt Version"));
        }
    }

    [Fact]
    public async Task DeleteAsyncVoidPrompt_WithDebugEnabled_LogsStartingAndCompletionMessages()
    {
        var (client, collector, provider) = CreateClientWithFakeLogger();
        await using (provider)
        {
            var promptName = $"del-prompt-{Guid.NewGuid():N}"[..20];
            await client.CreatePromptAsync(new CreateTextPromptRequest
            {
                Name = promptName,
                Prompt = "Test prompt to delete"
            });

            collector.Clear();

            await client.DeletePromptAsync(promptName, 1);

            IReadOnlyList<FakeLogRecord> logs = collector.GetSnapshot();
            List<FakeLogRecord> debugLogs = logs.Where(l => l.Level == LogLevel.Debug).ToList();

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Starting") &&
                l.Message.Contains("Delete Prompt"));

            debugLogs.ShouldContain(l =>
                l.Message.Contains("Successfully completed") &&
                l.Message.Contains("Delete Prompt"));
        }
    }
}
