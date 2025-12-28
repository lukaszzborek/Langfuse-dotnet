using System.Text.Json;
using Shouldly;
using zborek.Langfuse.Models.LlmConnection;

namespace zborek.Langfuse.Tests.Models;

public class LlmConnectionTests
{
    [Fact]
    public void Should_Serialize_LlmAdapter_To_Lowercase()
    {
        var testData = new
        {
            Adapter1 = LlmAdapter.Anthropic,
            Adapter2 = LlmAdapter.OpenAi,
            Adapter3 = LlmAdapter.Azure,
            Adapter4 = LlmAdapter.Bedrock
        };

        var json = JsonSerializer.Serialize(testData);

        json.ShouldContain("\"anthropic\"");
        json.ShouldContain("\"openai\"");
        json.ShouldContain("\"azure\"");
        json.ShouldContain("\"bedrock\"");
    }

    [Fact]
    public void Should_Serialize_LlmAdapter_With_Hyphens_For_Google()
    {
        var testData = new
        {
            Adapter1 = LlmAdapter.GoogleVertexAi,
            Adapter2 = LlmAdapter.GoogleAiStudio
        };

        var json = JsonSerializer.Serialize(testData);

        json.ShouldContain("\"google-vertex-ai\"");
        json.ShouldContain("\"google-ai-studio\"");
    }

    [Fact]
    public void Should_Deserialize_LlmAdapter_From_Lowercase()
    {
        var json1 = "{\"Adapter\":\"anthropic\"}";
        var json2 = "{\"Adapter\":\"openai\"}";
        var json3 = "{\"Adapter\":\"azure\"}";
        var json4 = "{\"Adapter\":\"bedrock\"}";

        var result1 = JsonSerializer.Deserialize<TestAdapterWrapper>(json1);
        var result2 = JsonSerializer.Deserialize<TestAdapterWrapper>(json2);
        var result3 = JsonSerializer.Deserialize<TestAdapterWrapper>(json3);
        var result4 = JsonSerializer.Deserialize<TestAdapterWrapper>(json4);

        result1!.Adapter.ShouldBe(LlmAdapter.Anthropic);
        result2!.Adapter.ShouldBe(LlmAdapter.OpenAi);
        result3!.Adapter.ShouldBe(LlmAdapter.Azure);
        result4!.Adapter.ShouldBe(LlmAdapter.Bedrock);
    }

    [Fact]
    public void Should_Deserialize_LlmAdapter_From_Hyphenated_Values()
    {
        var json1 = "{\"Adapter\":\"google-vertex-ai\"}";
        var json2 = "{\"Adapter\":\"google-ai-studio\"}";

        var result1 = JsonSerializer.Deserialize<TestAdapterWrapper>(json1);
        var result2 = JsonSerializer.Deserialize<TestAdapterWrapper>(json2);

        result1!.Adapter.ShouldBe(LlmAdapter.GoogleVertexAi);
        result2!.Adapter.ShouldBe(LlmAdapter.GoogleAiStudio);
    }

