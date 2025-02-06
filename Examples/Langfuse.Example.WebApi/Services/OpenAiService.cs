using System.Text.Json;
using Langfuse.Example.WebApi.Models;

namespace Langfuse.Example.WebApi.Services;

public class OpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string BaseUrl = "https://api.openai.com/v1";

    public OpenAiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey");
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    public async Task<ChatCompletionResponse?> GetChatCompletionAsync(string model, string prompt)
    {
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
        
        return await response.Content.ReadFromJsonAsync<ChatCompletionResponse>();
    }
}
