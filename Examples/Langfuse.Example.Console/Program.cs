// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
builder.Configuration.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
builder.Services.AddLangfuse(builder.Configuration);

var app = builder.Build();

var client =  app.Services.GetRequiredService<ILangfuseClient>();


Console.WriteLine();