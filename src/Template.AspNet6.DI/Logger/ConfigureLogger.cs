using System.Diagnostics;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Template.AspNet6.DI.Logger;

public static class Logger
{
    public static IServiceCollection ConfigureLogger(this IServiceCollection services, IConfiguration config)
    {
        var aiOptions = new ApplicationInsightsServiceOptions
        {
            EnableAdaptiveSampling = false,
            InstrumentationKey = config["ApplicationInsights:InstrumentationKey"],
            EnableQuickPulseMetricStream = true,
            EnableRequestTrackingTelemetryModule = true,
            EnableDependencyTrackingTelemetryModule = true,
            EnablePerformanceCounterCollectionModule = true,
            EnableEventCounterCollectionModule = true,
            DependencyCollectionOptions = {EnableLegacyCorrelationHeadersInjection = true},
            DeveloperMode = bool.TryParse(config["Logging:DeveloperMode"], out bool result) && result
        };

        services.AddApplicationInsightsTelemetry(aiOptions);

        services.AddSingleton<ITelemetryInitializer, ContextUserInjector>();
        services.AddApplicationInsightsTelemetryProcessor<TelemetryCleaner>();

        services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => { module.EnableSqlCommandTextInstrumentation = true; });

        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        Activity.ForceDefaultIdFormat = true;

        return services;
    }
}
