using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;
using zborek.Langfuse.OpenTelemetry.Trace;

namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     Extension methods for configuring Langfuse OTLP exporter
/// </summary>
public static class LangfuseOtlpExtensions
{
    private static bool _activityListenerRegistered;
    private static readonly object _listenerLock = new();

    /// <summary>
    ///     Adds Langfuse OTLP exporter to the tracing pipeline
    /// </summary>
    /// <param name="builder">The TracerProviderBuilder to configure</param>
    /// <param name="configure">Action to configure LangfuseOtlpExporterOptions</param>
    /// <returns>The TracerProviderBuilder for method chaining</returns>
    public static TracerProviderBuilder AddLangfuseExporter(
        this TracerProviderBuilder builder,
        Action<LangfuseOtlpExporterOptions> configure)
    {
        var langfuseOptions = new LangfuseOtlpExporterOptions();
        configure(langfuseOptions);

        return AddLangfuseExporterInternal(builder, langfuseOptions);
    }

    /// <summary>
    ///     Adds Langfuse OTLP exporter to the tracing pipeline using configuration
    /// </summary>
    /// <param name="builder">The TracerProviderBuilder to configure</param>
    /// <param name="configuration">The configuration section containing Langfuse settings</param>
    /// <returns>The TracerProviderBuilder for method chaining</returns>
    public static TracerProviderBuilder AddLangfuseExporter(
        this TracerProviderBuilder builder,
        IConfiguration configuration)
    {
        var langfuseOptions = new LangfuseOtlpExporterOptions();
        configuration.Bind(langfuseOptions);

        return AddLangfuseExporterInternal(builder, langfuseOptions);
    }

    private static TracerProviderBuilder AddLangfuseExporterInternal(
        TracerProviderBuilder builder,
        LangfuseOtlpExporterOptions langfuseOptions)
    {
        // Automatically add the Langfuse ActivitySource (even when disabled, for consistent tracing)
        builder.AddSource(OtelLangfuseTrace.ActivitySourceName);

        if (!langfuseOptions.Enabled)
        {
            return builder;
        }

        var otlpExporter = CreateOtlpExporter(langfuseOptions);

        BaseExporter<Activity> exporter = langfuseOptions.OnlyGenAiActivities || langfuseOptions.ActivityFilter != null
            ? new LangfuseFilteringExporter(otlpExporter, langfuseOptions)
            : otlpExporter;

        return builder.AddProcessor(new BatchActivityExportProcessor(exporter));
    }

    /// <summary>
    ///     Registers Langfuse OpenTelemetry tracing services for dependency injection.
    ///     This includes:
    ///     - IOtelLangfuseTrace (scoped) - for sharing a trace across services within a request
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddLangfuseTracing(this IServiceCollection services)
    {
        // Register the ActivityListener for automatic Baggage propagation
        UseLangfuseActivityListener();

        services.TryAddScoped<IOtelLangfuseTrace, OtelLangfuseTrace>();
        return services;
    }

    /// <summary>
    ///     Registers a no-op implementation of IOtelLangfuseTrace for testing scenarios.
    ///     Use this in tests where you don't need actual Langfuse tracing.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddLangfuseTracingNoOp(this IServiceCollection services)
    {
        services.TryAddScoped<IOtelLangfuseTrace>(_ => NullOtelLangfuseTrace.Instance);
        return services;
    }

    private static OtlpTraceExporter CreateOtlpExporter(LangfuseOtlpExporterOptions langfuseOptions)
    {
        if (string.IsNullOrEmpty(langfuseOptions.Endpoint))
        {
            throw new ArgumentException("Langfuse Endpoint must be provided in LangfuseOtlpExporterOptions");
        }

        if (string.IsNullOrEmpty(langfuseOptions.PublicKey))
        {
            throw new ArgumentException("Langfuse Public Key must be provided in LangfuseOtlpExporterOptions");
        }

        if (string.IsNullOrEmpty(langfuseOptions.SecretKey))
        {
            throw new ArgumentException("Langfuse Secret Key must be provided in LangfuseOtlpExporterOptions");
        }

        var otlpOptions = new OtlpExporterOptions();

        var endpoint = langfuseOptions.Endpoint.TrimEnd('/');
        otlpOptions.Endpoint = new Uri($"{endpoint}/{langfuseOptions.OpenTelemetryEndpoint}");
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
        otlpOptions.TimeoutMilliseconds = langfuseOptions.TimeoutMilliseconds;

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{langfuseOptions.PublicKey}:{langfuseOptions.SecretKey}")
        );
        otlpOptions.Headers = $"Authorization=Basic {credentials}";

        if (langfuseOptions.Headers.Count > 0)
        {
            IEnumerable<string> headerStrings = langfuseOptions.Headers
                .Select(kvp => $"{kvp.Key}={kvp.Value}");

            otlpOptions.Headers = $"{otlpOptions.Headers},{string.Join(",", headerStrings)}";
        }

        return new OtlpTraceExporter(otlpOptions);
    }

    /// <summary>
    ///     Registers the Langfuse ActivityListener for automatic context enrichment from Baggage.
    ///     Call this once at application startup. Safe to call multiple times - will only register once.
    /// </summary>
    public static void UseLangfuseActivityListener()
    {
        if (_activityListenerRegistered)
        {
            return;
        }

        lock (_listenerLock)
        {
            if (_activityListenerRegistered)
            {
                return;
            }

            ActivitySource.AddActivityListener(new ActivityListener
            {
                ShouldListenTo = source => source.Name == OtelLangfuseTrace.ActivitySourceName,
                Sample = (ref ActivityCreationOptions<ActivityContext> options) =>
                    options.Source.Name == OtelLangfuseTrace.ActivitySourceName
                        ? ActivitySamplingResult.AllDataAndRecorded
                        : ActivitySamplingResult.None,
                ActivityStarted = activity =>
                {
                    if (activity == null)
                    {
                        return;
                    }

                    // Auto-enrich from Baggage
                    var userId = Baggage.GetBaggage(LangfuseAttributes.UserId);
                    if (!string.IsNullOrEmpty(userId))
                    {
                        activity.SetTag(LangfuseAttributes.UserId, userId);
                    }

                    var sessionId = Baggage.GetBaggage(LangfuseAttributes.SessionId);
                    if (!string.IsNullOrEmpty(sessionId))
                    {
                        activity.SetTag(LangfuseAttributes.SessionId, sessionId);
                    }

                    var version = Baggage.GetBaggage(LangfuseAttributes.Version);
                    if (!string.IsNullOrEmpty(version))
                    {
                        activity.SetTag(LangfuseAttributes.Version, version);
                    }

                    var release = Baggage.GetBaggage(LangfuseAttributes.Release);
                    if (!string.IsNullOrEmpty(release))
                    {
                        activity.SetTag(LangfuseAttributes.Release, release);
                    }

                    var tags = Baggage.GetBaggage(LangfuseAttributes.TraceTags);
                    if (!string.IsNullOrEmpty(tags))
                    {
                        // Serialize as JSON array to match GenAiActivityHelper format
                        List<string> tagList = tags.Split(',').ToList();
                        activity.SetTag(LangfuseAttributes.TraceTags, JsonSerializer.Serialize(tagList));
                    }
                }
            });

            _activityListenerRegistered = true;
        }
    }
}