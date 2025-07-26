using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Options;
using zborek.Langfuse.Config;

namespace zborek.Langfuse.Services;

internal class AuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly IOptions<LangfuseConfig> _config;

    public AuthorizationDelegatingHandler(IOptions<LangfuseConfig> config)
    {
        _config = config;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
            GenerateBasicAuthHeaderValue(_config.Value.PublicKey, _config.Value.SecretKey));

        return base.SendAsync(request, cancellationToken);
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
            GenerateBasicAuthHeaderValue(_config.Value.PublicKey, _config.Value.SecretKey));
        return base.Send(request, cancellationToken);
    }

    private static string GenerateBasicAuthHeaderValue(string publicKey, string secretKey)
    {
        var credentials = $"{publicKey}:{secretKey}";
        var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        return base64Credentials;
    }
}