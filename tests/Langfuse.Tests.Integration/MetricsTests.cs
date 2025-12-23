using System.Text.Json;
using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Metrics;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class MetricsTests
{
    private readonly LangfuseTestFixture _fixture;

    public MetricsTests(LangfuseTestFixture fixture)
    {
        _fixture = fixture;
    }

    private ILangfuseClient CreateClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLangfuse(config =>
        {
            config.Url = _fixture.LangfuseBaseUrl;
            config.PublicKey = _fixture.PublicKey;
            config.SecretKey = _fixture.SecretKey;
            config.BatchMode = false;
        });

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ILangfuseClient>();
    }

    private TraceTestHelper CreateTraceHelper(ILangfuseClient client)
    {
        return new TraceTestHelper(client, _fixture);
    }

    [Fact]
    public async Task GetMetricsAsync_ReturnsTraceMetrics()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // Create some traces to have data
        var traceId = traceHelper.CreateTrace("metrics-test-trace");
        await traceHelper.WaitForTraceAsync(traceId);

        // Build metrics query for traces view with count metric
        var query = new
        {
            view = "traces",
            metrics = new[]
            {
                new { measure = "count", aggregation = "count" }
            },
            fromTimestamp = DateTime.UtcNow.AddDays(-1).ToString("o"),
            toTimestamp = DateTime.UtcNow.AddDays(1).ToString("o")
        };

        var request = new MetricsRequest
        {
            Query = JsonSerializer.Serialize(query)
        };

        // Act
        var response = await client.GetMetricsAsync(request);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetMetricsAsync_ReturnsObservationMetrics()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // Create trace with observations
        var result = traceHelper.CreateComplexTrace();
        await traceHelper.WaitForTraceAsync(result.TraceId);
        await traceHelper.WaitForObservationAsync(result.SpanId);

        // Build metrics query for observations view
        var query = new
        {
            view = "observations",
            metrics = new[]
            {
                new { measure = "count", aggregation = "count" }
            },
            fromTimestamp = DateTime.UtcNow.AddDays(-1).ToString("o"),
            toTimestamp = DateTime.UtcNow.AddDays(1).ToString("o")
        };

        var request = new MetricsRequest
        {
            Query = JsonSerializer.Serialize(query)
        };

        // Act
        var response = await client.GetMetricsAsync(request);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetMetricsAsync_GroupsByDimension()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var userId = $"user-{Guid.NewGuid():N}";
        var traceId = traceHelper.CreateTrace(userId: userId);
        await traceHelper.WaitForTraceAsync(traceId);

        // Build metrics query with grouping by name dimension
        var query = new
        {
            view = "traces",
            dimensions = new[]
            {
                new { field = "name" }
            },
            metrics = new[]
            {
                new { measure = "count", aggregation = "count" }
            },
            fromTimestamp = DateTime.UtcNow.AddDays(-1).ToString("o"),
            toTimestamp = DateTime.UtcNow.AddDays(1).ToString("o")
        };

        var request = new MetricsRequest
        {
            Query = JsonSerializer.Serialize(query)
        };

        // Act
        var response = await client.GetMetricsAsync(request);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetMetricsAsync_FiltersByTimeRange()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var fromTimestamp = DateTime.UtcNow.AddHours(-1);
        var toTimestamp = DateTime.UtcNow.AddHours(1);

        // Build metrics query with specific time range
        var query = new
        {
            view = "traces",
            metrics = new[]
            {
                new { measure = "count", aggregation = "count" }
            },
            fromTimestamp = fromTimestamp.ToString("o"),
            toTimestamp = toTimestamp.ToString("o")
        };

        var request = new MetricsRequest
        {
            Query = JsonSerializer.Serialize(query)
        };

        // Act
        var response = await client.GetMetricsAsync(request);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetMetricsAsync_WithLatencyMetric()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // Create trace with observations that have duration
        var (traceId, spanId) = traceHelper.CreateTraceWithSpan();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        // Build metrics query for latency
        var query = new
        {
            view = "observations",
            metrics = new[]
            {
                new { measure = "latency", aggregation = "avg" }
            },
            fromTimestamp = DateTime.UtcNow.AddDays(-1).ToString("o"),
            toTimestamp = DateTime.UtcNow.AddDays(1).ToString("o")
        };

        var request = new MetricsRequest
        {
            Query = JsonSerializer.Serialize(query)
        };

        // Act
        var response = await client.GetMetricsAsync(request);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetMetricsAsync_WithTimeDimension()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        // Build metrics query with time dimension (hourly grouping)
        var query = new
        {
            view = "traces",
            timeDimension = "hour",
            metrics = new[]
            {
                new { measure = "count", aggregation = "count" }
            },
            fromTimestamp = DateTime.UtcNow.AddDays(-1).ToString("o"),
            toTimestamp = DateTime.UtcNow.AddDays(1).ToString("o")
        };

        var request = new MetricsRequest
        {
            Query = JsonSerializer.Serialize(query)
        };

        // Act
        var response = await client.GetMetricsAsync(request);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetMetricsAsync_WithFilter()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var uniqueName = $"filtered-trace-{Guid.NewGuid():N}";
        var traceId = traceHelper.CreateTrace(uniqueName);
        await traceHelper.WaitForTraceAsync(traceId);

        // Build metrics query with filter
        var query = new
        {
            view = "traces",
            metrics = new[]
            {
                new { measure = "count", aggregation = "count" }
            },
            filter = new[]
            {
                new { column = "name", @operator = "equals", value = uniqueName }
            },
            fromTimestamp = DateTime.UtcNow.AddDays(-1).ToString("o"),
            toTimestamp = DateTime.UtcNow.AddDays(1).ToString("o")
        };

        var request = new MetricsRequest
        {
            Query = JsonSerializer.Serialize(query)
        };

        // Act
        var response = await client.GetMetricsAsync(request);

        // Assert
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetMetricsAsync_WithRowLimit()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        // Create multiple traces
        for (var i = 0; i < 3; i++)
        {
            var traceId = traceHelper.CreateTrace($"limit-test-{i}");
            await traceHelper.WaitForTraceAsync(traceId);
        }

        // Build metrics query with row limit
        var query = new
        {
            view = "traces",
            dimensions = new[]
            {
                new { field = "name" }
            },
            metrics = new[]
            {
                new { measure = "count", aggregation = "count" }
            },
            limit = 10,
            fromTimestamp = DateTime.UtcNow.AddDays(-1).ToString("o"),
            toTimestamp = DateTime.UtcNow.AddDays(1).ToString("o")
        };

        var request = new MetricsRequest
        {
            Query = JsonSerializer.Serialize(query)
        };

        // Act
        var response = await client.GetMetricsAsync(request);

        // Assert
        Assert.NotNull(response);
    }
}