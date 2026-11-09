using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.BlobStorageIntegration;

/// <summary>
///     Defines how Langfuse authenticates against the blob storage.
/// </summary>
[JsonConverter(typeof(SnakeCaseUpperEnumConverter<BlobStorageIntegrationAuthType>))]
public enum BlobStorageIntegrationAuthType
{
    /// <summary>
    ///     Stored access key ID and secret access key
    /// </summary>
    AccessKeys,

    /// <summary>
    ///     Langfuse assumes an IAM role in your AWS account. S3 only, Enterprise plan, and the deployment must configure a
    ///     cross-account role.
    /// </summary>
    RoleArn
}
