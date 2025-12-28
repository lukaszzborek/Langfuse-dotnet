using System.Diagnostics;
using Langfuse.Example.OpenTelemetry;
using Langfuse.Example.OpenTelemetry.Endpoints;
using Langfuse.Example.OpenTelemetry.Services;
using OpenTelemetry.Resources;
using zborek.Langfuse.OpenTelemetry;

Environment.SetEnvironmentVariable("OTEL_LOG_LEVEL", "debug");
Environment.SetEnvironmentVariable("OTEL_DOTNET_AUTO_LOG_DIRECTORY", "./logs");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("langfuse-otel-webapi-example", serviceVersion: "1.0.0"))
    .WithTracing(tracing =>
    {
        tracing.AddLangfuseExporter(builder.Configuration.GetSection("Langfuse"));
        tracing.AddSource("Langfuse.Example.OpenTelemetry");
    });

builder.Services.AddLangfuseTracing();

builder.Services.AddHttpClient<OtelOpenAiService>();
builder.Services.AddScoped<OtelDataService>();
builder.Services.AddScoped<OtelChatService>();
builder.Services.AddScoped<ParallelSummarizationService>();

builder.Services.AddSingleton(new ActivitySource("Langfuse.Example.OpenTelemetry"));

var app = builder.Build();

app.MapOtelTraceExamples();
app.MapGenAiActivityExamples();
app.MapDiExamples();

app.Run();
