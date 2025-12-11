using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Interface for OpenTelemetry-based Langfuse trace management.
///     Enables easy mocking in tests and no-op implementations.
/// </summary>
public interface IOtelLangfuseTrace : IDisposable
{
    /// <summary>
    ///     The root trace activity.
    /// </summary>
    Activity? TraceActivity { get; }

    /// <summary>
    ///     Gets whether a trace is currently active.
    /// </summary>
    bool HasActiveTrace { get; }

    /// <summary>
    ///     Starts the trace with the specified parameters.
    /// </summary>
    /// <param name="traceName">The name of the trace.</param>
    /// <param name="userId">Optional user ID for the trace.</param>
    /// <param name="sessionId">Optional session ID for the trace.</param>
    /// <param name="version">Optional version string.</param>
    /// <param name="release">Optional release string.</param>
    /// <param name="tags">Optional tags for the trace.</param>
    /// <param name="input">Optional input for the trace.</param>
    /// <param name="isRoot">If true, creates a new root trace (new TraceId) ignoring any current activity context.</param>
    /// <returns>This trace instance for fluent API.</returns>
    IOtelLangfuseTrace StartTrace(
        string traceName,
        string? userId = null,
        string? sessionId = null,
        string? version = null,
        string? release = null,
        IEnumerable<string>? tags = null,
        object? input = null,
        bool isRoot = false);

    /// <summary>
    ///     Sets the trace name.
    /// </summary>
    void SetTraceName(string name);

    /// <summary>
    ///     Sets trace-level input.
    /// </summary>
    void SetInput(object input);

    /// <summary>
    ///     Sets trace-level output.
    /// </summary>
    void SetOutput(object output);

    /// <summary>
    ///     Creates a span observation.
    /// </summary>
    /// <param name="name">The span name.</param>
    /// <param name="type">Optional span type (e.g., "workflow", "retrieval").</param>
    /// <param name="description">Optional description.</param>
    /// <param name="input">Optional input data.</param>
    /// <param name="configure">Optional action to configure the span.</param>
    OtelSpan CreateSpan(
        string name,
        string? type = null,
        string? description = null,
        object? input = null,
        Action<OtelSpan>? configure = null);

    /// <summary>
    ///     Creates a generation (LLM call) observation.
    /// </summary>
    /// <param name="name">The generation name.</param>
    /// <param name="model">The model name (e.g., "gpt-4").</param>
    /// <param name="provider">Optional provider name (e.g., "openai").</param>
    /// <param name="input">Optional input (messages or prompt).</param>
    /// <param name="configure">Optional action to configure the generation.</param>
    OtelGeneration CreateGeneration(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelGeneration>? configure = null);

    /// <summary>
    ///     Creates a tool call observation.
    /// </summary>
    /// <param name="name">The observation name.</param>
    /// <param name="toolName">The tool name.</param>
    /// <param name="toolDescription">Optional tool description.</param>
    /// <param name="toolType">Tool type (default: "function").</param>
    /// <param name="input">Optional input arguments.</param>
    /// <param name="configure">Optional action to configure the tool call.</param>
    OtelToolCall CreateToolCall(
        string name,
        string toolName,
        string? toolDescription = null,
        string toolType = "function",
        object? input = null,
        Action<OtelToolCall>? configure = null);

    /// <summary>
    ///     Creates an event observation.
    /// </summary>
    /// <param name="name">The event name.</param>
    /// <param name="input">Optional input data.</param>
    /// <param name="output">Optional output data.</param>
    OtelEvent CreateEvent(string name, object? input = null, object? output = null);

    /// <summary>
    ///     Creates an embedding observation.
    /// </summary>
    /// <param name="name">The embedding name.</param>
    /// <param name="model">The model name.</param>
    /// <param name="provider">Optional provider name.</param>
    /// <param name="input">Optional input text.</param>
    /// <param name="configure">Optional action to configure the embedding.</param>
    OtelEmbedding CreateEmbedding(
        string name,
        string model,
        string? provider = null,
        object? input = null,
        Action<OtelEmbedding>? configure = null);

    /// <summary>
    ///     Creates an agent observation.
    /// </summary>
    /// <param name="name">The agent name.</param>
    /// <param name="agentId">The agent ID.</param>
    /// <param name="description">Optional agent description.</param>
    /// <param name="input">Optional input data.</param>
    /// <param name="configure">Optional action to configure the agent.</param>
    OtelAgent CreateAgent(
        string name,
        string agentId,
        string? description = null,
        object? input = null,
        Action<OtelAgent>? configure = null);
}
