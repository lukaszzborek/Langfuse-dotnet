using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using zborek.Langfuse;
using zborek.Langfuse.Client;
using zborek.Langfuse.Models.Comment;
using zborek.Langfuse.Models.Core;

namespace Langfuse.Tests.Integration;

[Collection(LangfuseTestCollection.Name)]
public class CommentTests
{
    private readonly LangfuseTestFixture _fixture;

    public CommentTests(LangfuseTestFixture fixture)
    {
        _fixture = fixture;
    }

    private ILangfuseClient CreateClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLangfuse(config =>
        {
            config.Url = _fixture.LangfuseBaseUrl;
            config.PublicKey = _fixture.PublicKey;
            config.SecretKey = _fixture.SecretKey;
            config.BatchMode = false;
        });

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ILangfuseClient>();
    }

    private TraceTestHelper CreateTraceHelper(ILangfuseClient client)
    {
        return new TraceTestHelper(client, _fixture);
    }

    [Fact]
    public async Task CreateCommentAsync_CreatesTraceComment()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var request = new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "This is a test comment on a trace"
        };

        // Act
        var response = await client.CreateCommentAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Id);
    }

    [Fact]
    public async Task CreateCommentAsync_CreatesObservationComment()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        var request = new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Observation,
            ObjectId = spanId,
            Content = "This is a test comment on an observation"
        };

        // Act
        var response = await client.CreateCommentAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Id);
    }

    [Fact]
    public async Task GetCommentAsync_ReturnsComment()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var createResponse = await client.CreateCommentAsync(new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "Comment for retrieval test"
        });

        // Act
        var comment = await client.GetCommentAsync(createResponse.Id);

        // Assert
        Assert.NotNull(comment);
        Assert.Equal(createResponse.Id, comment.Id);
        Assert.Equal("Comment for retrieval test", comment.Content);
        Assert.Equal(traceId, comment.ObjectId);
    }

    [Fact]
    public async Task GetCommentAsync_NotFound_ThrowsException()
    {
        // Arrange
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<LangfuseApiException>(() =>
            client.GetCommentAsync(nonExistentId));

        Assert.Equal(404, exception.StatusCode);
    }

    [Fact]
    public async Task GetCommentListAsync_ReturnsList()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        // Create multiple comments
        await client.CreateCommentAsync(new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "First comment"
        });
        await client.CreateCommentAsync(new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "Second comment"
        });

        // Act
        var result = await client.GetCommentListAsync(new CommentListRequest
        {
            ObjectType = "TRACE",
            ObjectId = traceId,
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Length >= 2);
    }

    [Fact]
    public async Task GetCommentListAsync_FiltersByObjectType()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        await client.CreateCommentAsync(new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "Trace comment for filter test"
        });

        // Act
        var result = await client.GetCommentListAsync(new CommentListRequest
        {
            ObjectType = "TRACE",
            ObjectId = traceId,
            Page = 1,
            Limit = 50
        });

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.All(result.Data, c => Assert.Equal(CommentObjectType.Trace, c.ObjectType));
    }

    [Fact]
    public async Task CreateCommentAsync_WithAuthorUserId()
    {
        // Arrange
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var authorId = $"author-{Guid.NewGuid():N}";
        var request = new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "Comment with author",
            AuthorUserId = authorId
        };

        // Act
        var response = await client.CreateCommentAsync(request);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Id);
    }
}