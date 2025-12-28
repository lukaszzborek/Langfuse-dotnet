using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Organization;

/// <summary>
///     Response containing organization API keys
/// </summary>
public record OrganizationApiKeysResponse
{
    /// <summary>
    ///     Array of API keys in the organization
    /// </summary>
    [JsonPropertyName("apiKeys")]
    public required OrganizationApiKey[] ApiKeys { get; init; }
}
