using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using OpenTelemetry;
using OpenTelemetry.Trace;
using Testcontainers.ClickHouse;
using Testcontainers.Minio;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using zborek.Langfuse.OpenTelemetry;

namespace Langfuse.Tests.Integration.Fixtures;

/// <summary>
///     Test fixture that provides a complete Langfuse infrastructure using Testcontainers.
///     This includes PostgreSQL, ClickHouse, Redis, MinIO, and the Langfuse web/worker services.
/// </summary>
public class LangfuseTestFixture : IAsyncLifetime
{
    private const string PostgresUser = "postgres";
    private const string PostgresPassword = "postgres";
    private const string PostgresDatabase = "postgres";

    private const string ClickHouseUser = "clickhouse";
    private const string ClickHousePassword = "clickhouse";

    private const string RedisPassword = "myredissecret";

    private const string MinioUser = "minio";
    private const string MinioPassword = "miniosecret";
    private const string MinioBucket = "langfuse";

    private const string Salt = "mysalt";
    private const string EncryptionKey = "0000000000000000000000000000000000000000000000000000000000000000";
    private const string NextAuthSecret = "mysecret";

    // Init project credentials for testing
    public const string InitProjectPublicKey = "pk-lf-test-public-key";
    public const string InitProjectSecretKey = "sk-lf-test-secret-key";
    private ClickHouseContainer? _clickHouseContainer;
    private IContainer? _langfuseWebContainer;
    private IContainer? _langfuseWorkerContainer;
    private MinioContainer? _minioContainer;

    private INetwork? _network;
    private PostgreSqlContainer? _postgresContainer;
    private RedisContainer? _redisContainer;

    /// <summary>
    ///     Gets the Langfuse API base URL for testing.
    /// </summary>
    public string LangfuseBaseUrl => $"http://127.0.0.1:{_langfuseWebContainer?.GetMappedPublicPort(3000)}";

    /// <summary>
    ///     Gets the public key for the test project.
    /// </summary>
    public string PublicKey => InitProjectPublicKey;

    /// <summary>
    ///     Gets the secret key for the test project.
    /// </summary>
    public string SecretKey => InitProjectSecretKey;

    /// <summary>
    ///     Gets the project ID for the test project.
    /// </summary>
    public string ProjectId => "test-project";

    /// <summary>
    ///     Langfuse v4 write mode the containers run in: "dual" (default) or "events_only".
    ///     Set via LANGFUSE_TEST_V4_MODE. Tests tagged [Trait("Legacy", "true")] require "dual".
    /// </summary>
    public static string V4Mode { get; } =
        Environment.GetEnvironmentVariable("LANGFUSE_TEST_V4_MODE") is "events_only" ? "events_only" : "dual";

    /// <summary>
    ///     Gets the ID of the init user (member of the test organization).
    /// </summary>
    public string UserId { get; private set; } = string.Empty;

    /// <summary>
    ///     Gets the TracerProvider for OpenTelemetry-based tracing.
    /// </summary>
    public TracerProvider? TracerProvider { get; private set; }

    public async ValueTask InitializeAsync()
    {
        // Create a shared network for all containers
        _network = new NetworkBuilder()
            .WithName($"langfuse-test-{Guid.NewGuid():N}")
            .Build();

        await _network.CreateAsync();

        // Start infrastructure containers in parallel
        await StartInfrastructureContainersAsync();

        // Start Langfuse services (they depend on infrastructure)
        await StartLangfuseServicesAsync();

        UserId = await GetInitUserIdAsync();

        // Configure OpenTelemetry TracerProvider after services are running
        ConfigureOpenTelemetry();
    }

    public async ValueTask DisposeAsync()
    {
        // Dispose TracerProvider first to flush any pending exports
        TracerProvider?.Dispose();

        // Dispose containers in reverse order of dependencies
        if (_langfuseWebContainer is not null)
        {
            await _langfuseWebContainer.DisposeAsync();
        }

        if (_langfuseWorkerContainer is not null)
        {
            await _langfuseWorkerContainer.DisposeAsync();
        }

        if (_minioContainer is not null)
        {
            await _minioContainer.DisposeAsync();
        }

        if (_redisContainer is not null)
        {
            await _redisContainer.DisposeAsync();
        }

        if (_clickHouseContainer is not null)
        {
            await _clickHouseContainer.DisposeAsync();
        }

        if (_postgresContainer is not null)
        {
            await _postgresContainer.DisposeAsync();
        }

        if (_network is not null)
        {
            await _network.DeleteAsync();
        }
    }

    /// <summary>
    ///     Flushes all pending OpenTelemetry exports to ensure data is sent to Langfuse.
    /// </summary>
    public void FlushTraces()
    {
        TracerProvider?.ForceFlush();
    }

    private void ConfigureOpenTelemetry()
    {
        // Register the Langfuse ActivityListener for automatic context enrichment
        LangfuseOtlpExtensions.UseLangfuseActivityListener();

        TracerProvider = Sdk.CreateTracerProviderBuilder()
            .AddLangfuseExporter(options =>
            {
                options.Url = LangfuseBaseUrl;
                options.PublicKey = PublicKey;
                options.SecretKey = SecretKey;
                options.OnlyGenAiActivities = true;
            })
            .Build();
    }

