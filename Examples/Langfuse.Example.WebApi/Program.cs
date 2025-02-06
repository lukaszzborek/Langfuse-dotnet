using System.Text.Json;
using Langfuse;
using Langfuse.Example.WebApi.Models;
using Langfuse.Example.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddLangfuse(builder.Configuration);
builder.Services.AddHttpClient<OpenAiService>();

var app = builder.Build();

app.MapPost("/chat", async ([FromServices] ILangfuseClient langfuseClient, 
    [FromServices] OpenAiService openAiService, [FromBody] ChatRequestDto request) =>
{
    try
    {
        var langfuseTrace = new LangfuseTrace(request.Name, TimeProvider.System);
        langfuseTrace.Trace.Body.Input = request.Message;
        
        var data = await GetDataFromDb(langfuseTrace, request.Message);
        var prompt = $"""
                       <data>
                       {data}
                       </data>

                       <prompt>
                       {request.Message}
                       </prompt>
                       """;
        
        var response = await openAiService.GetChatCompletionAsync(request.Model, prompt);

        if(response == null)
        {
            return Results.BadRequest(new { error = "Failed to get response from OpenAI" });
        }
        
        var generation = langfuseTrace.CreateGeneration("generation", input: prompt);
        generation.Model = request.Model;
        generation.SetOutput(response.Choices[0]);
        generation.SetUsage(response.Usage);

        await langfuseClient.IngestAsync(langfuseTrace);
        langfuseTrace.Trace.Body.Output = response;
        
        return Results.Ok(new { response });
        
        async Task<string> GetDataFromDb(LangfuseTrace langfuseTrace1, string requestMessage)
        {
            var span = langfuseTrace1.CreateSpan("GetDataFromDb");
            var prompt = $"""
                          <task>
                          Ask helper question about prompt
                          </task>

                          <prompt>
                          {requestMessage}
                          </prompt>
                          """;
            var additionalQuestions = await openAiService.GetChatCompletionAsync("gpt-4o-mini", prompt);
            
            var gen = span.CreateGenerationEvent("Embeding", prompt);
            await Task.Delay(1000);
            
            span.SetOutput("Data from db");
            span.CreateEvent("Data downloaded", input: requestMessage, output: "Data from db");
            
            if (additionalQuestions == null)
            {
                return "Data from db";
            }
            
            gen.Model = "gpt-4o-mini";
            gen.SetOutput(additionalQuestions.Choices[0]);
            gen.SetUsage(additionalQuestions.Usage);
            
            return additionalQuestions.Choices[0].Message.Content;
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/chatDi", async ([FromServices] ILangfuseClient langfuseClient, [FromServices] LangfuseTrace langfuseTrace, 
    [FromServices] OpenAiService openAiService, [FromBody] ChatRequestDto request) =>
{
    try 
    {
        langfuseTrace.SetTraceName(request.Name);
        langfuseTrace.Trace.Body.Input = request.Message;
        var data = await GetDataFromDb(langfuseTrace, request.Message);

        var prompt = $"""
                       <data>
                       {data}
                       </data>

                       <prompt>
                       {request.Message}
                       </prompt>
                       """;
        
        var response = await openAiService.GetChatCompletionAsync(request.Model, prompt);
        if (response == null)
        {
            return Results.BadRequest(new { error = "Failed to get response from OpenAI" });
        }
        
        var generation = langfuseTrace.CreateGeneration("generation", input: prompt);
        generation.Model = request.Model;
        generation.SetOutput(response.Choices[0]);
        generation.SetUsage(response.Usage);

        await langfuseClient.IngestAsync(langfuseTrace);

        langfuseTrace.Trace.Body.Output = response.Choices[0];
        return Results.Ok(new { response });
        
        async Task<string> GetDataFromDb(LangfuseTrace langfuseTrace1, string requestMessage)
        {
            var span = langfuseTrace1.CreateSpan("GetDataFromDb");
            var prompt = $"""
                          <task>
                          Ask helper question about prompt
                          </task>

                          <prompt>
                          {requestMessage}
                          </prompt>
                          """;
            var additionalQuestions = await openAiService.GetChatCompletionAsync("gpt-4o-mini", prompt);
            
            var gen = span.CreateGenerationEvent("Embeding", prompt);
            await Task.Delay(1000);
            
            span.SetOutput("Data from db");
            span.CreateEvent("Data downloaded", input: requestMessage, output: "Data from db");
            
            if (additionalQuestions == null)
            {
                return "Data from db";
            }
            
            // write to langfuse
            
            gen.Model = "gpt-4o-mini";
            gen.SetOutput(additionalQuestions.Choices[0]);
            gen.SetUsage(additionalQuestions.Usage);
            
            return additionalQuestions.Choices[0].Message.Content;
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();