using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Feedback;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Submits explicit user-approved feedback about Langfuse skills, MCP tools, CLI, docs, or public API
    /// </summary>
    /// <param name="request">Feedback details including target type, target identifier, and feedback text</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Response containing a correlation ID for the submitted feedback</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>
    ///     Do not include secrets, credentials, customer data, trace payloads, or unrelated use-case details.
    /// </remarks>
    Task<SubmitFeedbackResponse> SubmitFeedbackAsync(SubmitFeedbackRequest request,
        CancellationToken cancellationToken = default);
}
