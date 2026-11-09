using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.ObservationV2;

/// <summary>
///     Response containing observations with field-group-based filtering and cursor-based pagination.
/// </summary>
/// <remarks>
///     The data array contains observation objects with only the requested field groups included.
///     Use the cursor in meta to retrieve the next page of results.
///     A page may hold fewer items than the requested limit, or none, while a cursor is present.
///     Continue until the cursor is absent.
/// </remarks>
public class ObservationsV2Response
{
    /// <summary>
    ///     Array of observation objects. Fields included depend on the fields parameter in the request.
    /// </summary>
    [JsonPropertyName("data")]
    public List<Dictionary<string, object?>> Data { get; set; } = [];

    /// <summary>
    ///     Metadata for cursor-based pagination.
    /// </summary>
    [JsonPropertyName("meta")]
    public ObservationsV2Meta Meta { get; set; } = new();
}

/// <summary>
///     Metadata for cursor-based pagination.
/// </summary>
public class ObservationsV2Meta
{
    /// <summary>
    ///     Base64-encoded cursor to use for retrieving the next page.
    ///     If not present (null), there are no more results. An empty Data list does not mean the end.
    /// </summary>
    [JsonPropertyName("cursor")]
    public string? Cursor { get; set; }

    /// <summary>
    ///     Present when an empty page has a cursor. Continue requesting pages until the cursor is absent.
    /// </summary>
    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}
