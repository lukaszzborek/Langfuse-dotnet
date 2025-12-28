using Langfuse.Tests.Integration.Fixtures;
using Langfuse.Tests.Integration.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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

        var response = await client.CreateCommentAsync(request);

        response.ShouldNotBeNull();
        response.Id.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateCommentAsync_CreatesObservationComment()
    {
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

        var response = await client.CreateCommentAsync(request);

        response.ShouldNotBeNull();
        response.Id.ShouldNotBeNull();
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

        var comment = await client.GetCommentAsync(createResponse.Id);

        comment.ShouldNotBeNull();
        comment.Id.ShouldBe(createResponse.Id);
        comment.Content.ShouldBe("Comment for retrieval test");
        comment.ObjectId.ShouldBe(traceId);
    }

    [Fact]
    public async Task GetCommentAsync_NotFound_ThrowsException()
    {
        var client = CreateClient();
        var nonExistentId = Guid.NewGuid().ToString();

        var exception = await Should.ThrowAsync<LangfuseApiException>(async () =>
            await client.GetCommentAsync(nonExistentId));

        exception.StatusCode.ShouldBe(404);
    }

    [Fact]
    public async Task GetCommentListAsync_ReturnsList()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var comment1 = await client.CreateCommentAsync(new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "First comment"
        });
        var comment2 = await client.CreateCommentAsync(new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "Second comment"
        });

        var result = await client.GetCommentListAsync(new CommentListRequest
        {
            ObjectType = "TRACE",
            ObjectId = traceId,
            Page = 1,
            Limit = 50
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.Length.ShouldBe(2);
        result.Data.ShouldContain(c => c.Id == comment1.Id && c.Content == "First comment");
        result.Data.ShouldContain(c => c.Id == comment2.Id && c.Content == "Second comment");
    }

    [Fact]
    public async Task GetCommentListAsync_FiltersByObjectType()
    {
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

        var result = await client.GetCommentListAsync(new CommentListRequest
        {
            ObjectType = "TRACE",
            ObjectId = traceId,
            Page = 1,
            Limit = 50
        });

        result.ShouldNotBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.ShouldAllBe(c => c.ObjectType == CommentObjectType.Trace);
    }

    [Fact]
    public async Task CreateCommentAsync_WithAuthorUserId()
    {
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

        var response = await client.CreateCommentAsync(request);

        response.ShouldNotBeNull();
        response.Id.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateCommentAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId);

        var content = "Comprehensive test comment with detailed feedback about the trace";
        var authorId = $"author-{Guid.NewGuid():N}";

        var request = new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = content,
            AuthorUserId = authorId
        };

        var response = await client.CreateCommentAsync(request);

        response.Id.ShouldNotBeNullOrEmpty();

        var comment = await client.GetCommentAsync(response.Id);

        comment.Id.ShouldBe(response.Id);
        comment.ProjectId.ShouldBe(_fixture.ProjectId);
        comment.ObjectType.ShouldBe(CommentObjectType.Trace);
        comment.ObjectId.ShouldBe(traceId);
        comment.Content.ShouldBe(content);
        comment.AuthorUserId.ShouldBe(authorId);
        comment.CreatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        comment.UpdatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task GetCommentAsync_ValidatesAllResponseFields()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);
        var beforeTest = DateTime.UtcNow.AddSeconds(-5);

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan();
        await traceHelper.WaitForTraceAsync(traceId);
        await traceHelper.WaitForObservationAsync(spanId);

        var content = "Observation comment for field validation";

        var createResponse = await client.CreateCommentAsync(new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Observation,
            ObjectId = spanId,
            Content = content
        });

        var comment = await client.GetCommentAsync(createResponse.Id);

        comment.Id.ShouldBe(createResponse.Id);
        comment.ProjectId.ShouldBe(_fixture.ProjectId);
        comment.ObjectType.ShouldBe(CommentObjectType.Observation);
        comment.ObjectId.ShouldBe(spanId);
        comment.Content.ShouldBe(content);
        comment.CreatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        comment.UpdatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
    }
}