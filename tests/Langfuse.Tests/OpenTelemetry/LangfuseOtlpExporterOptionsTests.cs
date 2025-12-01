using Microsoft.Extensions.Configuration;
using zborek.Langfuse.OpenTelemetry;

namespace zborek.Langfuse.Tests.OpenTelemetry;

public class LangfuseOtlpExporterOptionsTests
{
    [Fact]
    public void Constructor_InitializesDefaultValues()
    {
        // Act
        var options = new LangfuseOtlpExporterOptions();

        // Assert
        Assert.Equal("https://cloud.langfuse.com", options.Endpoint);
        Assert.Equal(string.Empty, options.PublicKey);
        Assert.Equal(string.Empty, options.SecretKey);
        Assert.NotNull(options.Headers);
        Assert.Empty(options.Headers);
        Assert.Equal(10000, options.TimeoutMilliseconds);
    }

    [Fact]
    public void Endpoint_CanBeSet()
    {
        // Arrange
        var options = new LangfuseOtlpExporterOptions();
        var customEndpoint = "https://custom.langfuse.com";

        // Act
        options.Endpoint = customEndpoint;

        // Assert
        Assert.Equal(customEndpoint, options.Endpoint);
    }

    [Fact]
    public void PublicKey_CanBeSet()
    {
        // Arrange
        var options = new LangfuseOtlpExporterOptions();
        var publicKey = "pk-test-12345";

        // Act
        options.PublicKey = publicKey;

        // Assert
        Assert.Equal(publicKey, options.PublicKey);
    }

    [Fact]
    public void SecretKey_CanBeSet()
    {
        // Arrange
        var options = new LangfuseOtlpExporterOptions();
        var secretKey = "sk-test-67890";

        // Act
        options.SecretKey = secretKey;

        // Assert
        Assert.Equal(secretKey, options.SecretKey);
    }

    [Fact]
    public void Headers_CanBeSet()
    {
        // Arrange
        var options = new LangfuseOtlpExporterOptions();
        var headers = new Dictionary<string, string>
        {
            { "X-Custom-Header", "CustomValue" },
            { "X-Another-Header", "AnotherValue" }
        };

        // Act
        options.Headers = headers;

        // Assert
        Assert.Equal(headers, options.Headers);
        Assert.Equal(2, options.Headers.Count);
        Assert.Equal("CustomValue", options.Headers["X-Custom-Header"]);
        Assert.Equal("AnotherValue", options.Headers["X-Another-Header"]);
    }

    [Fact]
    public void Headers_CanBeModified()
    {
        // Arrange
        var options = new LangfuseOtlpExporterOptions();

        // Act
        options.Headers.Add("X-Test-Header", "TestValue");

        // Assert
        Assert.Single(options.Headers);
        Assert.Equal("TestValue", options.Headers["X-Test-Header"]);
    }

    [Fact]
    public void TimeoutMilliseconds_CanBeSet()
    {
        // Arrange
        var options = new LangfuseOtlpExporterOptions();
        var customTimeout = 5000;

        // Act
        options.TimeoutMilliseconds = customTimeout;

        // Assert
        Assert.Equal(customTimeout, options.TimeoutMilliseconds);
    }

    [Fact]
    public void Configuration_CanBindToOptions()
    {
        // Arrange
        var configurationData = new Dictionary<string, string>
        {
            { "LangfuseOtlp:Endpoint", "https://test.langfuse.com" },
            { "LangfuseOtlp:PublicKey", "pk-test-key" },
            { "LangfuseOtlp:SecretKey", "sk-test-key" },
            { "LangfuseOtlp:TimeoutMilliseconds", "15000" },
            { "LangfuseOtlp:Headers:X-Custom", "CustomValue" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData!)
            .Build();

        // Act
        var options = new LangfuseOtlpExporterOptions();
        configuration.GetSection("LangfuseOtlp").Bind(options);

        // Assert
        Assert.Equal("https://test.langfuse.com", options.Endpoint);
        Assert.Equal("pk-test-key", options.PublicKey);
        Assert.Equal("sk-test-key", options.SecretKey);
        Assert.Equal(15000, options.TimeoutMilliseconds);
        Assert.Single(options.Headers);
        Assert.Equal("CustomValue", options.Headers["X-Custom"]);
    }

    [Fact]
    public void Configuration_WithMissingValues_UsesDefaults()
    {
        // Arrange
        var configurationData = new Dictionary<string, string>
        {
            { "LangfuseOtlp:PublicKey", "pk-test-key" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData!)
            .Build();

        // Act
        var options = new LangfuseOtlpExporterOptions();
        configuration.GetSection("LangfuseOtlp").Bind(options);

        // Assert
        Assert.Equal("https://cloud.langfuse.com", options.Endpoint);
        Assert.Equal("pk-test-key", options.PublicKey);
        Assert.Equal(string.Empty, options.SecretKey);
        Assert.Equal(10000, options.TimeoutMilliseconds);
        Assert.Empty(options.Headers);
    }

    [Fact]
    public void AllProperties_CanBeSetTogether()
    {
        // Arrange & Act
        var options = new LangfuseOtlpExporterOptions
        {
            Endpoint = "https://custom.langfuse.com",
            PublicKey = "pk-custom-key",
            SecretKey = "sk-custom-key",
            TimeoutMilliseconds = 20000,
            Headers = new Dictionary<string, string>
            {
                { "X-Header-1", "Value1" },
                { "X-Header-2", "Value2" }
            }
        };

        // Assert
        Assert.Equal("https://custom.langfuse.com", options.Endpoint);
        Assert.Equal("pk-custom-key", options.PublicKey);
        Assert.Equal("sk-custom-key", options.SecretKey);
        Assert.Equal(20000, options.TimeoutMilliseconds);
        Assert.Equal(2, options.Headers.Count);
    }
}
