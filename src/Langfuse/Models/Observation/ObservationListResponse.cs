using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.Observation;

/// <summary>
///     Response containing a paginated list of observations
/// </summary>
public class ObservationListResponse : PaginatedResponse<ObservationModel>
{
}