    private async Task StartInfrastructureContainersAsync()
    {
        // PostgreSQL
        _postgresContainer = new PostgreSqlBuilder("postgres:18")
            .WithNetwork(_network)
            .WithNetworkAliases("postgres")
            .WithUsername(PostgresUser)
            .WithPassword(PostgresPassword)
            .WithDatabase(PostgresDatabase)
            .WithImagePullPolicy(_ => true)
            .Build();

        // ClickHouse
        _clickHouseContainer = new ClickHouseBuilder("clickhouse/clickhouse-server:latest")
            .WithNetwork(_network)
            .WithNetworkAliases("clickhouse")
            .WithUsername(ClickHouseUser)
            .WithPassword(ClickHousePassword)
            .WithImagePullPolicy(_ => true)
            .Build();

        // Redis
        _redisContainer = new RedisBuilder("redis:latest")
            .WithNetwork(_network)
            .WithNetworkAliases("redis")
            .WithCommand("--requirepass", RedisPassword, "--maxmemory-policy", "noeviction")
            .WithImagePullPolicy(_ => true)
            .Build();

        // MinIO
        _minioContainer = new MinioBuilder("cgr.dev/chainguard/minio")
            .WithNetwork(_network)
            .WithNetworkAliases("minio")
            .WithUsername(MinioUser)
            .WithPassword(MinioPassword)
            .WithTmpfsMount("/data")
            .WithImagePullPolicy(_ => true)
            .Build();

        // Start all infrastructure containers in parallel
        await Task.WhenAll(
            _postgresContainer.StartAsync(),
            _clickHouseContainer.StartAsync(),
            _redisContainer.StartAsync(),
            _minioContainer.StartAsync()
        );

        // Create the langfuse bucket in MinIO
        await CreateMinioBucketAsync();
    }

    private async Task<string> GetInitUserIdAsync()
    {
        var result = await _postgresContainer!.ExecAsync(new[]
        {
            "psql", "-U", PostgresUser, "-d", PostgresDatabase, "-tAc",
            "SELECT id FROM users WHERE email = 'test@example.com'"
        });

        var userId = result.Stdout.Trim();
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException($"Init user not found in database: {result.Stderr}");
        }

