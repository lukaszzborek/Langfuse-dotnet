namespace zborek.Langfuse.OpenTelemetry.Models;

/// <summary>
///     Langfuse observation level for logging severity.
/// </summary>
public enum LangfuseObservationLevel
{
    /// <summary>
    ///     Debug level for detailed diagnostic information.
    /// </summary>
    Debug,

    /// <summary>
    ///     Default level for normal operations.
    /// </summary>
    Default,

    /// <summary>
    ///     Warning level for potentially problematic situations.
    /// </summary>
    Warning,

    /// <summary>
    ///     Error level for error conditions.
    /// </summary>
    Error
}