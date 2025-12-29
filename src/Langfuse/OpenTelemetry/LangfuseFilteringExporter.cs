using System.Diagnostics;
using OpenTelemetry;

namespace zborek.Langfuse.OpenTelemetry;

/// <summary>
///     Exporter wrapper that filters activities before passing to the underlying exporter.
///     This allows Langfuse to receive only Gen AI activities while other exporters receive all activities.
/// </summary>
internal class LangfuseFilteringExporter : BaseExporter<Activity>
{
    private static readonly string[] GenAiAttributePrefixes =
    [
        "gen_ai.",
        "langfuse."
    ];

    private readonly BaseExporter<Activity> _innerExporter;
    private readonly LangfuseOtlpExporterOptions _options;

    public LangfuseFilteringExporter(BaseExporter<Activity> innerExporter, LangfuseOtlpExporterOptions options)
    {
        _innerExporter = innerExporter;
        _options = options;
    }

    public override ExportResult Export(in Batch<Activity> batch)
    {
        var filteredActivities = new List<Activity>();

        foreach (var activity in batch)
        {
            if (ShouldExport(activity))
            {
                filteredActivities.Add(activity);
            }
        }

        if (filteredActivities.Count == 0)
        {
            return ExportResult.Success;
        }


        var filteredBatch = new Batch<Activity>(filteredActivities.ToArray(), filteredActivities.Count);
        return _innerExporter.Export(filteredBatch);
    }

    private bool ShouldExport(Activity activity)
    {
        // Check current activity and all parents - if any is not recorded, skip the whole chain
        var current = activity;
        while (current != null)
        {
            if (!current.Recorded || !current.IsAllDataRequested)
            {
                return false;
            }

            current = current.Parent;
        }

        var shouldExport = true;

        if (_options.OnlyGenAiActivities)
        {
            shouldExport = IsGenAiActivity(activity);
        }

        if (shouldExport && _options.ActivityFilter != null)
        {
            shouldExport = _options.ActivityFilter(activity);
        }

        return shouldExport;
    }

    private static bool IsGenAiActivity(Activity activity)
    {
        foreach (var tag in activity.EnumerateTagObjects())
        {
            foreach (var prefix in GenAiAttributePrefixes)
            {
                if (tag.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }

    protected override bool OnShutdown(int timeoutMilliseconds)
    {
        return _innerExporter.Shutdown(timeoutMilliseconds);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _innerExporter.Dispose();
        }

        base.Dispose(disposing);
    }
}