using Microsoft.Extensions.Configuration;
using Shouldly;
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
        options.Url.ShouldBe("https://cloud.langfuse.com");
        options.PublicKey.ShouldBe(string.Empty);
        options.SecretKey.ShouldBe(string.Empty);
        options.Headers.ShouldNotBeNull();
        options.Headers.ShouldBeEmpty();
        options.TimeoutMilliseconds.ShouldBe(10000);
    }

    [Fact]
    public void Endpoint_CanBeSet()
    {
        // Arrange
        var options = new LangfuseOtlpExporterOptions();
        var customEndpoint = "https://custom.langfuse.com";

        // Act
        options.Url = customEndpoint;

        // Assert
        options.Url.ShouldBe(customEndpoint);
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
        options.PublicKey.ShouldBe(publicKey);
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
        options.SecretKey.ShouldBe(secretKey);
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
        options.Headers.ShouldBe(headers);
        options.Headers.Count.ShouldBe(2);
        options.Headers["X-Custom-Header"].ShouldBe("CustomValue");
        options.Headers["X-Another-Header"].ShouldBe("AnotherValue");
    }

    [Fact]
    public void Headers_CanBeModified()
    {
        // Arrange
        var options = new LangfuseOtlpExporterOptions();

        // Act
        options.Headers.Add("X-Test-Header", "TestValue");

        // Assert
        options.Headers.ShouldHaveSingleItem();
        options.Headers["X-Test-Header"].ShouldBe("TestValue");
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
        options.TimeoutMilliseconds.ShouldBe(customTimeout);
    }

    [Fact]
    public void Configuration_CanBindToOptions()
    {
        // Arrange
        var configurationData = new Dictionary<string, string>
        {
            { "LangfuseOtlp:EnableOpenTelemetryExporter", "false" },
            { "LangfuseOtlp:Url", "https://test.langfuse.com" },
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
        options.EnableOpenTelemetryExporter.ShouldBe(false);
        options.Url.ShouldBe("https://test.langfuse.com");
        options.PublicKey.ShouldBe("pk-test-key");
        options.SecretKey.ShouldBe("sk-test-key");
        options.TimeoutMilliseconds.ShouldBe(15000);
        options.Headers.ShouldHaveSingleItem();
        options.Headers["X-Custom"].ShouldBe("CustomValue");
    }

    [Fact]
    public void Configuration_CanBindToOptions_EnabledExporter()
    {
        // Arrange
        var configurationData = new Dictionary<string, string>
        {
            { "LangfuseOtlp:EnableOpenTelemetryExporter", "true" },
            { "LangfuseOtlp:Url", "https://test.langfuse.com" },
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
        options.EnableOpenTelemetryExporter.ShouldBe(true);
        options.Url.ShouldBe("https://test.langfuse.com");
        options.PublicKey.ShouldBe("pk-test-key");
        options.SecretKey.ShouldBe("sk-test-key");
        options.TimeoutMilliseconds.ShouldBe(15000);
        options.Headers.ShouldHaveSingleItem();
        options.Headers["X-Custom"].ShouldBe("CustomValue");
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
        options.Url.ShouldBe("https://cloud.langfuse.com");
        options.PublicKey.ShouldBe("pk-test-key");
        options.SecretKey.ShouldBe(string.Empty);
        options.TimeoutMilliseconds.ShouldBe(10000);
        options.Headers.ShouldBeEmpty();
    }

    [Fact]
    public void AllProperties_CanBeSetTogether()
    {
        // Arrange & Act
        var options = new LangfuseOtlpExporterOptions
        {
            Url = "https://custom.langfuse.com",
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
        options.Url.ShouldBe("https://custom.langfuse.com");
        options.PublicKey.ShouldBe("pk-custom-key");
        options.SecretKey.ShouldBe("sk-custom-key");
        options.TimeoutMilliseconds.ShouldBe(20000);
        options.Headers.Count.ShouldBe(2);
    }
}