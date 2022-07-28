using HashiVaultCs.Interfaces;

namespace HashiVaultCs;

public sealed class HttpVaultClient : IHttpVaultClient
{
    private const string _media_type = "application/json";
    private readonly TimeSpan _http_client_timeout = TimeSpan.FromSeconds(6);
    private readonly HttpClient _http_client;
    private readonly HttpRequestMessage _http_request_message;
    private readonly List<HttpMethod> _supported_http_methods_list = new() { HttpMethod.Get, HttpMethod.Post };
    private static readonly JsonDocumentOptions _jdo = new()
    {
        AllowTrailingCommas = false,
        CommentHandling = JsonCommentHandling.Disallow,
        MaxDepth = 32
    };

    /// <summary>
    /// Build the HTTP Client for making requests to the Vault
    /// </summary>
    /// <param name="method">GET or POST</param>
    /// <param name="vault_headers">The Vault specific HTTP headers</param>
    /// <param name="headers">Any extract HTTP headers</param>
    /// <param name="request_uri">The URI that will be called, this should include the FQDN, with schema and path and query if needed</param>
    /// <param name="data">The method will attempt to serialise any incoming data into a JSON string and include it into the body of the request.</param>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
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

        // Alway allow returning certificate by using DangerousAcceptAnyServerCertificateValidator <- this will need improvement in the future to allow the choice or not to accept any server certificate
        HttpClientHandler handler = new()
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                // (http_request_message, cert, chain, policy_errors) => true
        };

        _http_client = new(handler)
        {
            Timeout = _http_client_timeout
        };

        _http_client.DefaultRequestHeaders.Accept.Clear();
        _http_client.DefaultRequestHeaders.Accept.Add(new(_media_type));
    }

    /// <summary>
    /// After building the HTTP Client for the Vault, when calling this method, it will take into consideration all the parameters fed into the constructor.
    /// This method will throw an exception if the HTTP response status is anything but a success.
    /// </summary>
    /// <remarks>Only valid JSON is returned from this method through the return of JsonDocument values.</remarks>
    /// <returns>JsonDocument describing the returned content, that can be then deserialised into a model of choice.</returns>
    public async Task<JsonDocument> SendAsync(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage http_response_message = await _http_client.SendAsync(_http_request_message, cancellationToken);
        http_response_message.EnsureSuccessStatusCode();
        string response_json = await http_response_message.Content.ReadAsStringAsync(cancellationToken);
        return JsonDocument.Parse(response_json, _jdo);
    }

    /// <inheritdoc cref="SendAsync"/>
    public JsonDocument Send(CancellationToken cancellationToken = default)
    {
        HttpResponseMessage http_response_message = _http_client.Send(_http_request_message, cancellationToken);
        http_response_message.EnsureSuccessStatusCode();
        Stream response_stream = http_response_message.Content.ReadAsStream(cancellationToken);
        using StreamReader response_stream_reader = new(response_stream, true);
        string response_json = response_stream_reader.ReadToEnd();
        return JsonDocument.Parse(response_json, _jdo);
    }
}
