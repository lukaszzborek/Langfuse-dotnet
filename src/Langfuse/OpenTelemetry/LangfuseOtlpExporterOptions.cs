namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     Configuration options for the Langfuse OTLP exporter
/// </summary>
public class LangfuseOtlpExporterOptions
{
    /// <summary>
    ///     Langfuse endpoint URL. Default is https://cloud.langfuse.com
    /// </summary>
    public string BaseAddress { get; set; } = "https://cloud.langfuse.com";

    /// <summary>
    ///     Endpoint for the OpenTelemetry traces. Default is api/public/otel/v1/traces.
    /// </summary>
    public string OpenTelemetryEndpoint { get; set; } = "api/public/otel/v1/traces";

    /// <summary>
    ///     Langfuse public API key
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    ///     Langfuse secret API key
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    ///     Optional custom headers to be sent with OTLP requests
    /// </summary>
    public Dictionary<string, string> Headers { get; set; } = new();

    /// <summary>
    ///     Export timeout in milliseconds. Default is 10000 (10 seconds)
    /// </summary>
    public int TimeoutMilliseconds { get; set; } = 10000;
}
