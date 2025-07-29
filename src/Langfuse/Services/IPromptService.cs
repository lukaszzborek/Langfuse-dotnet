using zborek.Langfuse.Models;

namespace zborek.Langfuse.Services;

public interface IPromptService
{
    Task<PromptMetaListResponse> ListAsync(PromptListRequest request, CancellationToken cancellationToken = default);

    Task<Prompt> GetAsync(string promptName, int? version = null, string? label = null,
        CancellationToken cancellationToken = default);

    Task<Prompt> CreateAsync(CreatePromptRequest request, CancellationToken cancellationToken = default);

    Task<Prompt> UpdateVersionAsync(string promptName, int version, UpdatePromptVersionRequest request,
        CancellationToken cancellationToken = default);
}