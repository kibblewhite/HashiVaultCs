using HashiVaultCs.Interfaces;
using HashiVaultCs.Models;

namespace HashiVaultCs;

public class HttpVaultClient : IHttpVaultClient
{
    private const string _media_type = MediaTypeNames.Application.Json;
    private readonly TimeSpan _http_client_timeout = TimeSpan.FromSeconds(10);
    private readonly HttpClient _http_client;
    private readonly HttpRequestMessage _http_request_message;
    private readonly List<HttpMethod> _supported_http_methods_list = [HttpMethod.Get, HttpMethod.Post];
    private static readonly JsonDocumentOptions _jdo = new()
    {
        AllowTrailingCommas = false,
        CommentHandling = JsonCommentHandling.Disallow,
        MaxDepth = 32
    };

    /// <summary>
    /// Build the HTTP Client for making requests to the Vault
    /// </summary>
    /// <param name="http_client_factory"></param>
    /// <param name="method">GET or POST</param>
    /// <param name="vault_headers">The Vault specific HTTP headers</param>
    /// <param name="headers">Any extract HTTP headers</param>
    /// <param name="request_uri">The URI that will be called, this should include the FQDN, with schema and path and query if needed</param>
    /// <param name="data">The method will attempt to serialise any incoming data into a JSON string and include it into the body of the request.</param>
    /// <param name="server_certificate_custom_validation_callback">The delegate for handling SSL server certificate validation. It is part of the HttpClientHandler and can be used to manage self-signed certificates.</param>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public HttpVaultClient(IHttpClientFactory http_client_factory, HttpMethod method, HttpVaultHeaders vault_headers, IReadOnlyDictionary<string, string> headers, Uri request_uri, object? data = null)
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

        _http_client = http_client_factory.CreateClient(nameof(HttpVaultClient));
        _http_client.Timeout = _http_client_timeout;

        _http_client.DefaultRequestHeaders.Accept.Clear();
        _http_client.DefaultRequestHeaders.Accept.Add(new(_media_type));
    }

    /// <summary>
    /// After building the HTTP Client for the Vault, when calling this method, it will take into consideration all the parameters fed into the constructor.
    /// This method will throw an exception if the HTTP response status is anything but a success.
    /// </summary>
    /// <remarks>Only valid JSON is returned from this method through the return of JsonDocument values.</remarks>
    /// <returns>JsonDocument describing the returned content, that can be then deserialised into a model of choice.</returns>
    public async Task<JsonDocumentResult> SendAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage http_response_message = await _http_client.SendAsync(_http_request_message, cancellationToken);
            http_response_message.EnsureSuccessStatusCode();
            string response_json = await http_response_message.Content.ReadAsStringAsync(cancellationToken);
            JsonDocument json_document = JsonDocument.Parse(response_json, _jdo);
            return JsonDocumentResult.Success(json_document);
        }
        catch (Exception ex)
        {
            return JsonDocumentResult.Failure(ex.Message);
        }
    }

    /// <inheritdoc cref="SendAsync"/>
    public JsonDocumentResult Send(CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage http_response_message = _http_client.Send(_http_request_message, cancellationToken);
            http_response_message.EnsureSuccessStatusCode();
            using Stream response_stream = http_response_message.Content.ReadAsStream(cancellationToken);
            using StreamReader response_stream_reader = new(response_stream, true);
            string response_json = response_stream_reader.ReadToEnd();
            JsonDocument json_document = JsonDocument.Parse(response_json, _jdo);
            return JsonDocumentResult.Success(json_document);
        }
        catch (Exception ex)
        {
            return JsonDocumentResult.Failure(ex.Message);
        }
    }
}