        return userId;
    }

    private async Task CreateMinioBucketAsync()
    {
        // Use mc client to create bucket
        var result = await _minioContainer!.ExecAsync(new[]
        {
            "mc", "alias", "set", "local", "http://localhost:9000", MinioUser, MinioPassword
        });

        result = await _minioContainer.ExecAsync(new[]
        {
            "mc", "mb", $"local/{MinioBucket}", "--ignore-existing"
        });
    }

    private async Task StartLangfuseServicesAsync()
    {
        var langfuseEnvVars = GetLangfuseEnvironmentVariables();

        // Langfuse Worker
        _langfuseWorkerContainer = new ContainerBuilder("docker.io/langfuse/langfuse-worker:4")
            .WithNetwork(_network)
            .WithNetworkAliases("langfuse-worker")
            .WithPortBinding(3030, true)
            .WithEnvironment(langfuseEnvVars)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r.ForPath("").ForPort(3030)))
            .WithImagePullPolicy(_ => true)
            .Build();

        // Langfuse Web
        var webEnvVars = new Dictionary<string, string>(langfuseEnvVars)
        {
            ["NEXTAUTH_SECRET"] = NextAuthSecret,
            ["LANGFUSE_INIT_ORG_ID"] = "test-org",
            ["LANGFUSE_INIT_ORG_NAME"] = "Test Organization",
            ["LANGFUSE_INIT_PROJECT_ID"] = "test-project",
            ["LANGFUSE_INIT_PROJECT_NAME"] = "Test Project",
            ["LANGFUSE_INIT_PROJECT_PUBLIC_KEY"] = InitProjectPublicKey,
            ["LANGFUSE_INIT_PROJECT_SECRET_KEY"] = InitProjectSecretKey,
            ["LANGFUSE_INIT_USER_EMAIL"] = "test@example.com",
            ["LANGFUSE_INIT_USER_NAME"] = "Test User",
            ["LANGFUSE_INIT_USER_PASSWORD"] = "testpassword123"
        };

        _langfuseWebContainer = new ContainerBuilder("docker.io/langfuse/langfuse:4")
            .WithNetwork(_network)
            .WithNetworkAliases("langfuse-web")
            .WithPortBinding(3000, true)
            .WithEnvironment(webEnvVars)
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r.ForPath("/api/public/health").ForPort(3000)))
            .WithImagePullPolicy(_ => true)
            .Build();

        await _langfuseWebContainer.StartAsync();
        await _langfuseWorkerContainer.StartAsync();
    }

    private Dictionary<string, string> GetLangfuseEnvironmentVariables()
    {
        return new Dictionary<string, string>
        {
            // NextAuth
            ["NEXTAUTH_URL"] = "http://localhost:3000",

            // PostgreSQL
            ["DATABASE_URL"] = $"postgresql://{PostgresUser}:{PostgresPassword}@postgres:5432/{PostgresDatabase}",

            // Security
            ["SALT"] = Salt,
            ["ENCRYPTION_KEY"] = EncryptionKey,

            // Telemetry
            ["TELEMETRY_ENABLED"] = "false",

            // Langfuse v4 defaults to events_only mode where legacy read APIs (GET /traces/{id}, /observations/{id}, ...)
            // return 404. Dual mode keeps legacy tables populated so both legacy and v2 APIs can be tested.
            // Controlled by LANGFUSE_TEST_V4_MODE env var: "dual" (default) or "events_only".
            ["LANGFUSE_MIGRATION_V4_WRITE_MODE"] = V4Mode,
            ["LANGFUSE_MIGRATION_V4_NATIVE_OTEL_BEHAVIOUR"] = V4Mode == "dual" ? "dual_write" : "direct",
            ["LANGFUSE_ENABLE_EXPERIMENTAL_FEATURES"] = "true",

            // Code evaluators are disabled unless a dispatcher is configured; without it the evaluator and
            // evaluation rule APIs reject code evaluators with 412 "Code evaluations are not enabled for this
            // deployment.". insecure-local runs evaluator code in the worker process - fine for tests.
            ["LANGFUSE_CODE_EVAL_DISPATCHER"] = "insecure-local",
            ["QUEUE_CONSUMER_CODE_EVAL_EXECUTION_QUEUE_IS_ENABLED"] = "true",

            // ClickHouse
            ["CLICKHOUSE_MIGRATION_URL"] = "clickhouse://clickhouse:9000",
            ["CLICKHOUSE_URL"] = "http://clickhouse:8123",
            ["CLICKHOUSE_USER"] = ClickHouseUser,
            ["CLICKHOUSE_PASSWORD"] = ClickHousePassword,
            ["CLICKHOUSE_CLUSTER_ENABLED"] = "false",

            // S3/MinIO - Event Upload
            ["LANGFUSE_USE_AZURE_BLOB"] = "false",
            ["LANGFUSE_S3_EVENT_UPLOAD_BUCKET"] = MinioBucket,
            ["LANGFUSE_S3_EVENT_UPLOAD_REGION"] = "auto",
            ["LANGFUSE_S3_EVENT_UPLOAD_ACCESS_KEY_ID"] = MinioUser,
            ["LANGFUSE_S3_EVENT_UPLOAD_SECRET_ACCESS_KEY"] = MinioPassword,
            ["LANGFUSE_S3_EVENT_UPLOAD_ENDPOINT"] = "http://minio:9000",
            ["LANGFUSE_S3_EVENT_UPLOAD_FORCE_PATH_STYLE"] = "true",
            ["LANGFUSE_S3_EVENT_UPLOAD_PREFIX"] = "events/",

            // S3/MinIO - Media Upload
            ["LANGFUSE_S3_MEDIA_UPLOAD_BUCKET"] = MinioBucket,
            ["LANGFUSE_S3_MEDIA_UPLOAD_REGION"] = "auto",
            ["LANGFUSE_S3_MEDIA_UPLOAD_ACCESS_KEY_ID"] = MinioUser,
            ["LANGFUSE_S3_MEDIA_UPLOAD_SECRET_ACCESS_KEY"] = MinioPassword,
            ["LANGFUSE_S3_MEDIA_UPLOAD_ENDPOINT"] = "http://minio:9000",
            ["LANGFUSE_S3_MEDIA_UPLOAD_FORCE_PATH_STYLE"] = "true",
            ["LANGFUSE_S3_MEDIA_UPLOAD_PREFIX"] = "media/",

            // S3/MinIO - Batch Export
            ["LANGFUSE_S3_BATCH_EXPORT_ENABLED"] = "false",
            ["LANGFUSE_S3_BATCH_EXPORT_BUCKET"] = MinioBucket,
            ["LANGFUSE_S3_BATCH_EXPORT_PREFIX"] = "exports/",
            ["LANGFUSE_S3_BATCH_EXPORT_REGION"] = "auto",
            ["LANGFUSE_S3_BATCH_EXPORT_ENDPOINT"] = "http://minio:9000",
            ["LANGFUSE_S3_BATCH_EXPORT_ACCESS_KEY_ID"] = MinioUser,
            ["LANGFUSE_S3_BATCH_EXPORT_SECRET_ACCESS_KEY"] = MinioPassword,
            ["LANGFUSE_S3_BATCH_EXPORT_FORCE_PATH_STYLE"] = "true",

            // Redis
            ["REDIS_HOST"] = "redis",
            ["REDIS_PORT"] = "6379",
            ["REDIS_AUTH"] = RedisPassword,
            ["REDIS_TLS_ENABLED"] = "false",

            ["AWS_REQUEST_CHECKSUM_CALCULATION"] = "WHEN_REQUIRED",
            ["AWS_RESPONSE_CHECKSUM_VALIDATION"] = "WHEN_REQUIRED"
        };
    }
}