namespace zborek.Langfuse.Config;

/// <summary>
/// Langfuse configuration
/// </summary>
public class LangfuseConfig
{
    /// <summary>
    /// Url for Langufuse API. Default is https://cloud.langfuse.com
    /// </summary>
    public string Url { get; set; } = "https://cloud.langfuse.com";
    
    /// <summary>
    /// Public key for Langfuse API
    /// </summary>
    public string PublicKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Secret key for Langfuse API
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Enable batch mode. When enabled, events will be sent in batches in background.
    /// </summary>
    public bool BatchMode { get; set; } = true;
    
    /// <summary>
    /// Batch wait time. Default is 5 seconds.
    /// </summary>
    public TimeSpan BatchWaitTimeSeconds { get; set; } = TimeSpan.FromSeconds(5);
}