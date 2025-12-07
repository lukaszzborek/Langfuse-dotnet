using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenTelemetry;
using zborek.Langfuse.Client;
using zborek.Langfuse.Config;
using zborek.Langfuse.Models.Core;
using zborek.Langfuse.OpenTelemetry;
using zborek.Langfuse.OpenTelemetry.Trace;
using zborek.Langfuse.Services;

namespace zborek.Langfuse;

/// <summary>
///     Extension methods for setting up Langfuse services in an <see cref="IServiceCollection" />.
/// </summary>
public static class Extensions
{
    private static bool _activityListenerRegistered;
    private static readonly object _listenerLock = new();

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

    /// <summary>
    ///     Registers the Langfuse ActivityListener for automatic context enrichment from Baggage.
    ///     Call this once at application startup. Safe to call multiple times - will only register once.
    /// </summary>
    public static void UseLangfuseActivityListener()
    {
        if (_activityListenerRegistered)
        {
            return;
        }

        lock (_listenerLock)
        {
            if (_activityListenerRegistered)
            {
                return;
            }

            _activityListenerRegistered = true;
        }

        ActivitySource.AddActivityListener(new ActivityListener
        {
            ShouldListenTo = source => source.Name == OtelLangfuseTrace.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity =>
            {
                if (activity == null)
                {
                    return;
                }

                // Auto-enrich from Baggage
                var userId = Baggage.GetBaggage(LangfuseBaggageKeys.UserId);
                if (!string.IsNullOrEmpty(userId))
                {
                    activity.SetTag(LangfuseAttributes.UserId, userId);
                }

                var sessionId = Baggage.GetBaggage(LangfuseBaggageKeys.SessionId);
                if (!string.IsNullOrEmpty(sessionId))
                {
                    activity.SetTag(LangfuseAttributes.SessionId, sessionId);
                }

                var version = Baggage.GetBaggage(LangfuseBaggageKeys.Version);
                if (!string.IsNullOrEmpty(version))
                {
                    activity.SetTag(LangfuseAttributes.Version, version);
                }

                var release = Baggage.GetBaggage(LangfuseBaggageKeys.Release);
                if (!string.IsNullOrEmpty(release))
                {
                    activity.SetTag(LangfuseAttributes.Release, release);
                }

                var tags = Baggage.GetBaggage(LangfuseBaggageKeys.Tags);
                if (!string.IsNullOrEmpty(tags))
                {
                    activity.SetTag(LangfuseAttributes.TraceTags, tags.Split(','));
                }
            }
        });
    }
}