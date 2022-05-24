using HashiVaultCs.Interfaces;

namespace HashiVaultCs;

public sealed class HttpVaultClient : IHttpVaultClient
{
    private const string _media_type = "application/json";
    private readonly TimeSpan _http_client_timeout = TimeSpan.FromSeconds(6);
    private readonly HttpClient _http_client;
    private readonly HttpRequestMessage _http_request_message;
    private readonly List<HttpMethod> _supported_http_methods_list = new() { HttpMethod.Get, HttpMethod.Post };

    public HttpVaultClient(HttpMethod method, HttpVaultHeaders vault_headers, IReadOnlyDictionary<string, string> headers, Uri request_uri, object? data = null)
    {
        // Currently only HTTP Methods GET & POST is supported.
        if (_supported_http_methods_list.Contains(method) is false)
        {
            throw new NotSupportedException($"{Resources.HttpVaultClient.Resource.MethodNotSupportedException}. {Resources.HttpVaultClient.Resource.SupportedHttpMethodsAre}: {string.Join(", ", _supported_http_methods_list)}");
        }

        // HTTP Headers should not be null, but can be empty (ImmutableDictionary<string, string>.Empty).
        if (headers?.Any() is null)
        {
            throw new ArgumentNullException(nameof(headers), Resources.HttpVaultClient.Resource.HeadersArgumentNullException);
        }

        // Generate HTTP Request Message and set content
        _http_request_message = new(method, request_uri)
        {
            Content = data is null
                ? null
                : new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, _media_type)
        };

        // Build the HTTP Headers, both custom and Vault Specific HTTP Headers.
        _http_request_message.AddVaultHttpHeaders(vault_headers);
        foreach (KeyValuePair<string, string> kvp in headers)
        {
            _http_request_message.Headers.Remove(kvp.Key);
            _http_request_message.Headers.Add(kvp.Key, kvp.Value);
        }

        HttpClientHandler handler = new();
        _http_client = new(handler)
        {
            Timeout = _http_client_timeout
        };

        _http_client.DefaultRequestHeaders.Accept.Clear();
        _http_client.DefaultRequestHeaders.Accept.Add(new(_media_type));
    }

    /// <summary>
    /// This method will throw an exception if the HTTP response status is anything but a success.
    /// </summary>
    /// <remarks>Only valid JSON is returned from this method through the return of JsonDocument values.</remarks>
    /// <returns></returns>
    public async Task<JsonDocument> SendAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage http_response_message = await _http_client.SendAsync(_http_request_message, cancellationToken);
        http_response_message.EnsureSuccessStatusCode();
        string response_json = await http_response_message.Content.ReadAsStringAsync(cancellationToken);
        JsonDocumentOptions jdo = new()
        {
            AllowTrailingCommas = false,
            CommentHandling = JsonCommentHandling.Disallow,
            MaxDepth = 32
        };
        JsonDocument document = JsonDocument.Parse(response_json, jdo);
        return document;
    }
}
