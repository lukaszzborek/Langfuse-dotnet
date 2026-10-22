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
        });

        var provider = services.BuildServiceProvider();
        return provider.GetRequiredService<ILangfuseClient>();
    }

    private TraceTestHelper CreateTraceHelper(ILangfuseClient client)
    {
        return new TraceTestHelper(client, _fixture);
    }

    // The comment endpoint checks a different table than observations v2, which WaitForTraceAsync polls,
    // so the referenced object can still be missing for a moment and the server answers 404.
    private static async Task<CreateCommentResponse> CreateCommentWhenReadyAsync(ILangfuseClient client,
        CreateCommentRequest request)
    {
        var deadline = DateTime.UtcNow.AddSeconds(30);

        while (true)
        {
            try
            {
                return await client.CreateCommentAsync(request);
            }
            catch (LangfuseApiException ex) when (ex.StatusCode == 404 && DateTime.UtcNow < deadline)
            {
                await Task.Delay(500);
            }
        }
    }

    [Fact]
    public async Task CreateCommentAsync_CreatesTraceComment()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var traceId = traceHelper.CreateTrace();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: TestContext.Current.CancellationToken);

        var request = new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "This is a test comment on a trace"
        };

        var response = await CreateCommentWhenReadyAsync(client,request);

        response.ShouldNotBeNull();
        response.Id.ShouldNotBeNull();
    }

    [Fact]
    public async Task CreateCommentAsync_CreatesObservationComment()
    {
        var client = CreateClient();
        var traceHelper = CreateTraceHelper(client);

        var (traceId, spanId) = traceHelper.CreateTraceWithSpan();
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: TestContext.Current.CancellationToken);
        await traceHelper.WaitForObservationAsync(spanId, cancellationToken: TestContext.Current.CancellationToken);

        var request = new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Observation,
            ObjectId = spanId,
            Content = "This is a test comment on an observation"
        };

        var response = await CreateCommentWhenReadyAsync(client,request);

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
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: TestContext.Current.CancellationToken);

        var createResponse = await CreateCommentWhenReadyAsync(client,new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "Comment for retrieval test"
        });

        var comment = await client.GetCommentAsync(createResponse.Id, TestContext.Current.CancellationToken);

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
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: TestContext.Current.CancellationToken);

        var comment1 = await CreateCommentWhenReadyAsync(client,new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "First comment"
        });
        var comment2 = await CreateCommentWhenReadyAsync(client,new CreateCommentRequest
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
        }, TestContext.Current.CancellationToken);

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
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: TestContext.Current.CancellationToken);

        await CreateCommentWhenReadyAsync(client,new CreateCommentRequest
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
        }, TestContext.Current.CancellationToken);

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
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: TestContext.Current.CancellationToken);

        var authorId = _fixture.UserId;
        var request = new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = "Comment with author",
            AuthorUserId = authorId
        };

        var response = await CreateCommentWhenReadyAsync(client,request);

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
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: TestContext.Current.CancellationToken);

        var content = "Comprehensive test comment with detailed feedback about the trace";
        var authorId = _fixture.UserId;

        var request = new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Trace,
            ObjectId = traceId,
            Content = content,
            AuthorUserId = authorId
        };

        var response = await CreateCommentWhenReadyAsync(client,request);

        response.Id.ShouldNotBeNullOrEmpty();

        var comment = await client.GetCommentAsync(response.Id, TestContext.Current.CancellationToken);

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
        await traceHelper.WaitForTraceAsync(traceId, cancellationToken: TestContext.Current.CancellationToken);
        await traceHelper.WaitForObservationAsync(spanId, cancellationToken: TestContext.Current.CancellationToken);

        var content = "Observation comment for field validation";

        var createResponse = await CreateCommentWhenReadyAsync(client,new CreateCommentRequest
        {
            ProjectId = _fixture.ProjectId,
            ObjectType = CommentObjectType.Observation,
            ObjectId = spanId,
            Content = content
        });

        var comment = await client.GetCommentAsync(createResponse.Id, TestContext.Current.CancellationToken);

        comment.Id.ShouldBe(createResponse.Id);
        comment.ProjectId.ShouldBe(_fixture.ProjectId);
        comment.ObjectType.ShouldBe(CommentObjectType.Observation);
        comment.ObjectId.ShouldBe(spanId);
        comment.Content.ShouldBe(content);
        comment.CreatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
        comment.UpdatedAt.ShouldBe(beforeTest, TimeSpan.FromMinutes(1));
    }
}