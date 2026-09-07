using System.Text.Json.Serialization;
using zborek.Langfuse.Converters;

namespace zborek.Langfuse.Models.Core;

/// <summary>
///     Machine-readable error codes returned by the stable evaluators and evaluation-rules API.
/// </summary>
[JsonConverter(typeof(SnakeCaseLowerEnumConverter<PublicApiErrorCode>))]
public enum PublicApiErrorCode
{
    /// <summary>
    ///     Authentication credentials were missing or invalid.
    /// </summary>
    AuthenticationFailed,

    /// <summary>
    ///     The authenticated principal is not allowed to perform the requested action.
    /// </summary>
    AccessDenied,

    /// <summary>
    ///     The request was malformed or otherwise invalid.
    /// </summary>
    InvalidRequest,

    /// <summary>
    ///     One or more query parameters were invalid.
    /// </summary>
    InvalidQuery,

    /// <summary>
    ///     The request body was invalid.
    /// </summary>
    InvalidBody,

    /// <summary>
    ///     The requested resource could not be found.
    /// </summary>
    ResourceNotFound,

    /// <summary>
    ///     The request conflicts with the current state of the resource.
    /// </summary>
    Conflict,

    /// <summary>
    ///     The client has exceeded the allowed rate limit.
    /// </summary>
    RateLimited,

    /// <summary>
    ///     The HTTP method used is not allowed for the requested resource.
    /// </summary>
    MethodNotAllowed,

    /// <summary>
    ///     An unexpected internal error occurred while processing the request.
    /// </summary>
    InternalError
}