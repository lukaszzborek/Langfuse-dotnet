using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using zborek.Langfuse.Client;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models.Core;
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
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddLangfuse(this IServiceCollection services, Action<LangfuseConfig> configure)
    {
        var config = new LangfuseConfig();
        configure(config);

        services.Configure(configure);

        return ServiceCollection(services, config);
    }

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

        return ServiceCollection(services, config);
    }

    private static IServiceCollection ServiceCollection(IServiceCollection services, LangfuseConfig? config)
    {
        if (config == null)
        {
            throw new Exception("Langfuse configuration is missing");
        }

        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<LangfuseTrace>();
        services.AddScoped<AuthorizationDelegatingHandler>();
        services.AddSingleton(Channel.CreateUnbounded<IIngestionEvent>());

        services.AddHttpClient<ILangfuseClient, LangfuseClient>(x => { x.BaseAddress = new Uri(config.Url); })
            .AddHttpMessageHandler<AuthorizationDelegatingHandler>();

        if (config.BatchMode)
        {
            services.AddHostedService<LangfuseBackgroundService>();
        }

        return services;
    }
}