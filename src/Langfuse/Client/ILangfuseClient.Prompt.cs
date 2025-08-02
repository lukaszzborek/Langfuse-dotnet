using zborek.Langfuse.Models;

namespace zborek.Langfuse.Client;

public partial interface ILangfuseClient
{
    Task<PromptMetaListResponse> GetPromptListAsync(PromptListRequest request,
        CancellationToken cancellationToken = default);

    Task<Prompt> GetPromptAsync(string promptName, int? version = null, string? label = null,
        CancellationToken cancellationToken = default);

    Task<Prompt> CreatePromptAsync(CreatePromptRequest request, CancellationToken cancellationToken = default);

    Task<Prompt> UpdatePromptVersionAsync(string promptName, int version, UpdatePromptVersionRequest request,
        CancellationToken cancellationToken = default);
}