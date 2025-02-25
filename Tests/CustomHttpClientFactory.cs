namespace Tests;

/// <summary>
/// Custom implementation of <see cref="IHttpClientFactory"/> for unit testing.
/// </summary>
public class CustomHttpClientFactory : IHttpClientFactory
{
    private readonly HttpClient _http_client;

    public CustomHttpClientFactory() => _http_client = new HttpClient(new HttpClientHandler
    {
        // HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true // Accept self-signed certificates
    });

    public HttpClient CreateClient(string name) => _http_client;
}
