namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     Well-known values for the <c>gen_ai.operation.name</c> attribute.
///     Based on: https://github.com/open-telemetry/semantic-conventions-genai
/// </summary>
public static class GenAiOperationNames
{
    /// <summary>Chat completion operation.</summary>
    public const string Chat = "chat";

    /// <summary>Multimodal content generation operation (e.g., Gemini generateContent).</summary>
    public const string GenerateContent = "generate_content";

    /// <summary>Realtime, bidirectional streaming inference.</summary>
    public const string RealtimeInference = "realtime_inference";

    /// <summary>User speech turn in a realtime session.</summary>
    public const string UserSpeech = "user_speech";

    /// <summary>Text completion operation.</summary>
    public const string TextCompletion = "text_completion";

    /// <summary>Embeddings operation.</summary>
    public const string Embeddings = "embeddings";

    /// <summary>Retrieval of documents (RAG, vector search).</summary>
    public const string Retrieval = "retrieval";

    /// <summary>Fetching a previously started (background) response.</summary>
    public const string FetchResponse = "fetch_response";

    /// <summary>Agent creation.</summary>
    public const string CreateAgent = "create_agent";

    /// <summary>Agent invocation.</summary>
    public const string InvokeAgent = "invoke_agent";

    /// <summary>Tool execution.</summary>
    public const string ExecuteTool = "execute_tool";

    /// <summary>Workflow invocation. The spec merged it into <see cref="InvokeAgent" />; kept for older backends.</summary>
    public const string InvokeWorkflow = "invoke_workflow";

    /// <summary>Agent runner invocation.</summary>
    public const string InvokeRunner = "invoke_runner";

    /// <summary>Agent planning step.</summary>
    public const string Plan = "plan";

    /// <summary>Search in a memory store.</summary>
    public const string SearchMemory = "search_memory";

    /// <summary>Create a memory record.</summary>
    public const string CreateMemory = "create_memory";

    /// <summary>Update a memory record.</summary>
    public const string UpdateMemory = "update_memory";

    /// <summary>Create or update a memory record.</summary>
    public const string UpsertMemory = "upsert_memory";

    /// <summary>Delete a memory record.</summary>
    public const string DeleteMemory = "delete_memory";

    /// <summary>Create a memory store.</summary>
    public const string CreateMemoryStore = "create_memory_store";

    /// <summary>Delete a memory store.</summary>
    public const string DeleteMemoryStore = "delete_memory_store";
}
