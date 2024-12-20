using System.Text.Json;
using Langfuse;
using Langfuse.Client;
using Langfuse.Example.WebApi.Models;
using Langfuse.Example.WebApi.Services;
using Langfuse.Services;
using Microsoft.AspNetCore.Mvc;

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
        var response = await openAiService.GetChatCompletionAsync(request.Model, $"{data}-{request.Message}");

        var generation = langfuseTrace.CreateGenerationEvent("generation", input: request.Message);
        generation.Model = request.Model;
        generation.SetOutput(response);
        generation.SetUsage(JsonSerializer.Deserialize<TokenInfo>("""
                                                                  {
                                                                     "prompt_tokens":9,
                                                                     "completion_tokens":12,
                                                                     "total_tokens":21,
                                                                     "completion_tokens_details":{
                                                                        "reasoning_tokens":0,
                                                                        "accepted_prediction_tokens":0,
                                                                        "rejected_prediction_tokens":0
                                                                     }
                                                                  }
                                                                  """)!);

        await langfuseClient.IngestAsync(langfuseTrace);
        langfuseTrace.Trace.Body.Output = response;
        
        return Results.Ok(new { response });
        
        async Task<string> GetDataFromDb(LangfuseTrace langfuseTrace1, string requestMessage)
        {
            var span = langfuseTrace1.CreateSpan("GetDataFromDb");
            var gen = span.CreateGenerationEvent("Embeding", requestMessage);
            await Task.Delay(1000);

            gen.SetOutput("");
            gen.SetUsage(JsonSerializer.Deserialize<TokenInfo>("""
                                                               {
                                                                  "prompt_tokens":9,
                                                                  "completion_tokens":12,
                                                                  "total_tokens":21,
                                                                  "completion_tokens_details":{
                                                                     "reasoning_tokens":0,
                                                                     "accepted_prediction_tokens":0,
                                                                     "rejected_prediction_tokens":0
                                                                  }
                                                               }
                                                               """)!);
            span.SetOutput("Data from db");
            
            return "Data from db";
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
        
        var response = await openAiService.GetChatCompletionAsync(request.Model, $"{data}-{request.Message}");
        
        var generation = langfuseTrace.CreateGenerationEvent("generation", input: request.Message);
        generation.Model = request.Model;
        generation.SetOutput(response);
        generation.SetUsage(JsonSerializer.Deserialize<TokenInfo>("""
                                                                  {
                                                                     "prompt_tokens":9,
                                                                     "completion_tokens":12,
                                                                     "total_tokens":21,
                                                                     "completion_tokens_details":{
                                                                        "reasoning_tokens":0,
                                                                        "accepted_prediction_tokens":0,
                                                                        "rejected_prediction_tokens":0
                                                                     }
                                                                  }
                                                                  """)!);

        await langfuseClient.IngestAsync(langfuseTrace);

        langfuseTrace.Trace.Body.Output = response;
        return Results.Ok(new { response });
        
        async Task<string> GetDataFromDb(LangfuseTrace langfuseTrace1, string requestMessage)
        {
            var span = langfuseTrace1.CreateSpan("GetDataFromDb");
            var gen = span.CreateGenerationEvent("Embeding", requestMessage);
            await Task.Delay(1000);

            gen.SetOutput("");
            gen.SetUsage(JsonSerializer.Deserialize<TokenInfo>("""
                                                                 {
                                                                    "prompt_tokens":9,
                                                                    "completion_tokens":12,
                                                                    "total_tokens":21,
                                                                    "completion_tokens_details":{
                                                                       "reasoning_tokens":0,
                                                                       "accepted_prediction_tokens":0,
                                                                       "rejected_prediction_tokens":0
                                                                    }
                                                                 }
                                                                 """)!);
            span.SetOutput("Data from db");
            
            return "Data from db";
        }
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();