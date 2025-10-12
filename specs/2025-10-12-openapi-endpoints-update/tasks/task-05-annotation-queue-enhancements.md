# Task 05: Annotation Queue Enhancements

**Task ID:** openapi-update-05
**Priority:** Medium
**Dependencies:** None
**Approach:** Implementation

## Objective

Add missing model classes for annotation queue operations and implement three new methods in the existing annotation queue client for creating queues and managing assignments.

## What Needs to Be Done

### Files to Create

Create the following files in `src/Langfuse/Models/AnnotationQueue/`:

1. **CreateAnnotationQueueRequest.cs**
   ```csharp
   /// <summary>
   /// Request to create an annotation queue
   /// </summary>
   public record CreateAnnotationQueueRequest
   {
       /// <summary>
       /// Name of the annotation queue
       /// </summary>
       [JsonPropertyName("name")]
       public required string Name { get; init; }

       /// <summary>
       /// Description of the annotation queue
       /// </summary>
       [JsonPropertyName("description")]
       public string? Description { get; init; }

       /// <summary>
       /// IDs of score configurations associated with this queue
       /// </summary>
       [JsonPropertyName("scoreConfigIds")]
       public required string[] ScoreConfigIds { get; init; }
   }
   ```

2. **AnnotationQueueAssignmentRequest.cs**
   ```csharp
   /// <summary>
   /// Request to create or delete a user assignment to an annotation queue
   /// </summary>
   public record AnnotationQueueAssignmentRequest
   {
       /// <summary>
       /// User ID to assign or unassign
       /// </summary>
       [JsonPropertyName("userId")]
       public required string UserId { get; init; }
   }
   ```

3. **CreateAnnotationQueueAssignmentResponse.cs**
   ```csharp
   /// <summary>
   /// Response after creating an annotation queue assignment
   /// </summary>
   public record CreateAnnotationQueueAssignmentResponse
   {
       /// <summary>
       /// User ID
       /// </summary>
       [JsonPropertyName("userId")]
       public required string UserId { get; init; }

       /// <summary>
       /// Queue ID
       /// </summary>
       [JsonPropertyName("queueId")]
       public required string QueueId { get; init; }

       /// <summary>
       /// Project ID
       /// </summary>
       [JsonPropertyName("projectId")]
       public required string ProjectId { get; init; }
   }
   ```

4. **DeleteAnnotationQueueAssignmentResponse.cs**
   ```csharp
   /// <summary>
   /// Response after deleting an annotation queue assignment
   /// </summary>
   public record DeleteAnnotationQueueAssignmentResponse
   {
       /// <summary>
       /// Success indicator
       /// </summary>
       [JsonPropertyName("success")]
       public required bool Success { get; init; }
   }
   ```

### Files to Modify

**src/Langfuse/Client/LangfuseClient.AnnotationQueue.cs**

Add three new methods to the existing partial class:

```csharp
/// <summary>
/// Create an annotation queue
/// </summary>
/// <param name="request">Annotation queue details</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Created annotation queue</returns>
public async Task<AnnotationQueue> CreateAnnotationQueueAsync(
    CreateAnnotationQueueRequest request,
    CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(request);

    var response = await _httpClient.PostAsJsonAsync(
        "annotation-queues",
        request,
        _jsonOptions,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<AnnotationQueue>(
        _jsonOptions,
        cancellationToken);

    return result ?? throw new LangfuseApiException("Failed to deserialize response");
}

/// <summary>
/// Create an assignment for a user to an annotation queue
/// </summary>
/// <param name="queueId">The unique identifier of the annotation queue</param>
/// <param name="request">Assignment details</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Created assignment</returns>
public async Task<CreateAnnotationQueueAssignmentResponse> CreateQueueAssignmentAsync(
    string queueId,
    AnnotationQueueAssignmentRequest request,
    CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(queueId);
    ArgumentNullException.ThrowIfNull(request);

    var response = await _httpClient.PostAsJsonAsync(
        $"annotation-queues/{Uri.EscapeDataString(queueId)}/assignments",
        request,
        _jsonOptions,
        cancellationToken);

    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<CreateAnnotationQueueAssignmentResponse>(
        _jsonOptions,
        cancellationToken);

    return result ?? throw new LangfuseApiException("Failed to deserialize response");
}

/// <summary>
/// Delete an assignment for a user from an annotation queue
/// </summary>
/// <param name="queueId">The unique identifier of the annotation queue</param>
/// <param name="request">Assignment details</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Deletion result</returns>
public async Task<DeleteAnnotationQueueAssignmentResponse> DeleteQueueAssignmentAsync(
    string queueId,
    AnnotationQueueAssignmentRequest request,
    CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(queueId);
    ArgumentNullException.ThrowIfNull(request);

    var httpRequest = new HttpRequestMessage(HttpMethod.Delete,
        $"annotation-queues/{Uri.EscapeDataString(queueId)}/assignments")
    {
        Content = JsonContent.Create(request, options: _jsonOptions)
    };

    var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<DeleteAnnotationQueueAssignmentResponse>(
        _jsonOptions,
        cancellationToken);

    return result ?? throw new LangfuseApiException("Failed to deserialize response");
}
```

### Key Implementation Points

1. **POST vs DELETE with Body:** DELETE with body requires creating `HttpRequestMessage` manually
2. **Path Parameters:** URL encode queueId using `Uri.EscapeDataString()`
3. **Validation:** Check both queueId and request object
4. **Existing Pattern:** Follow the style of existing methods in the file

### Testing Requirements

Add tests to existing file or create new: `tests/Langfuse.Tests/Client/LangfuseClientAnnotationQueueTests.cs`

Tests to write:
1. **CreateAnnotationQueueAsync:**
   - Test successful creation
   - Test request serialization
   - Test response deserialization
   - Test throws on null request

2. **CreateQueueAssignmentAsync:**
   - Test successful assignment creation
   - Test queueId is URL encoded
   - Test throws on null/empty queueId
   - Test throws on null request

3. **DeleteQueueAssignmentAsync:**
   - Test successful deletion with DELETE + body
   - Test request body is sent with DELETE
   - Test queueId is URL encoded
   - Test response indicates success

4. **Model Tests:**
   - Test CreateAnnotationQueueRequest serialization
   - Test AnnotationQueueAssignmentRequest serialization
   - Test response deserialization

## Acceptance Criteria

- [ ] All 4 model files created in `src/Langfuse/Models/AnnotationQueue/`
- [ ] All models use proper JSON serialization attributes
- [ ] All models have XML documentation
- [ ] Three new methods added to `LangfuseClient.AnnotationQueue.cs`
- [ ] Methods follow existing patterns in the file
- [ ] DELETE with body implemented correctly using HttpRequestMessage
- [ ] Path parameters properly URL encoded
- [ ] All parameters validated
- [ ] XML documentation on all methods
- [ ] Tests added for new methods
- [ ] Tests added for new models
- [ ] All tests pass
- [ ] Code builds without warnings
