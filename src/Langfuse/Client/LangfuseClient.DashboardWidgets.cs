using zborek.Langfuse.Models.DashboardWidget;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<DashboardWidget> CreateDashboardWidgetAsync(
        CreateDashboardWidgetRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await PostAsync<DashboardWidget>("/api/public/unstable/dashboard-widgets", request,
            "Create Dashboard Widget", cancellationToken);
    }
}