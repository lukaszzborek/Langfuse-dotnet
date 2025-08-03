using zborek.Langfuse.Models.Core;

namespace zborek.Langfuse.Models.AnnotationQueue;

/// <summary>
///     Response model for annotation queue item list requests
/// </summary>
public class AnnotationQueueItemListResponse : PaginatedResponse<AnnotationQueueItem>
{
}