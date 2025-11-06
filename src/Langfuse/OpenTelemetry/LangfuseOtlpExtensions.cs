using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;
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

        return builder.AddOtlpExporter(otlpOptions =>
        {
            ConfigureOtlpOptions(otlpOptions, langfuseOptions);
        });
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
        
        return builder.AddOtlpExporter(otlpOptions =>
        {
            ConfigureOtlpOptions(otlpOptions, langfuseOptions);
        });
    }

    private static void ConfigureOtlpOptions(OtlpExporterOptions otlpOptions, LangfuseOtlpExporterOptions langfuseOptions)
    {
        if(string.IsNullOrEmpty(langfuseOptions.BaseAddress))
        {
            throw new ArgumentException("Langfuse Endpoint must be provided in LangfuseOtlpExporterOptions");
        }

        if (string.IsNullOrEmpty(langfuseOptions.PublicKey))
        {
            throw new ArgumentException("Langfuse Public Key must be provided in LangfuseOtlpExporterOptions");
        }
        
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
    }
}
