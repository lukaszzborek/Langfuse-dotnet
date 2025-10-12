# Task 06: Organization Management Enhancements

**Task ID:** openapi-update-06
**Priority:** Medium
**Dependencies:** None
**Approach:** Implementation

## Objective

Add model classes for membership deletion and implement two new methods in the organization client for deleting organization and project memberships.

## What Needs to Be Done

### Files to Create

Create the following files in `src/Langfuse/Models/Membership/`:

1. **DeleteMembershipRequest.cs**
   ```csharp
   /// <summary>
   /// Request to delete a membership
   /// </summary>
   public record DeleteMembershipRequest
   {
       /// <summary>
       /// User ID of the member to remove
       /// </summary>
       [JsonPropertyName("userId")]
       public required string UserId { get; init; }
   }
   ```

2. **MembershipDeletionResponse.cs**
   ```csharp
   /// <summary>
   /// Response after deleting a membership
   /// </summary>
   public record MembershipDeletionResponse
   {
       /// <summary>
       /// Success or error message
       /// </summary>
       [JsonPropertyName("message")]
       public required string Message { get; init; }

       /// <summary>
       /// User ID of the deleted member
       /// </summary>
       [JsonPropertyName("userId")]
       public required string UserId { get; init; }
   }
   ```

### Files to Modify

**src/Langfuse/Client/LangfuseClient.Organization.cs**

Add two new methods to the existing partial class:

```csharp
/// <summary>
/// Delete a membership from the organization associated with the API key.
/// Requires organization-scoped API key.
/// </summary>
/// <param name="request">Membership deletion details</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Deletion confirmation</returns>
public async Task<MembershipDeletionResponse> DeleteOrganizationMembershipAsync(
    DeleteMembershipRequest request,
    CancellationToken cancellationToken = default)
{
    ArgumentNullException.ThrowIfNull(request);

    var httpRequest = new HttpRequestMessage(HttpMethod.Delete, "organizations/memberships")
    {
        Content = JsonContent.Create(request, options: _jsonOptions)
    };

    var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<MembershipDeletionResponse>(
        _jsonOptions,
        cancellationToken);

    return result ?? throw new LangfuseApiException("Failed to deserialize response");
}

/// <summary>
/// Delete a membership from a specific project.
/// The user must be a member of the organization.
/// Requires organization-scoped API key.
/// </summary>
/// <param name="projectId">The unique identifier of the project</param>
/// <param name="request">Membership deletion details</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>Deletion confirmation</returns>
public async Task<MembershipDeletionResponse> DeleteProjectMembershipAsync(
    string projectId,
    DeleteMembershipRequest request,
    CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrWhiteSpace(projectId);
    ArgumentNullException.ThrowIfNull(request);

    var httpRequest = new HttpRequestMessage(HttpMethod.Delete,
        $"projects/{Uri.EscapeDataString(projectId)}/memberships")
    {
        Content = JsonContent.Create(request, options: _jsonOptions)
    };

    var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<MembershipDeletionResponse>(
        _jsonOptions,
        cancellationToken);

    return result ?? throw new LangfuseApiException("Failed to deserialize response");
}
```

### Key Implementation Points

1. **DELETE with Body:** Both methods use DELETE with request body, requiring `HttpRequestMessage`
2. **Organization Scope:** Both require organization-scoped API keys, document in XML comments
3. **Path Parameters:** URL encode projectId in the second method
4. **Validation:** Validate all parameters
5. **Response Handling:** Return deletion confirmation with message and userId

### Testing Requirements

Add tests to existing file or create: `tests/Langfuse.Tests/Client/LangfuseClientOrganizationTests.cs`

Tests to write:
1. **DeleteOrganizationMembershipAsync:**
   - Test successful deletion at organization level
   - Test DELETE method with request body
   - Test request serialization
   - Test response deserialization
   - Test throws on null request
   - Test 403 error with project-scoped key

2. **DeleteProjectMembershipAsync:**
   - Test successful deletion at project level
   - Test projectId is URL encoded
   - Test DELETE method with request body
   - Test request serialization
   - Test response deserialization
   - Test throws on null/empty projectId
   - Test throws on null request
   - Test 403 error with project-scoped key

3. **Model Tests:**
   - Test DeleteMembershipRequest serialization
   - Test MembershipDeletionResponse deserialization
   - Test response message and userId fields

### Edge Cases

1. **Self-Deletion:** Consider testing behavior when deleting own membership
2. **Last Admin:** What happens if deleting the last organization admin?
3. **Invalid User:** Test deletion of non-existent user
4. **Wrong Scope:** Test 403 error with project-scoped API key

## Acceptance Criteria

- [x] Both model files created in `src/Langfuse/Models/Organization/`
- [x] Models use proper JSON serialization attributes
- [x] Models have XML documentation
- [x] Two new methods added to `LangfuseClient.Organization.cs`
- [x] Both methods use DELETE with request body correctly
- [x] Organization-scoped key requirement documented
- [x] Path parameters properly URL encoded
- [x] All parameters validated
- [x] XML documentation on all methods
- [x] Tests added for both methods
- [x] Tests added for models
- [x] Organization vs project scope tested
- [x] Error cases tested (403 with wrong key scope)
- [x] All tests pass
- [x] Code builds without warnings
