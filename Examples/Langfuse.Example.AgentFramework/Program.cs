using System.ClientModel;
using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using OpenTelemetry;
using OpenTelemetry.Trace;
using zborek.Langfuse.OpenTelemetry;

var apikey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
             ?? throw new InvalidOperationException("Set OPENAI_API_KEY");
var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-5.2";
var langfusePublicKey = Environment.GetEnvironmentVariable("LANGFUSE_PUBLIC_KEY")
    ?? throw new InvalidOperationException("Set LANGFUSE_PUBLIC_KEY");
var langfuseSecretKey = Environment.GetEnvironmentVariable("LANGFUSE_SECRET_KEY")
    ?? throw new InvalidOperationException("Set LANGFUSE_SECRET_KEY");

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddLangfuseDefaultAiSources("*MyApplication*", "MyApplication")
    .AddLangfuseExporter(options =>
    {
        options.Url = "https://cloud.langfuse.com";
        options.PublicKey = langfusePublicKey;
        options.SecretKey = langfuseSecretKey;
    })
    .Build();

AIAgent agent = new OpenAIClient(new ApiKeyCredential(apikey), new OpenAIClientOptions())
    .GetChatClient(model)
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.", name: "HelloAgent", tools: [AIFunctionFactory.Create(GetWeather)])
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "MyApplication", configure: (cfg) => cfg.EnableSensitiveData = true)
    .Build();

Console.WriteLine(await agent.RunAsync("What is the weather like in Amsterdam?"));
Console.ReadKey();


[Description("Get the weather for a given location.")]
static string GetWeather([Description("The location to get the weather for.")] string location)
    => $"The weather in {location} is cloudy with a high of 15°C.";
