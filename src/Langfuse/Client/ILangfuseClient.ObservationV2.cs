using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.ObservationV2;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Get a list of observations with cursor-based pagination and flexible field selection (V2 API).
    /// </summary>
    /// <param name="request">
    ///     Request parameters including field groups to include, pagination cursor, limit, and optional filters.
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    ///     Response containing observations with field-group-based filtering and cursor-based pagination.
    ///     Use the cursor in meta to retrieve the next page of results.
    /// </returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>
    ///     <para>Field Groups:</para>
    ///     <list type="bullet">
    ///         <item>core - Always included: id, traceId, startTime, endTime, projectId, parentObservationId, type</item>
    ///         <item>basic - name, level, statusMessage, version, environment, bookmarked, public, userId, sessionId</item>
    ///         <item>time - completionStartTime, createdAt, updatedAt</item>
    ///         <item>io - input, output</item>
    ///         <item>metadata - metadata</item>
    ///         <item>model - providedModelName, internalModelId, modelParameters</item>
    ///         <item>usage - usageDetails, costDetails, totalCost</item>
    ///         <item>prompt - promptId, promptName, promptVersion</item>
    ///         <item>metrics - latency, timeToFirstToken</item>
    ///     </list>
    ///     <para>If not specified, core and basic field groups are returned.</para>
    /// </remarks>
    Task<ObservationsV2Response> GetObservationsV2Async(ObservationsV2Request? request = null,
        CancellationToken cancellationToken = default);
}