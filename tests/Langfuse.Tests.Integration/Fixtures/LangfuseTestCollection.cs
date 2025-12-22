namespace Langfuse.Tests.Integration.Fixtures;

/// <summary>
///     Collection definition that allows sharing the LangfuseTestFixture across all tests.
///     This ensures containers are started once and reused, improving test performance.
/// </summary>
[CollectionDefinition(Name)]
public class LangfuseTestCollection : ICollectionFixture<LangfuseTestFixture>
{
    public const string Name = "Langfuse";
}