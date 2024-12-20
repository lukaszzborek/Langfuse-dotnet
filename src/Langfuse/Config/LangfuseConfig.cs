namespace Langfuse.Config;

public class LangfuseConfig
{
    public string Url { get; set; } = "https://cloud.langfuse.com";
    public string PublicKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool BatchMode { get; set; } = true;
    public int BatchWaitTimeSeconds { get; set; } = 5;
}