using Microsoft.AspNetCore.Mvc;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Observation;
using zborek.Langfuse.Models.Session;
using zborek.Langfuse.Models.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddLangfuse(builder.Configuration);

var app = builder.Build();

// Test endpoint for new read operations
app.MapGet("/traces", async ([FromServices] ILangfuseClient langfuseClient) =>
{
    try
    {
        var request = new TraceListRequest
        {
            Limit = 5 // Get only first 5 traces
        };

        var traces = await langfuseClient.GetTraceListAsync(request);

        return Results.Ok(new
        {
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
        var request = new ObservationListRequest
        {
            Limit = 5 // Get only first 5 observations
        };

        var observations = await langfuseClient.GetObservationListAsync(request);

        return Results.Ok(new
        {
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
        var request = new SessionListRequest
        {
            Limit = 5 // Get only first 5 sessions
        };

        var sessions = await langfuseClient.GetSessionListAsync(request);

        return Results.Ok(new
        {
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
