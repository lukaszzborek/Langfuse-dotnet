using System.Threading.Channels;
using Langfuse.Client;
using Langfuse.Config;
using Langfuse.Models;
using Langfuse.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Langfuse;

public static class Extensions
{
    public static IServiceCollection AddLangfuse(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LangfuseConfig>(configuration.GetSection("Langfuse"));

        var config = configuration.GetSection("Langfuse").Get<LangfuseConfig>();
        
        if(config == null)
        {
            throw new Exception("Langfuse configuration is missing");
        }

        services.AddScoped<LangfuseTrace>();
        services.AddScoped<AuthorizationDelegatingHandler>();
        services.AddSingleton(sp => Channel.CreateUnbounded<IIngestionEvent>());
        services.AddHttpClient<ILangfuseClient, LangfuseClient>(x =>
        {
            x.BaseAddress = new Uri(config.Url);
        }).AddHttpMessageHandler<AuthorizationDelegatingHandler>();

        if (config.BatchMode)
        {
            services.AddHostedService<LangfuseBackgroundService>();
        }

        return services;
    }
}