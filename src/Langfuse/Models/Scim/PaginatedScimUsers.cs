using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Paginated response containing SCIM users.
/// </summary>
public class PaginatedScimUsers
{
    /// <summary>
    ///     SCIM schema URIs for this response.
    /// </summary>
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new() { "urn:ietf:params:scim:api:messages:2.0:ListResponse" };

    /// <summary>
    ///     Total number of results available.
    /// </summary>
    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    /// <summary>
    ///     1-based index of the first result returned.
    /// </summary>
    [JsonPropertyName("startIndex")]
    public int StartIndex { get; set; }

    /// <summary>
    ///     Number of items per page.
    /// </summary>
    [JsonPropertyName("itemsPerPage")]
    public int ItemsPerPage { get; set; }

    /// <summary>
    ///     List of SCIM user resources.
    /// </summary>
    [JsonPropertyName("Resources")]
    public List<ScimUser> Resources { get; set; } = new();
}