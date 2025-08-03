using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

/// <summary>
///     Represents a dataset run with its associated items - an experimental execution of a dataset for evaluation purposes.
///     Dataset runs track how your LLM application performs against a specific set of test cases, enabling systematic evaluation and comparison.
/// </summary>
public class DatasetRunWithItems
{
    /// <summary>
    ///     Unique identifier of the dataset run.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the dataset run, used for identification and organization of experiments.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Optional description explaining the purpose and context of this dataset run.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    ///     Additional metadata associated with the dataset run as a JSON object, containing experiment parameters and configuration.
    /// </summary>
    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     ID of the dataset this run is based on, establishing the relationship to the test cases.
    /// </summary>
    [JsonPropertyName("datasetId")]
    public string DatasetId { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the dataset this run is based on, for display and organizational purposes.
    /// </summary>
    [JsonPropertyName("datasetName")]
    public string DatasetName { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the dataset run was first created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the dataset run was last updated or modified.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    /// <summary>
    ///     Collection of run items representing the individual executions for each dataset item in this run.
    /// </summary>
    [JsonPropertyName("datasetRunItems")]
    public List<DatasetRunItem> DatasetRunItems { get; set; } = new();
}

/// <summary>
///     Represents a single execution item within a dataset run - links a dataset item to its corresponding trace/observation execution.
///     Dataset run items connect test cases to their actual execution results for evaluation and scoring.
/// </summary>
public class DatasetRunItem
{
    /// <summary>
    ///     Unique identifier of the dataset run item.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the dataset run this item belongs to, establishing the parent-child relationship.
    /// </summary>
    [JsonPropertyName("datasetRunId")]
    public string DatasetRunId { get; set; } = string.Empty;
    
    /// <summary>
    ///     Name of the dataset run this item belongs to, for display and organizational purposes.
    /// </summary>
    [JsonPropertyName("datasetRunName")]
    public string DatasetRunName { get; set; } = string.Empty;
    
    /// <summary>
    ///     ID of the dataset item (test case) this run item corresponds to, linking input/expected output.
    /// </summary>
    [JsonPropertyName("datasetItemId")]
    public string DatasetItemId { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the trace that was executed for this dataset item, containing the actual application behavior.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string TraceId { get; set; } = string.Empty;

    /// <summary>
    ///     ID of the specific observation within the trace, if the evaluation focuses on a particular operation.
    /// </summary>
    [JsonPropertyName("observationId")]
    public string? ObservationId { get; set; }

    /// <summary>
    ///     Timestamp when the dataset run item was first created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    ///     Timestamp when the dataset run item was last updated or modified.
    /// </summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}