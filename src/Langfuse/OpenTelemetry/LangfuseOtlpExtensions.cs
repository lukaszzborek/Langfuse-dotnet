using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Trace;

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

        return builder.AddProcessor(new BatchActivityExportProcessor(exporter));
    }

    private static OtlpTraceExporter CreateOtlpExporter(LangfuseOtlpExporterOptions langfuseOptions)
    {
        if (string.IsNullOrEmpty(langfuseOptions.BaseAddress))
        {
            throw new ArgumentException("Langfuse Endpoint must be provided in LangfuseOtlpExporterOptions");
        }

        if (string.IsNullOrEmpty(langfuseOptions.PublicKey))
        {
            throw new ArgumentException("Langfuse Public Key must be provided in LangfuseOtlpExporterOptions");
        }

        var otlpOptions = new OtlpExporterOptions();

        var endpoint = langfuseOptions.BaseAddress.TrimEnd('/');
        otlpOptions.Endpoint = new Uri($"{endpoint}/{langfuseOptions.OpenTelemetryEndpoint}");
        otlpOptions.Protocol = OtlpExportProtocol.HttpProtobuf;
        otlpOptions.TimeoutMilliseconds = langfuseOptions.TimeoutMilliseconds;

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{langfuseOptions.PublicKey}:{langfuseOptions.SecretKey}")
        );
        otlpOptions.Headers = $"Authorization=Basic {credentials}";

        if (langfuseOptions.Headers.Count > 0)
        {
            var headerStrings = langfuseOptions.Headers
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

        foreach (var activity in filteredActivities)
        {
            var singleBatch = new Batch<Activity>([activity], 1);
            var result = _innerExporter.Export(singleBatch);
            if (result != ExportResult.Success)
            {
                return result;
            }
        }

        return ExportResult.Success;
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
        foreach (var tag in activity.Tags)
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
