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
builder.Services.AddHttpClient<OpenAiWithLangfuseService>();
builder.Services.AddScoped<ChatService>();

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
        generation.Metadata = new { request.Name, request.Message, Date = DateTime.UtcNow };

        langfuseTrace.Trace.Body.Output = response;
        await langfuseClient.IngestAsync(langfuseTrace);
        
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
            
            var gen = span.CreateGeneration("Embeding", prompt);
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

app.MapPost("/chatDi", async ([FromServices] ChatService chatService, [FromBody] ChatRequestDto request) =>
{
    try 
    {
        var response = await chatService.ChatAsync(request);
        
        return Results.Ok(new { response });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

// Test endpoint for new read operations
app.MapGet("/traces", async ([FromServices] ILangfuseClient langfuseClient) =>
{
    try 
    {
        var request = new zborek.Langfuse.Models.TraceListRequest
        {
            Limit = 5 // Get only first 5 traces
        };
        
        var traces = await langfuseClient.Traces.ListAsync(request);
        
        return Results.Ok(new { 
            traces = traces.Data, 
            pagination = traces.Meta,
            message = "Successfully retrieved traces using new read API"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message, message = "Error testing new read API" });
    }
});

app.MapGet("/observations", async ([FromServices] ILangfuseClient langfuseClient) =>
{
    try 
    {
        var request = new zborek.Langfuse.Models.ObservationListRequest
        {
            Limit = 5 // Get only first 5 observations
        };
        
        var observations = await langfuseClient.Observations.ListAsync(request);
        
        return Results.Ok(new { 
            observations = observations.Data, 
            pagination = observations.Meta,
            message = "Successfully retrieved observations using new read API"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message, message = "Error testing new read API" });
    }
});

app.MapGet("/sessions", async ([FromServices] ILangfuseClient langfuseClient) =>
{
    try 
    {
        var request = new zborek.Langfuse.Models.SessionListRequest
        {
            Limit = 5 // Get only first 5 sessions
        };
        
        var sessions = await langfuseClient.Sessions.ListAsync(request);
        
        return Results.Ok(new { 
            sessions = sessions.Data, 
            pagination = sessions.Meta,
            message = "Successfully retrieved sessions using new read API"
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message, message = "Error testing new read API" });
    }
});

app.Run();