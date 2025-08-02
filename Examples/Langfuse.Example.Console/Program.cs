// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using zborek.Langfuse;
using zborek.Langfuse.Client;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", true, true);
builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);
builder.Services.AddLangfuse(builder.Configuration);

var app = builder.Build();

var client = app.Services.GetRequiredService<ILangfuseClient>();

var t = await client.GetHealthAsync();

Console.WriteLine();