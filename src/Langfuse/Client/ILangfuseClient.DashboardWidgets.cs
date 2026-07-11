using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.DashboardWidget;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Creates a reusable dashboard widget
    /// </summary>
    /// <param name="request">Dashboard widget configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created dashboard widget</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>
    ///     This endpoint creates the widget only; placing it on a dashboard grid has to be done in the UI.
    ///     Supported views are observations, scores-numeric, and scores-categorical.
    ///     This is an unstable API surface and may evolve while dashboard/widget APIs are being finalized.
    /// </remarks>
    Task<DashboardWidget> CreateDashboardWidgetAsync(
        CreateDashboardWidgetRequest request,
        CancellationToken cancellationToken = default);
}