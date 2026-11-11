using System.Diagnostics;

namespace zborek.Langfuse.OpenTelemetry.Trace;

/// <summary>
///     Represents a retriever observation (RAG, vector store, or database lookup).
/// </summary>
public class OtelRetriever : OtelObservation
{
    internal OtelRetriever(Activity? activity) : base(activity)
    {
    }

    /// <summary>
    ///     Sets the query text used for retrieval.
    /// </summary>
    public void SetQuery(string queryText)
    {
        Activity?.SetTag(GenAiAttributes.RetrievalQueryText, queryText);
    }

    /// <summary>
    ///     Sets the maximum number of documents the retriever was asked to return.
    /// </summary>
    public void SetTopK(int topK)
    {
        Activity?.SetTag(GenAiAttributes.RetrievalTopK, topK);
    }

    /// <summary>
    ///     Sets the data source (e.g., knowledge base or index) the documents come from.
    /// </summary>
    public void SetDataSource(string dataSourceId)
    {
        GenAiActivityHelper.SetDataSource(Activity, dataSourceId);
    }

    /// <summary>
    ///     Records the retrieved documents. They are also set as the observation output.
    /// </summary>
    public void SetDocuments(object documents)
    {
        if (Activity == null)
        {
            return;
        }

        var json = GenAiActivityHelper.Serialize(documents);
        Activity.SetTag(GenAiAttributes.RetrievalDocuments, json);
        Activity.SetTag(LangfuseAttributes.ObservationOutput, json);
    }
}
