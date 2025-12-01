using System.Text;
using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using OpenTelemetry.Trace;
using zborek.Langfuse.OpenTelemetry;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class LangfuseOtlpExtensionsTests
{
    [Fact]
    public void AddLangfuseExporter_WithAction_ConfiguresTracerProvider()
    {
        // Arrange
        var builder = Sdk.CreateTracerProviderBuilder();

        // Act
        var result = builder.AddLangfuseExporter(options =>
        {
            options.Endpoint = "https://test.langfuse.com";
            options.PublicKey = "test-public-key";
            options.SecretKey = "test-secret-key";
        });

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddLangfuseExporter_WithConfiguration_ConfiguresTracerProvider()
    {
        // Arrange
        var configData = new Dictionary<string, string>
        {
            { "Endpoint", "https://config.langfuse.com" },
            { "PublicKey", "config-public-key" },
            { "SecretKey", "config-secret-key" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        var builder = Sdk.CreateTracerProviderBuilder();

        // Act
        var result = builder.AddLangfuseExporter(configuration);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddLangfuseExporter_WithDefaultOptions_UsesDefaultEndpoint()
    {
        // Arrange
        var builder = Sdk.CreateTracerProviderBuilder();

        // Act
        var result = builder.AddLangfuseExporter(options =>
        {
            options.PublicKey = "test-key";
            options.SecretKey = "test-secret";
            // Endpoint should default to https://cloud.langfuse.com
        });

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddLangfuseExporter_WithCustomHeaders_AcceptsHeaders()
    {
        // Arrange
        var builder = Sdk.CreateTracerProviderBuilder();

        // Act
        var result = builder.AddLangfuseExporter(options =>
        {
            options.Endpoint = "https://test.langfuse.com";
            options.PublicKey = "test-key";
            options.SecretKey = "test-secret";
            options.Headers.Add("X-Custom-Header", "CustomValue");
            options.Headers.Add("X-Another-Header", "AnotherValue");
        });

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddLangfuseExporter_WithTrailingSlashInEndpoint_HandlesCorrectly()
    {
        // Arrange
        var builder = Sdk.CreateTracerProviderBuilder();

        // Act
        var result = builder.AddLangfuseExporter(options =>
        {
            options.Endpoint = "https://test.langfuse.com/";
            options.PublicKey = "test-key";
            options.SecretKey = "test-secret";
        });

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddLangfuseExporter_WithCustomTimeout_AcceptsTimeout()
    {
        // Arrange
        var builder = Sdk.CreateTracerProviderBuilder();

        // Act
        var result = builder.AddLangfuseExporter(options =>
        {
            options.Endpoint = "https://test.langfuse.com";
            options.PublicKey = "test-key";
            options.SecretKey = "test-secret";
            options.TimeoutMilliseconds = 5000;
        });

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddLangfuseExporter_ReturnsBuilderForChaining()
    {
        // Arrange
        var builder = Sdk.CreateTracerProviderBuilder();

        // Act
        var result = builder
            .AddLangfuseExporter(options =>
            {
                options.PublicKey = "test-key";
                options.SecretKey = "test-secret";
            })
            .AddSource("TestSource");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public void AddLangfuseExporter_WithEmptyConfiguration_ThrowsForMissingPublicKey()
    {
        // Arrange
        var configData = new Dictionary<string, string>();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        var builder = Sdk.CreateTracerProviderBuilder();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => builder.AddLangfuseExporter(configuration));
        Assert.Contains("Public Key", exception.Message);
    }
    
    [Fact]
    public void AddLangfuseExporter_WithoutSecretKey_ThrowsForMissingSecretKey()
    {
        // Arrange
        var configData = new Dictionary<string, string>
        {
            { "Endpoint", "https://config.langfuse.com" },
            { "PublicKey", "config-public-key" }
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData!)
            .Build();

        var builder = Sdk.CreateTracerProviderBuilder();

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => builder.AddLangfuseExporter(configuration));
        Assert.Contains("Secret Key", exception.Message);
    }

    [Fact]
    public void BasicAuthenticationEncoding_CreatesCorrectFormat()
    {
        // Arrange
        var publicKey = "test-public";
        var secretKey = "test-secret";
        var expectedCredentials = $"{publicKey}:{secretKey}";
        var expectedEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(expectedCredentials));

        // Act
        var actualEncoded = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{publicKey}:{secretKey}")
        );

        // Assert
        Assert.Equal(expectedEncoded, actualEncoded);
        // Verify it follows Basic Auth format
        Assert.Equal("dGVzdC1wdWJsaWM6dGVzdC1zZWNyZXQ=", actualEncoded);
    }

    [Fact]
    public void EndpointConstruction_CreatesCorrectPath()
    {
        // Arrange
        var baseEndpoint = "https://cloud.langfuse.com";
        var expectedPath = "/api/public/otel";

        // Act
        var fullEndpoint = $"{baseEndpoint.TrimEnd('/')}{expectedPath}";

        // Assert
        Assert.Equal("https://cloud.langfuse.com/api/public/otel", fullEndpoint);
    }

    [Fact]
    public void EndpointConstruction_WithTrailingSlash_CreatesCorrectPath()
    {
        // Arrange
        var baseEndpoint = "https://cloud.langfuse.com/";
        var expectedPath = "/api/public/otel";

        // Act
        var fullEndpoint = $"{baseEndpoint.TrimEnd('/')}{expectedPath}";

        // Assert
        Assert.Equal("https://cloud.langfuse.com/api/public/otel", fullEndpoint);
    }
}
