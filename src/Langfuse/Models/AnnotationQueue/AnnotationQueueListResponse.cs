using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Response model for annotation queue list requests
/// </summary>
public class AnnotationQueueListResponse : PaginatedResponse<AnnotationQueueModel>
{
}