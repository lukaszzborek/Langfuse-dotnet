namespace zborek.Langfuse.Tests.OpenTelemetry;

/// <summary>
///     ActivityListeners are process-global and match ActivitySources by name, so test classes that
///     register listeners capture activities created by other classes running in parallel. Grouping
///     them in one collection forces serial execution and prevents cross-test contamination.
/// </summary>
[CollectionDefinition("ActivityListener tests")]
public class ActivityListenerCollection;
