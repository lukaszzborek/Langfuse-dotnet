using System.Diagnostics;
using OpenTelemetry;
using Shouldly;
using zborek.Langfuse.OpenTelemetry;

namespace zborek.Langfuse.Tests.OpenTelemetry;

[Collection("ActivityListener tests")]
public class LangfuseFilteringExporterTests : IDisposable
{
    private const string SourceName = "LangfuseFilteringExporterTests";

    private readonly ActivityListener _listener;
    private readonly ActivitySource _source;

    public LangfuseFilteringExporterTests()
    {
        _source = new ActivitySource(SourceName);
        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public void Dispose()
    {
        _listener.Dispose();
        _source.Dispose();
    }

    [Fact]
    public void Export_StampsEnvironment_WhenActivityHasNoEnvironmentTag()
    {
        var options = new LangfuseOtlpExporterOptions { Environment = "production", OnlyGenAiActivities = false };
        var innerExporter = new CapturingExporter();
        var exporter = new LangfuseFilteringExporter(innerExporter, options);

        var activity = _source.StartActivity("operation")!;
        activity.Stop();

        exporter.Export(new Batch<Activity>([activity], 1));

        innerExporter.Exported.ShouldHaveSingleItem();
        activity.GetTagItem(LangfuseAttributes.Environment).ShouldBe("production");
    }

    [Fact]
    public void Export_PreservesExistingEnvironmentTag()
    {
        var options = new LangfuseOtlpExporterOptions { Environment = "production", OnlyGenAiActivities = false };
        var innerExporter = new CapturingExporter();
        var exporter = new LangfuseFilteringExporter(innerExporter, options);

        var activity = _source.StartActivity("operation")!;
        activity.SetTag(LangfuseAttributes.Environment, "staging");
        activity.Stop();

        exporter.Export(new Batch<Activity>([activity], 1));

        activity.GetTagItem(LangfuseAttributes.Environment).ShouldBe("staging");
    }

    [Fact]
    public void Export_DoesNotStampEnvironment_WhenOptionNotSet()
    {
        var options = new LangfuseOtlpExporterOptions { OnlyGenAiActivities = false };
        var innerExporter = new CapturingExporter();
        var exporter = new LangfuseFilteringExporter(innerExporter, options);

        var activity = _source.StartActivity("operation")!;
        activity.Stop();

        exporter.Export(new Batch<Activity>([activity], 1));

        activity.GetTagItem(LangfuseAttributes.Environment).ShouldBeNull();
    }

    [Fact]
    public void Export_StampsEnvironmentOnGenAiActivities_WhenFilteringEnabled()
    {
        var options = new LangfuseOtlpExporterOptions { Environment = "production" };
        var innerExporter = new CapturingExporter();
        var exporter = new LangfuseFilteringExporter(innerExporter, options);

        var genAiActivity = _source.StartActivity("llm-call")!;
        genAiActivity.SetTag("gen_ai.request.model", "gpt-4");
        genAiActivity.Stop();

        var infraActivity = _source.StartActivity("http-request")!;
        infraActivity.Stop();

        exporter.Export(new Batch<Activity>([genAiActivity, infraActivity], 2));

        innerExporter.Exported.ShouldHaveSingleItem();
        genAiActivity.GetTagItem(LangfuseAttributes.Environment).ShouldBe("production");
        infraActivity.GetTagItem(LangfuseAttributes.Environment).ShouldBeNull();
    }

    private class CapturingExporter : BaseExporter<Activity>
    {
        public List<Activity> Exported { get; } = [];

        public override ExportResult Export(in Batch<Activity> batch)
        {
            foreach (var activity in batch)
            {
                Exported.Add(activity);
            }

            return ExportResult.Success;
        }
    }
}