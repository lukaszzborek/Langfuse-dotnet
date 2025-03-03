using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using zborek.Langfuse.Client;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models;
using zborek.Langfuse.Services;

namespace zborek.Langfuse;

/// <summary>
///     Extension methods for setting up Langfuse services in an <see cref="IServiceCollection" />.
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Registers Langfuse services in an <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddLangfuse(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LangfuseConfig>(configuration.GetSection("Langfuse"));

        var config = configuration.GetSection("Langfuse").Get<LangfuseConfig>();

        if (config == null)
        {
            throw new Exception("Langfuse configuration is missing");
        }

        services.TryAddSingleton(TimeProvider.System);
        ;
        services.AddScoped<LangfuseTrace>();
        services.AddScoped<AuthorizationDelegatingHandler>();
        services.AddSingleton(sp => Channel.CreateUnbounded<IIngestionEvent>());
        services.AddHttpClient<ILangfuseClient, LangfuseClient>(x => { x.BaseAddress = new Uri(config.Url); })
            .AddHttpMessageHandler<AuthorizationDelegatingHandler>();

        if (config.BatchMode)
        {
            services.AddHostedService<LangfuseBackgroundService>();
        }

        return services;
    }
}