using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Score;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a cursor-paginated list of scores from the v3 scores API with a polymorphic value field
    /// </summary>
    /// <param name="request">
    ///     Filter and pagination parameters including comma-separated ID, name, source, data type, environment,
    ///     config, queue, author, subject (trace/session/observation/experiment) and value filters, plus the
    ///     field groups to include (details, subject, annotation)
    /// </param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    ///     Cursor-paginated list of scores. Each score is a concrete <see cref="ScoreV3" /> subtype depending on
    ///     its data type: <see cref="NumericScoreV3" />, <see cref="BooleanScoreV3" />,
    ///     <see cref="CategoricalScoreV3" />, <see cref="TextScoreV3" /> or <see cref="CorrectionScoreV3" />
    /// </returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    /// <remarks>
    ///     Use <see cref="GetScoresV3Meta.Cursor" /> from the response as <see cref="ScoreV3ListRequest.Cursor" />
    ///     of the next request to fetch the next page; it is absent on the final page.
    /// </remarks>
    Task<GetScoresV3Response> GetScoresV3Async(ScoreV3ListRequest? request = null,
        CancellationToken cancellationToken = default);
}