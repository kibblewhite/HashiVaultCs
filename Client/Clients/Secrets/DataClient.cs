using HashiVaultCs.Interfaces.Secrets;
using HashiVaultCs.Models;
using HashiVaultCs.Models.Requests.Secrets;

namespace HashiVaultCs.Clients.Secrets;

public sealed class DataClient(IHttpClientFactory http_client_factory, HttpVaultHeaders vault_headers, string base_address) : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>(vault_headers, base_address), IDataClient
{
    public async Task<Secret> GetAsync(string engine, string path, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.SecretsEngineDataPath.FormatWith(new { engine, path });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Get, vault_headers, headers, request_uri, null);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret Get(string engine, string path, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.SecretsEngineDataPath.FormatWith(new { engine, path });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Get, vault_headers, headers, request_uri, null);
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public async Task<Secret> PostAsync(string engine, string path, SecretData data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.SecretsEngineDataPath.FormatWith(new { engine, path });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, data);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret Post(string engine, string path, SecretData data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.SecretsEngineDataPath.FormatWith(new { engine, path });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, data);
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }
}
