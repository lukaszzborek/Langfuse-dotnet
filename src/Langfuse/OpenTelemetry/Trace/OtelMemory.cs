using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents a memory operation (search, create, update, upsert, or delete memory records).
/// </summary>
public class OtelMemory : OtelObservation
{
    internal OtelMemory(Activity? activity) : base(activity)
    {
    }

    /// <summary>
    ///     Sets the search query used to retrieve memories.
    /// </summary>
    public void SetQuery(string queryText)
    {
        Activity?.SetTag(GenAiAttributes.MemoryQueryText, queryText);
    }

    /// <summary>
    ///     Sets the identifier of the memory record.
    /// </summary>
    public void SetRecordId(string recordId)
    {
        Activity?.SetTag(GenAiAttributes.MemoryRecordId, recordId);
    }

    /// <summary>
    ///     Sets the number of memory records relevant to the operation.
    /// </summary>
    public void SetRecordCount(int recordCount)
    {
        Activity?.SetTag(GenAiAttributes.MemoryRecordCount, recordCount);
    }

    /// <summary>
    ///     Records the memory records stored or retrieved. They are also set as the observation output.
    /// </summary>
    public void SetRecords(object records)
    {
        if (Activity == null)
        {
            return;
        }

        var json = GenAiActivityHelper.Serialize(records);
        Activity.SetTag(GenAiAttributes.MemoryRecords, json);
        Activity.SetTag(LangfuseAttributes.ObservationOutput, json);
    }
}
