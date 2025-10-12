using System.Text.Json;
using zborek.Langfuse.Models.Core;
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

        Assert.Contains("\"anthropic\"", json);
        Assert.Contains("\"openai\"", json);
        Assert.Contains("\"azure\"", json);
        Assert.Contains("\"bedrock\"", json);
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

        Assert.Contains("\"google-vertex-ai\"", json);
        Assert.Contains("\"google-ai-studio\"", json);
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

        Assert.Equal(LlmAdapter.Anthropic, result1!.Adapter);
        Assert.Equal(LlmAdapter.OpenAi, result2!.Adapter);
        Assert.Equal(LlmAdapter.Azure, result3!.Adapter);
        Assert.Equal(LlmAdapter.Bedrock, result4!.Adapter);
    }

    [Fact]
    public void Should_Deserialize_LlmAdapter_From_Hyphenated_Values()
    {
        var json1 = "{\"Adapter\":\"google-vertex-ai\"}";
        var json2 = "{\"Adapter\":\"google-ai-studio\"}";

        var result1 = JsonSerializer.Deserialize<TestAdapterWrapper>(json1);
        var result2 = JsonSerializer.Deserialize<TestAdapterWrapper>(json2);

        Assert.Equal(LlmAdapter.GoogleVertexAi, result1!.Adapter);
        Assert.Equal(LlmAdapter.GoogleAiStudio, result2!.Adapter);
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

        Assert.Contains("\"provider\"", json);
        Assert.Contains("\"my-openai-gateway\"", json);
        Assert.Contains("\"adapter\"", json);
        Assert.Contains("\"openai\"", json);
        Assert.Contains("\"secretKey\"", json);
        Assert.Contains("\"sk-test123456\"", json);
        Assert.Contains("\"baseURL\"", json);
        Assert.Contains("\"https://api.mygateway.com\"", json);
        Assert.Contains("\"customModels\"", json);
        Assert.Contains("\"gpt-4-custom\"", json);
        Assert.Contains("\"gpt-3.5-custom\"", json);
        Assert.Contains("\"withDefaultModels\"", json);
        Assert.Contains("\"extraHeaders\"", json);
        Assert.Contains("\"X-Custom-Header\"", json);
        Assert.Contains("\"value1\"", json);
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

        Assert.Contains("\"provider\":\"my-provider\"", json);
        Assert.Contains("\"adapter\":\"anthropic\"", json);
        Assert.Contains("\"secretKey\":\"sk-ant-api03-test\"", json);
        Assert.Contains("\"baseURL\":null", json);
        Assert.Contains("\"customModels\":null", json);
        Assert.Contains("\"withDefaultModels\":null", json);
        Assert.Contains("\"extraHeaders\":null", json);
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

        Assert.NotNull(response);
        Assert.Equal("conn-123", response.Id);
        Assert.Equal("openai-prod", response.Provider);
        Assert.Equal(LlmAdapter.OpenAi, response.Adapter);
        Assert.Equal("sk-...3456", response.DisplaySecretKey);
        Assert.Equal("https://api.openai.com", response.BaseURL);
        Assert.Equal(2, response.CustomModels.Length);
        Assert.Contains("gpt-4-turbo", response.CustomModels);
        Assert.Contains("gpt-4-vision", response.CustomModels);
        Assert.True(response.WithDefaultModels);
        Assert.Equal(2, response.ExtraHeaderKeys.Length);
        Assert.Contains("X-Custom-Header", response.ExtraHeaderKeys);
        Assert.Contains("X-Rate-Limit", response.ExtraHeaderKeys);
        Assert.Equal(new DateTime(2024, 10, 1, 10, 0, 0, DateTimeKind.Utc), response.CreatedAt);
        Assert.Equal(new DateTime(2024, 10, 12, 15, 30, 0, DateTimeKind.Utc), response.UpdatedAt);
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

        Assert.NotNull(response);
        Assert.NotNull(response.Data);
        Assert.Equal(3, response.Data.Length);

        Assert.Equal("conn-1", response.Data[0].Id);
        Assert.Equal("openai", response.Data[0].Provider);
        Assert.Equal(LlmAdapter.OpenAi, response.Data[0].Adapter);

        Assert.Equal("conn-2", response.Data[1].Id);
        Assert.Equal("anthropic", response.Data[1].Provider);
        Assert.Equal(LlmAdapter.Anthropic, response.Data[1].Adapter);
        Assert.Single(response.Data[1].CustomModels);

        Assert.Equal("conn-3", response.Data[2].Id);
        Assert.Equal("google-vertex", response.Data[2].Provider);
        Assert.Equal(LlmAdapter.GoogleVertexAi, response.Data[2].Adapter);
        Assert.Equal(2, response.Data[2].CustomModels.Length);

        Assert.NotNull(response.Meta);
        Assert.Equal(1, response.Meta.Page);
        Assert.Equal(50, response.Meta.Limit);
        Assert.Equal(3, response.Meta.TotalItems);
        Assert.Equal(1, response.Meta.TotalPages);
    }

    [Fact]
    public void LlmConnection_Should_Not_Have_SecretKey_Property()
    {
        var connectionType = typeof(LlmConnection);
        var secretKeyProperty = connectionType.GetProperty("SecretKey");

        Assert.Null(secretKeyProperty);
    }

    [Fact]
    public void LlmConnection_Should_Have_DisplaySecretKey_Property()
    {
        var connectionType = typeof(LlmConnection);
        var displaySecretKeyProperty = connectionType.GetProperty("DisplaySecretKey");

        Assert.NotNull(displaySecretKeyProperty);
        Assert.Equal(typeof(string), displaySecretKeyProperty.PropertyType);
    }

    [Fact]
    public void UpsertLlmConnectionRequest_Should_Have_SecretKey_Property()
    {
        var requestType = typeof(UpsertLlmConnectionRequest);
        var secretKeyProperty = requestType.GetProperty("SecretKey");

        Assert.NotNull(secretKeyProperty);
        Assert.Equal(typeof(string), secretKeyProperty.PropertyType);
    }

    [Fact]
    public void UpsertLlmConnectionRequest_Should_Not_Have_DisplaySecretKey_Property()
    {
        var requestType = typeof(UpsertLlmConnectionRequest);
        var displaySecretKeyProperty = requestType.GetProperty("DisplaySecretKey");

        Assert.Null(displaySecretKeyProperty);
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

        Assert.NotNull(response);
        Assert.Equal("conn-minimal", response.Id);
        Assert.Null(response.BaseURL);
        Assert.Empty(response.CustomModels);
        Assert.False(response.WithDefaultModels);
        Assert.Empty(response.ExtraHeaderKeys);
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
        Assert.Contains("\"extraHeaders\"", requestJson);
        Assert.Contains("\"X-Header-1\"", requestJson);
        Assert.Contains("\"value1\"", requestJson);

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
        Assert.NotNull(response);
        Assert.Equal(2, response.ExtraHeaderKeys.Length);
        Assert.Contains("X-Header-1", response.ExtraHeaderKeys);
        Assert.Contains("X-Header-2", response.ExtraHeaderKeys);
    }

    [Fact]
    public void Should_Serialize_All_LlmAdapter_Values()
    {
        var allAdapters = new
        {
            Anthropic = LlmAdapter.Anthropic,
            OpenAi = LlmAdapter.OpenAi,
            Azure = LlmAdapter.Azure,
            Bedrock = LlmAdapter.Bedrock,
            GoogleVertexAi = LlmAdapter.GoogleVertexAi,
            GoogleAiStudio = LlmAdapter.GoogleAiStudio
        };

        var json = JsonSerializer.Serialize(allAdapters);

        Assert.Contains("\"anthropic\"", json);
        Assert.Contains("\"openai\"", json);
        Assert.Contains("\"azure\"", json);
        Assert.Contains("\"bedrock\"", json);
        Assert.Contains("\"google-vertex-ai\"", json);
        Assert.Contains("\"google-ai-studio\"", json);
    }

    private class TestAdapterWrapper
    {
        public LlmAdapter Adapter { get; set; }
    }
}
