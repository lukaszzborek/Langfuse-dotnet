using zborek.Langfuse.Models.Skill;
using zborek.Langfuse.Services;

namespace zborek.Langfuse.Client;

internal partial class LangfuseClient
{
    /// <inheritdoc />
    public async Task<SkillMetaListResponse> GetSkillListAsync(SkillListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        var query = QueryStringHelper.BuildQueryString(request);
        var endpoint = $"/api/public/skills{query}";
        return await GetAsync<SkillMetaListResponse>(endpoint, "List Skills", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SkillVersion> GetSkillAsync(string skillName, int? version = null, string? label = null,
        bool? includeContent = null, CancellationToken cancellationToken = default)
    {
        ValidateSkillName(skillName);

        var query = QueryStringHelper.BuildVersionLabelQuery(version, label, includeContent);
        var endpoint = $"/api/public/skills/{Uri.EscapeDataString(skillName)}{query}";
        return await GetAsync<SkillVersion>(endpoint, "Get Skill", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SkillVersion> CreateSkillVersionAsync(CreateSkillVersionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        if (request.Files == null || request.Files.Count == 0)
        {
            throw new ArgumentException("At least one file must be provided", nameof(request));
        }

        foreach (var file in request.Files)
        {
            if ((file.Content == null) == (file.Sha256Hash == null))
            {
                throw new ArgumentException($"File '{file.Path}' must set exactly one of Content or Sha256Hash",
                    nameof(request));
            }
        }

        const string endpoint = "/api/public/skills";
        return await PostAsync<SkillVersion>(endpoint, request, "Create Skill Version", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SkillVersion> UpdateSkillAsync(string skillName, UpdateSkillRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateSkillName(skillName);

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"/api/public/skills/{Uri.EscapeDataString(skillName)}";
        return await PatchAsync<SkillVersion>(endpoint, request, "Update Skill", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SkillFileContentsResponse> GetSkillFileContentsAsync(IReadOnlyCollection<string> sha256Hashes,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sha256Hashes);

        if (sha256Hashes.Count == 0)
        {
            throw new ArgumentException("At least one hash must be provided", nameof(sha256Hashes));
        }

        if (sha256Hashes.Count > 50)
        {
            throw new ArgumentException("At most 50 hashes can be requested at once", nameof(sha256Hashes));
        }

        var query = QueryStringHelper.BuildSkillFileContentsQuery(sha256Hashes);
        var endpoint = $"/api/public/skills/files/content{query}";
        return await GetAsync<SkillFileContentsResponse>(endpoint, "Get Skill File Contents", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SkillVersion> UpdateSkillVersionLabelsAsync(string skillName, int version,
        UpdateSkillLabelsRequest request, CancellationToken cancellationToken = default)
    {
        ValidateSkillName(skillName);

        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var endpoint = $"/api/public/skills/{Uri.EscapeDataString(skillName)}/versions/{version}";
        return await PatchAsync<SkillVersion>(endpoint, request, "Update Skill Version Labels", cancellationToken);
    }

    /// <inheritdoc />
    public async Task<DeleteSkillVersionResponse> DeleteSkillVersionAsync(string skillName, int version,
        CancellationToken cancellationToken = default)
    {
        ValidateSkillName(skillName);

        var endpoint = $"/api/public/skills/{Uri.EscapeDataString(skillName)}/versions/{version}";
        return await DeleteAsync<DeleteSkillVersionResponse>(endpoint, "Delete Skill Version", cancellationToken);
    }

    private static void ValidateSkillName(string skillName)
    {
        if (string.IsNullOrWhiteSpace(skillName))
        {
            throw new ArgumentException("Skill name cannot be null or empty", nameof(skillName));
        }
    }
}
