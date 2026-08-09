using zborek.Langfuse.Models.Feedback;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<SubmitFeedbackResponse> SubmitFeedbackAsync(SubmitFeedbackRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return await PostAsync<SubmitFeedbackResponse>("/api/public/feedback", request, "Submit Feedback",
            cancellationToken);
    }
}
