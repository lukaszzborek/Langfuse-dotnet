namespace zborek.Langfuse.Config;

/// <summary>
///     Langfuse configuration
/// </summary>
public class LangfuseConfig
{
    /// <summary>
    ///     Url for Langufuse API. Default is https://cloud.langfuse.com
    /// </summary>
    public string Url { get; set; } = "https://cloud.langfuse.com";

    /// <summary>
    ///     Public key for Langfuse API
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    ///     Secret key for Langfuse API
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    ///     Enable batch mode. When enabled, events will be sent in batches in background.
    /// </summary>
    public bool BatchMode { get; set; } = true;

    /// <summary>
    ///     Batch wait time. Default is 5 seconds.
    /// </summary>
    public TimeSpan BatchWaitTime { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    ///     Default timeout for HTTP requests to Langfuse API. Default is 30 seconds.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    ///     Default page size for paginated requests. Default is 50.
    /// </summary>
    public int DefaultPageSize { get; set; } = 50;

    /// <summary>
    ///     Enable automatic retry for failed requests. Default is true.
    /// </summary>
    public bool EnableRetry { get; set; } = true;

    /// <summary>
    ///     Maximum number of retry attempts for failed requests. Default is 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    ///     Base delay between retry attempts. Default is 1 second.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
}