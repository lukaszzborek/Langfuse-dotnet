using System.Diagnostics;
using System.Text;
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
        var otlpExporter = CreateOtlpExporter(langfuseOptions);

        BaseExporter<Activity> exporter = langfuseOptions.OnlyGenAiActivities || langfuseOptions.ActivityFilter != null
            ? new LangfuseFilteringExporter(otlpExporter, langfuseOptions)
            : otlpExporter;

        // Automatically add the Langfuse ActivitySource
        builder.AddSource(OtelLangfuseTrace.ActivitySourceName);

        return builder.AddProcessor(new BatchActivityExportProcessor(exporter));
    }

    /// <summary>
    ///     Registers Langfuse OpenTelemetry tracing services for dependency injection.
    ///     This includes:
    ///     - IOtelLangfuseTraceContext (scoped) - for sharing a trace across services within a request
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddLangfuseTracing(this IServiceCollection services)
    {
        // Register the ActivityListener for automatic Baggage propagation
        Extensions.UseLangfuseActivityListener();

        services.TryAddScoped<IOtelLangfuseTraceContext, OtelLangfuseTraceContext>();
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
}

/// <summary>
///     Exporter wrapper that filters activities before passing to the underlying exporter.
///     This allows Langfuse to receive only Gen AI activities while other exporters receive all activities.
/// </summary>
internal class LangfuseFilteringExporter : BaseExporter<Activity>
{
    private static readonly string[] GenAiAttributePrefixes =
    [
        "gen_ai.",
        "langfuse."
    ];

    private readonly BaseExporter<Activity> _innerExporter;
    private readonly LangfuseOtlpExporterOptions _options;

    public LangfuseFilteringExporter(BaseExporter<Activity> innerExporter, LangfuseOtlpExporterOptions options)
    {
        _innerExporter = innerExporter;
        _options = options;
    }

    public override ExportResult Export(in Batch<Activity> batch)
    {
        var filteredActivities = new List<Activity>();

        foreach (var activity in batch)
        {
            if (ShouldExport(activity))
            {
                filteredActivities.Add(activity);
            }
        }

        if (filteredActivities.Count == 0)
        {
            return ExportResult.Success;
        }


        var filteredBatch = new Batch<Activity>(filteredActivities.ToArray(), filteredActivities.Count);
        return _innerExporter.Export(filteredBatch);
    }

    private bool ShouldExport(Activity activity)
    {
        var shouldExport = true;

        if (_options.OnlyGenAiActivities)
        {
            shouldExport = IsGenAiActivity(activity);
        }

        if (shouldExport && _options.ActivityFilter != null)
        {
            shouldExport = _options.ActivityFilter(activity);
        }

        return shouldExport;
    }

    private static bool IsGenAiActivity(Activity activity)
    {
        foreach (KeyValuePair<string, string?> tag in activity.Tags)
        {
            foreach (var prefix in GenAiAttributePrefixes)
            {
                if (tag.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected override bool OnShutdown(int timeoutMilliseconds)
    {
        return _innerExporter.Shutdown(timeoutMilliseconds);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _innerExporter.Dispose();
        }

        base.Dispose(disposing);
    }
}