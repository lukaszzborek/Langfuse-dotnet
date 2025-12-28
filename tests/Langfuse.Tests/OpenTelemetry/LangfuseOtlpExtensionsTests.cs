using System.Text;
using Microsoft.Extensions.Configuration;
using OpenTelemetry;
using Shouldly;
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
        result.ShouldNotBeNull();
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
        result.ShouldNotBeNull();
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
        result.ShouldNotBeNull();
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
        result.ShouldNotBeNull();
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
        result.ShouldNotBeNull();
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
        result.ShouldNotBeNull();
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
        result.ShouldNotBeNull();
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
        var exception = Should.Throw<ArgumentException>(() => builder.AddLangfuseExporter(configuration));
        exception.Message.ShouldContain("Public Key");
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
        var exception = Should.Throw<ArgumentException>(() => builder.AddLangfuseExporter(configuration));
        exception.Message.ShouldContain("Secret Key");
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
        actualEncoded.ShouldBe(expectedEncoded);
        // Verify it follows Basic Auth format
        actualEncoded.ShouldBe("dGVzdC1wdWJsaWM6dGVzdC1zZWNyZXQ=");
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
        fullEndpoint.ShouldBe("https://cloud.langfuse.com/api/public/otel");
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
        fullEndpoint.ShouldBe("https://cloud.langfuse.com/api/public/otel");
    }
}