    [Fact]
    public void Should_Serialize_UpsertLlmConnectionRequest_With_All_Fields()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "my-openai-gateway",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test123456",
            BaseURL = "https://api.mygateway.com",
            CustomModels = ["gpt-4-custom", "gpt-3.5-custom"],
            WithDefaultModels = true,
            ExtraHeaders = new Dictionary<string, string>
            {
                { "X-Custom-Header", "value1" },
                { "X-Auth-Token", "token123" }
            }
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"provider\"");
        json.ShouldContain("\"my-openai-gateway\"");
        json.ShouldContain("\"adapter\"");
        json.ShouldContain("\"openai\"");
        json.ShouldContain("\"secretKey\"");
        json.ShouldContain("\"sk-test123456\"");
        json.ShouldContain("\"baseURL\"");
        json.ShouldContain("\"https://api.mygateway.com\"");
        json.ShouldContain("\"customModels\"");
        json.ShouldContain("\"gpt-4-custom\"");
        json.ShouldContain("\"gpt-3.5-custom\"");
        json.ShouldContain("\"withDefaultModels\"");
        json.ShouldContain("\"extraHeaders\"");
        json.ShouldContain("\"X-Custom-Header\"");
        json.ShouldContain("\"value1\"");
    }

    [Fact]
    public void Should_Serialize_UpsertLlmConnectionRequest_With_Nullable_Fields_As_Null()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "my-provider",
            Adapter = LlmAdapter.Anthropic,
            SecretKey = "sk-ant-api03-test",
            BaseURL = null,
            CustomModels = null,
            WithDefaultModels = null,
            ExtraHeaders = null
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"provider\":\"my-provider\"");
        json.ShouldContain("\"adapter\":\"anthropic\"");
        json.ShouldContain("\"secretKey\":\"sk-ant-api03-test\"");
        json.ShouldContain("\"baseURL\":null");
        json.ShouldContain("\"customModels\":null");
        json.ShouldContain("\"withDefaultModels\":null");
        json.ShouldContain("\"extraHeaders\":null");
    }

    [Fact]
    public void Should_Deserialize_LlmConnection_Response()
    {
        var json = @"{
            ""id"": ""conn-123"",
            ""provider"": ""openai-prod"",
            ""adapter"": ""openai"",
            ""displaySecretKey"": ""sk-...3456"",
            ""baseURL"": ""https://api.openai.com"",
            ""customModels"": [""gpt-4-turbo"", ""gpt-4-vision""],
            ""withDefaultModels"": true,
            ""extraHeaderKeys"": [""X-Custom-Header"", ""X-Rate-Limit""],
            ""createdAt"": ""2024-10-01T10:00:00Z"",
            ""updatedAt"": ""2024-10-12T15:30:00Z""
        }";

        var response = JsonSerializer.Deserialize<LlmConnection>(json);

        response.ShouldNotBeNull();
        response.Id.ShouldBe("conn-123");
        response.Provider.ShouldBe("openai-prod");
        response.Adapter.ShouldBe(LlmAdapter.OpenAi);
        response.DisplaySecretKey.ShouldBe("sk-...3456");
        response.BaseURL.ShouldBe("https://api.openai.com");
        response.CustomModels.Length.ShouldBe(2);
        response.CustomModels.ShouldContain("gpt-4-turbo");
        response.CustomModels.ShouldContain("gpt-4-vision");
        response.WithDefaultModels.ShouldBeTrue();
        response.ExtraHeaderKeys.Length.ShouldBe(2);
        response.ExtraHeaderKeys.ShouldContain("X-Custom-Header");
        response.ExtraHeaderKeys.ShouldContain("X-Rate-Limit");
        response.CreatedAt.ShouldBe(new DateTime(2024, 10, 1, 10, 0, 0, DateTimeKind.Utc));
        response.UpdatedAt.ShouldBe(new DateTime(2024, 10, 12, 15, 30, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Should_Deserialize_PaginatedLlmConnections_With_Array_And_Meta()
    {
        var json = @"{
            ""data"": [
                {
                    ""id"": ""conn-1"",
                    ""provider"": ""openai"",
                    ""adapter"": ""openai"",
                    ""displaySecretKey"": ""sk-...1234"",
                    ""baseURL"": null,
                    ""customModels"": [],
                    ""withDefaultModels"": true,
                    ""extraHeaderKeys"": [],
                    ""createdAt"": ""2024-01-01T00:00:00Z"",
                    ""updatedAt"": ""2024-01-01T00:00:00Z""
                },
                {
                    ""id"": ""conn-2"",
                    ""provider"": ""anthropic"",
                    ""adapter"": ""anthropic"",
                    ""displaySecretKey"": ""sk-ant-...5678"",
                    ""baseURL"": ""https://api.anthropic.com"",
                    ""customModels"": [""claude-custom""],
                    ""withDefaultModels"": false,
                    ""extraHeaderKeys"": [""X-API-Version""],
                    ""createdAt"": ""2024-02-01T00:00:00Z"",
                    ""updatedAt"": ""2024-02-15T00:00:00Z""
                },
                {
                    ""id"": ""conn-3"",
                    ""provider"": ""google-vertex"",
                    ""adapter"": ""google-vertex-ai"",
                    ""displaySecretKey"": ""***"",
                    ""baseURL"": null,
                    ""customModels"": [""gemini-pro"", ""gemini-ultra""],
                    ""withDefaultModels"": true,
                    ""extraHeaderKeys"": [],
                    ""createdAt"": ""2024-03-01T00:00:00Z"",
                    ""updatedAt"": ""2024-03-01T00:00:00Z""
                }
            ],
            ""meta"": {
                ""page"": 1,
                ""limit"": 50,
                ""totalItems"": 3,
                ""totalPages"": 1
            }
        }";

        var response = JsonSerializer.Deserialize<PaginatedLlmConnections>(json);

        response.ShouldNotBeNull();
        response.Data.ShouldNotBeNull();
        response.Data.Length.ShouldBe(3);

        response.Data[0].Id.ShouldBe("conn-1");
        response.Data[0].Provider.ShouldBe("openai");
        response.Data[0].Adapter.ShouldBe(LlmAdapter.OpenAi);

        response.Data[1].Id.ShouldBe("conn-2");
        response.Data[1].Provider.ShouldBe("anthropic");
        response.Data[1].Adapter.ShouldBe(LlmAdapter.Anthropic);
        response.Data[1].CustomModels.ShouldHaveSingleItem();

        response.Data[2].Id.ShouldBe("conn-3");
        response.Data[2].Provider.ShouldBe("google-vertex");
        response.Data[2].Adapter.ShouldBe(LlmAdapter.GoogleVertexAi);
        response.Data[2].CustomModels.Length.ShouldBe(2);

        response.Meta.ShouldNotBeNull();
        response.Meta.Page.ShouldBe(1);
        response.Meta.Limit.ShouldBe(50);
        response.Meta.TotalItems.ShouldBe(3);
        response.Meta.TotalPages.ShouldBe(1);
    }

    [Fact]
    public void LlmConnection_Should_Not_Have_SecretKey_Property()
    {
        var connectionType = typeof(LlmConnection);
        var secretKeyProperty = connectionType.GetProperty("SecretKey");

        secretKeyProperty.ShouldBeNull();
    }

    [Fact]
    public void LlmConnection_Should_Have_DisplaySecretKey_Property()
    {
        var connectionType = typeof(LlmConnection);
        var displaySecretKeyProperty = connectionType.GetProperty("DisplaySecretKey");

        displaySecretKeyProperty.ShouldNotBeNull();
        displaySecretKeyProperty.PropertyType.ShouldBe(typeof(string));
    }

    [Fact]
    public void UpsertLlmConnectionRequest_Should_Have_SecretKey_Property()
    {
        var requestType = typeof(UpsertLlmConnectionRequest);
        var secretKeyProperty = requestType.GetProperty("SecretKey");

        secretKeyProperty.ShouldNotBeNull();
        secretKeyProperty.PropertyType.ShouldBe(typeof(string));
    }

    [Fact]
    public void UpsertLlmConnectionRequest_Should_Not_Have_DisplaySecretKey_Property()
    {
        var requestType = typeof(UpsertLlmConnectionRequest);
        var displaySecretKeyProperty = requestType.GetProperty("DisplaySecretKey");

        displaySecretKeyProperty.ShouldBeNull();
    }

    [Fact]
    public void Should_Deserialize_LlmConnection_With_Null_Optional_Fields()
    {
        var json = @"{
            ""id"": ""conn-minimal"",
            ""provider"": ""minimal-provider"",
            ""adapter"": ""azure"",
            ""displaySecretKey"": ""***"",
            ""baseURL"": null,
            ""customModels"": [],
            ""withDefaultModels"": false,
            ""extraHeaderKeys"": [],
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<LlmConnection>(json);

        response.ShouldNotBeNull();
        response.Id.ShouldBe("conn-minimal");
        response.BaseURL.ShouldBeNull();
        response.CustomModels.ShouldBeEmpty();
        response.WithDefaultModels.ShouldBeFalse();
        response.ExtraHeaderKeys.ShouldBeEmpty();
    }

    [Fact]
    public void Should_Handle_ExtraHeaders_In_Request_And_ExtraHeaderKeys_In_Response()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.Bedrock,
            SecretKey = "secret",
            ExtraHeaders = new Dictionary<string, string>
            {
                { "X-Header-1", "value1" },
                { "X-Header-2", "value2" }
            }
        };

        var requestJson = JsonSerializer.Serialize(request);
        requestJson.ShouldContain("\"extraHeaders\"");
        requestJson.ShouldContain("\"X-Header-1\"");
        requestJson.ShouldContain("\"value1\"");

        var responseJson = @"{
            ""id"": ""conn-test"",
            ""provider"": ""test-provider"",
            ""adapter"": ""bedrock"",
            ""displaySecretKey"": ""***"",
            ""customModels"": [],
            ""withDefaultModels"": true,
            ""extraHeaderKeys"": [""X-Header-1"", ""X-Header-2""],
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<LlmConnection>(responseJson);
        response.ShouldNotBeNull();
        response.ExtraHeaderKeys.Length.ShouldBe(2);
        response.ExtraHeaderKeys.ShouldContain("X-Header-1");
        response.ExtraHeaderKeys.ShouldContain("X-Header-2");
    }

    [Fact]
    public void Should_Serialize_All_LlmAdapter_Values()
    {
        var allAdapters = new
        {
            LlmAdapter.Anthropic,
            LlmAdapter.OpenAi,
            LlmAdapter.Azure,
            LlmAdapter.Bedrock,
            LlmAdapter.GoogleVertexAi,
            LlmAdapter.GoogleAiStudio
        };

        var json = JsonSerializer.Serialize(allAdapters);

        json.ShouldContain("\"anthropic\"");
        json.ShouldContain("\"openai\"");
        json.ShouldContain("\"azure\"");
        json.ShouldContain("\"bedrock\"");
        json.ShouldContain("\"google-vertex-ai\"");
        json.ShouldContain("\"google-ai-studio\"");
    }

    [Theory]
    [InlineData(LlmAdapter.Anthropic, "anthropic")]
    [InlineData(LlmAdapter.OpenAi, "openai")]
    [InlineData(LlmAdapter.Azure, "azure")]
    [InlineData(LlmAdapter.Bedrock, "bedrock")]
    [InlineData(LlmAdapter.GoogleVertexAi, "google-vertex-ai")]
    [InlineData(LlmAdapter.GoogleAiStudio, "google-ai-studio")]
    public void LlmAdapter_SerializesTo_LowercaseWithHyphens(LlmAdapter adapter, string expectedJson)
    {
        var wrapper = new TestAdapterWrapper { Adapter = adapter };
        var json = JsonSerializer.Serialize(wrapper);

        json.ShouldContain($"\"{expectedJson}\"");
    }

    [Theory]
    [InlineData("anthropic", LlmAdapter.Anthropic)]
    [InlineData("openai", LlmAdapter.OpenAi)]
    [InlineData("azure", LlmAdapter.Azure)]
    [InlineData("bedrock", LlmAdapter.Bedrock)]
    [InlineData("google-vertex-ai", LlmAdapter.GoogleVertexAi)]
    [InlineData("google-ai-studio", LlmAdapter.GoogleAiStudio)]
    public void LlmAdapter_DeserializesFrom_LowercaseWithHyphens(string jsonValue, LlmAdapter expectedAdapter)
    {
        var json = $"{{\"Adapter\":\"{jsonValue}\"}}";
        var result = JsonSerializer.Deserialize<TestAdapterWrapper>(json);

        result.ShouldNotBeNull();
        result.Adapter.ShouldBe(expectedAdapter);
    }

    [Fact]
    public void Should_Serialize_CustomModels_As_EmptyArray()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test",
            CustomModels = Array.Empty<string>()
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"customModels\":[]");
    }

    [Fact]
    public void Should_Serialize_WithDefaultModels_True_With_CustomModels()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.Anthropic,
            SecretKey = "sk-test",
            CustomModels = new[] { "claude-custom" },
            WithDefaultModels = true
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"customModels\"");
        json.ShouldContain("\"claude-custom\"");
        json.ShouldContain("\"withDefaultModels\":true");
    }

    [Fact]
    public void Should_Serialize_ExtraHeaders_As_EmptyDictionary()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.Bedrock,
            SecretKey = "sk-test",
            ExtraHeaders = new Dictionary<string, string>()
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"extraHeaders\":{}");
    }

    [Fact]
    public void Should_Deserialize_ExtraHeaderKeys_As_EmptyArray()
    {
        var json = @"{
            ""id"": ""conn-test"",
            ""provider"": ""test"",
            ""adapter"": ""openai"",
            ""displaySecretKey"": ""***"",
            ""customModels"": [],
            ""withDefaultModels"": true,
            ""extraHeaderKeys"": [],
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<LlmConnection>(json);

        response.ShouldNotBeNull();
        response.ExtraHeaderKeys.ShouldBeEmpty();
    }

    [Fact]
    public void Should_Serialize_BaseURL_As_Null()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test",
            BaseURL = null
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"baseURL\":null");
    }

    [Fact]
    public void Should_Serialize_BaseURL_With_CustomUrl()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.Azure,
            SecretKey = "sk-test",
            BaseURL = "https://custom.openai.azure.com"
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"baseURL\":\"https://custom.openai.azure.com\"");
    }

    [Fact]
    public void Should_Deserialize_LlmConnection_With_Null_BaseURL()
    {
        var json = @"{
            ""id"": ""conn-test"",
            ""provider"": ""test"",
            ""adapter"": ""openai"",
            ""displaySecretKey"": ""***"",
            ""baseURL"": null,
            ""customModels"": [],
            ""withDefaultModels"": true,
            ""extraHeaderKeys"": [],
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<LlmConnection>(json);

        response.ShouldNotBeNull();
        response.BaseURL.ShouldBeNull();
    }

    [Fact]
    public void Should_Deserialize_DisplaySecretKey_With_MaskedFormat()
    {
        var json = @"{
            ""id"": ""conn-test"",
            ""provider"": ""test"",
            ""adapter"": ""anthropic"",
            ""displaySecretKey"": ""sk-ant-***def"",
            ""customModels"": [],
            ""withDefaultModels"": true,
            ""extraHeaderKeys"": [],
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<LlmConnection>(json);

        response.ShouldNotBeNull();
        response.DisplaySecretKey.ShouldBe("sk-ant-***def");
        response.DisplaySecretKey.ShouldContain("***");
    }

    [Fact]
    public void Should_Deserialize_PaginatedLlmConnections_With_EmptyData()
    {
        var json = @"{
            ""data"": [],
            ""meta"": {
                ""page"": 1,
                ""limit"": 50,
                ""totalItems"": 0,
                ""totalPages"": 0
            }
        }";

        var response = JsonSerializer.Deserialize<PaginatedLlmConnections>(json);

        response.ShouldNotBeNull();
        response.Data.ShouldNotBeNull();
        response.Data.ShouldBeEmpty();
        response.Meta.TotalItems.ShouldBe(0);
        response.Meta.TotalPages.ShouldBe(0);
    }

    [Fact]
    public void Should_Serialize_Request_With_Multiple_CustomModels()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.OpenAi,
            SecretKey = "sk-test",
            CustomModels = new[] { "gpt-4-turbo", "gpt-4-vision", "gpt-4-32k", "gpt-3.5-turbo-16k" }
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"customModels\"");
        json.ShouldContain("\"gpt-4-turbo\"");
        json.ShouldContain("\"gpt-4-vision\"");
        json.ShouldContain("\"gpt-4-32k\"");
        json.ShouldContain("\"gpt-3.5-turbo-16k\"");
    }

    [Fact]
    public void Should_Serialize_Request_With_Multiple_ExtraHeaders()
    {
        var request = new UpsertLlmConnectionRequest
        {
            Provider = "test-provider",
            Adapter = LlmAdapter.GoogleVertexAi,
            SecretKey = "sk-test",
            ExtraHeaders = new Dictionary<string, string>
            {
                { "X-API-Version", "2024-01-01" },
                { "X-Custom-Auth", "bearer-token" },
                { "X-Request-ID", "req-123" },
                { "X-Client-Name", "langfuse-csharp" }
            }
        };

        var json = JsonSerializer.Serialize(request);

        json.ShouldContain("\"extraHeaders\"");
        json.ShouldContain("\"X-API-Version\"");
        json.ShouldContain("\"X-Custom-Auth\"");
        json.ShouldContain("\"X-Request-ID\"");
        json.ShouldContain("\"X-Client-Name\"");
    }

    [Fact]
    public void Should_Deserialize_Response_With_Multiple_ExtraHeaderKeys()
    {
        var json = @"{
            ""id"": ""conn-test"",
            ""provider"": ""test"",
            ""adapter"": ""google-ai-studio"",
            ""displaySecretKey"": ""***"",
            ""customModels"": [],
            ""withDefaultModels"": true,
            ""extraHeaderKeys"": [""X-Header-1"", ""X-Header-2"", ""X-Header-3"", ""X-Header-4""],
            ""createdAt"": ""2024-01-01T00:00:00Z"",
            ""updatedAt"": ""2024-01-01T00:00:00Z""
        }";

        var response = JsonSerializer.Deserialize<LlmConnection>(json);

        response.ShouldNotBeNull();
        response.ExtraHeaderKeys.Length.ShouldBe(4);
        response.ExtraHeaderKeys.ShouldContain("X-Header-1");
        response.ExtraHeaderKeys.ShouldContain("X-Header-2");
        response.ExtraHeaderKeys.ShouldContain("X-Header-3");
        response.ExtraHeaderKeys.ShouldContain("X-Header-4");
    }

    private class TestAdapterWrapper
    {
        public LlmAdapter Adapter { get; set; }
    }
}