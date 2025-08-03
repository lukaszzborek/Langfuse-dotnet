using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Score;

/// <summary>
///     Response containing a paginated list of scores
/// </summary>
public class ScoreListResponse : PaginatedResponse<ScoreModel>
{
}