using HashiVaultCs.Interfaces;

namespace HashiVaultCs;

public sealed class HttpVaultHeaders : IHttpVaultHeaders
{
    public bool RequestHeaderPresent { get; private set; }
    private KeyValuePair<string, string>? _request;
    public KeyValuePair<string, string> Request => RequestHeaderPresent is false || _request is null
        ? throw new InvalidOperationException($"{Resources.Resource.VaultHttpHeadersInvalidOperationException}: {nameof(Request)}")
        : _request ?? throw new NullReferenceException();

    public bool TokenHeaderPresent { get; private set; }
    private KeyValuePair<string, string>? _token;
    public KeyValuePair<string, string> Token => TokenHeaderPresent is false || _token is null
        ? throw new InvalidOperationException($"{Resources.Resource.VaultHttpHeadersInvalidOperationException}: {nameof(Token)}")
        : _token ?? throw new NullReferenceException();

    public bool NamespaceHeaderPresent { get; private set; }
    private KeyValuePair<string, string>? _namespace;
    public KeyValuePair<string, string> Namespace => NamespaceHeaderPresent is false || _namespace is null
        ? throw new InvalidOperationException($"{Resources.Resource.VaultHttpHeadersInvalidOperationException}: {nameof(Namespace)}")
        : _namespace ?? throw new NullReferenceException();

    public bool WrapTimeToLiveHeaderPresent { get; private set; }
    private KeyValuePair<string, string>? _wrapttl;
    public KeyValuePair<string, string> WrapTimeToLive => WrapTimeToLiveHeaderPresent is false || _wrapttl is null
        ? throw new InvalidOperationException($"{Resources.Resource.VaultHttpHeadersInvalidOperationException}: {nameof(WrapTimeToLive)}")
        : _wrapttl ?? throw new NullReferenceException();

    HttpVaultHeaders IHttpVaultHeaders.Build(IReadOnlyDictionary<string, string> headers) => Build(headers);

    public static HttpVaultHeaders Build(IReadOnlyDictionary<string, string> headers)
    {
        HttpVaultHeaders vault_http_headers = new();
        foreach (KeyValuePair<string, string> kvp in headers)
        {
            switch (kvp.Key)
            {
                case HttpVaultHeaderKey.Request:
                    {
                        vault_http_headers._request = new KeyValuePair<string, string>(HttpVaultHeaderKey.Request, kvp.Value);
                        vault_http_headers.RequestHeaderPresent = true;
                        break;
                    }
                case HttpVaultHeaderKey.Token:
                    {
                        vault_http_headers._token = new KeyValuePair<string, string>(HttpVaultHeaderKey.Token, kvp.Value);
                        vault_http_headers.TokenHeaderPresent = true;
                        break;
                    }
                case HttpVaultHeaderKey.Namespace:
                    {
                        vault_http_headers._namespace = new KeyValuePair<string, string>(HttpVaultHeaderKey.Namespace, kvp.Value);
                        vault_http_headers.NamespaceHeaderPresent = true;
                        break;
                    }
                case HttpVaultHeaderKey.WrapTimeToLive:
                    {
                        vault_http_headers._wrapttl = new KeyValuePair<string, string>(HttpVaultHeaderKey.WrapTimeToLive, kvp.Value);
                        vault_http_headers.WrapTimeToLiveHeaderPresent = true;
                        break;
                    }
            }
        }

        return vault_http_headers;
    }
}
