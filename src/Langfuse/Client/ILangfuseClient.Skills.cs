using zborek.Langfuse.Models.Core;
using zborek.Langfuse.Models.Skill;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    /// <summary>
    ///     Retrieves a paginated list of skills with metadata from their latest version
    /// </summary>
    /// <param name="request">Optional filter and pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of skill metadata, shared tags and the production version</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<SkillMetaListResponse> GetSkillListAsync(SkillListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves a skill's metadata and file manifest by version or label
    /// </summary>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="version">Optional specific version number</param>
    /// <param name="label">Optional label to target a labeled version</param>
    /// <param name="includeContent">Whether to include the UTF-8 text content of each file. Defaults to false.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The resolved skill version</returns>
    /// <exception cref="LangfuseApiException">Thrown when the skill is not found or an API error occurs</exception>
    /// <remarks>
    ///     If neither version nor label is specified, returns the version with the production label. Without
    ///     includeContent, use each file's sha256Hash with <see cref="GetSkillFileContentsAsync" />.
    /// </remarks>
    Task<SkillVersion> GetSkillAsync(string skillName, int? version = null, string? label = null,
        bool? includeContent = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new version of a skill
    /// </summary>
    /// <param name="request">The files and optional commit message of the new version</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created skill version</returns>
    /// <exception cref="LangfuseApiException">Thrown when the skill version creation fails</exception>
    Task<SkillVersion> CreateSkillVersionAsync(CreateSkillVersionRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Replaces the shared tags across all versions of a skill
    /// </summary>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="request">The complete set of tags. An empty list clears all tags.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The latest skill version with the updated tags</returns>
    /// <exception cref="LangfuseApiException">Thrown when the skill is not found or the update fails</exception>
    Task<SkillVersion> UpdateSkillAsync(string skillName, UpdateSkillRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Reads a batch of skill file contents by their hashes
    /// </summary>
    /// <param name="sha256Hashes">One to 50 canonical base64-encoded SHA-256 hashes from skill file manifests</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The text contents for the requested hashes</returns>
    /// <exception cref="LangfuseApiException">Thrown when an API error occurs</exception>
    Task<SkillFileContentsResponse> GetSkillFileContentsAsync(IReadOnlyCollection<string> sha256Hashes,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Replaces the labels on a skill version
    /// </summary>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="version">The version number to label</param>
    /// <param name="request">The labels to set</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated skill version</returns>
    /// <exception cref="LangfuseApiException">Thrown when the skill version is not found or the update fails</exception>
    /// <remarks>The labels move atomically from any other version that has them</remarks>
    Task<SkillVersion> UpdateSkillVersionLabelsAsync(string skillName, int version, UpdateSkillLabelsRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes one immutable skill version
    /// </summary>
    /// <param name="skillName">The name of the skill</param>
    /// <param name="version">The version number to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Deletion confirmation</returns>
    /// <exception cref="LangfuseApiException">Thrown when the skill version is not found or deletion fails</exception>
    Task<DeleteSkillVersionResponse> DeleteSkillVersionAsync(string skillName, int version,
        CancellationToken cancellationToken = default);
}
