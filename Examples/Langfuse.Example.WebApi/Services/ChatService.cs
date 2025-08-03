using Langfuse.Example.WebApi.Models;
using zborek.Langfuse.Services;

namespace Langfuse.Example.WebApi.Services;

public class ChatService
{
    private readonly LangfuseTrace _langfuseTrace;
    private readonly OpenAiWithLangfuseService _openAiService;

    public ChatService(LangfuseTrace langfuseTrace, OpenAiWithLangfuseService openAiService)
    {
        _langfuseTrace = langfuseTrace;
        _openAiService = openAiService;
    }
    
    public async Task<string> ChatAsync(ChatRequestDto request, CancellationToken cancellationToken = default)
    {
        _langfuseTrace.SetInput(request.Message);
        _langfuseTrace.SetTraceName(request.Name);
        _langfuseTrace.Trace.Body.Metadata = new { request.Name, request.Message, Date = DateTime.UtcNow };
        
        using var span = _langfuseTrace.CreateSpanScoped(request.Message);
        
        var data = await GetDataFromDb(request.Message);
        var prompt = $"""
                      <data>
                      {data}
                      </data>

                      <prompt>
                      {request.Message}
                      </prompt>
                      """;
        
        var response = await _openAiService.GetChatCompletionAsync(request.Model, prompt);

        if(response == null)
        {
            throw new BadHttpRequestException("Failed to get response from OpenAI");
        }

        _langfuseTrace.SetOutput(response.Choices[0].Message.Content);
        await _langfuseTrace.IngestAsync();
        
        return response.Choices[0].Message.Content;
    }

    private async Task<string> GetDataFromDb(string requestMessage)
    {
        using var span = _langfuseTrace.CreateSpanScoped("GetDataFromDb");
        var prompt = $"""
                      <task>
                      Ask helper question about prompt
                      </task>

                      <prompt>
                      {requestMessage}
                      </prompt>
                      """;
        
        var additionalQuestions = await _openAiService.GetChatCompletionAsync("gpt-4o-mini", prompt);
        
        await Task.Delay(1000);
        
        span.SetOutput("Data from db");
        span.CreateEvent("Data downloaded", input: requestMessage, output: "Data from db");
            
        if (additionalQuestions == null)
        {
            return "Data from db";
        }
            
        return additionalQuestions.Choices[0].Message.Content;
    }
}