using System.Text.Json;
using Langfuse.Example.WebApi.Models;
using zborek.Langfuse.Services;

namespace Langfuse.Example.WebApi.Services;

public class OpenAiWithLangfuseService
{
    private readonly HttpClient _httpClient;
    private readonly LangfuseTrace _langfuseTrace;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.openai.com/v1";

    public OpenAiWithLangfuseService(HttpClient httpClient, LangfuseTrace langfuseTrace, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _langfuseTrace = langfuseTrace;
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<ChatCompletionResponse?> GetChatCompletionAsync(string model, string prompt)
    {
        using var generation = _langfuseTrace.CreateGeneration("openai-chat-completion", prompt);
        generation.Model = model;
        
        var requestBody = new
        {
            model = model,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var response = await _httpClient.PostAsync(
            $"{BaseUrl}/chat/completions",
            new StringContent(JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json")
        );

        response.EnsureSuccessStatusCode();
        
        var result =  await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();
        
        generation.Model = "gpt-4o-mini";
        
        if (result?.Choices.Count > 0)
        {
            generation.SetOutput(result.Choices[0]);
            generation.SetUsage(result.Usage);
        }
        
        return result;
    }
}
