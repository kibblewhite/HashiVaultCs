using HashiVaultCs.Interfaces.Secrets;
using HashiVaultCs.Models.Requests.Secrets;

namespace HashiVaultCs.Clients.Secrets;

public sealed class DataClient : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>, IDataClient
{
    public DataClient(HttpVaultHeaders vault_headers, string base_address, Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? server_certificate_custom_validation_callback = null)
        : base(vault_headers, base_address, server_certificate_custom_validation_callback) { }

    public async Task<Secret> GetAsync(string engine, string path, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.SecretsEngineDataPath.FormatWith(new { engine, path });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Get, vault_headers, headers, request_uri, null, _server_certificate_custom_validation_callback);
        JsonDocument response = await http_vault_client.SendAsync(cancellationToken);
        return response.Deserialize<Secret>() ?? new Secret();
    }
    public Secret Get(string engine, string path, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.SecretsEngineDataPath.FormatWith(new { engine, path });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Get, vault_headers, headers, request_uri, null, _server_certificate_custom_validation_callback);
        JsonDocument response = http_vault_client.Send(cancellationToken);
        return response.Deserialize<Secret>() ?? new Secret();
    }

    public async Task<Secret> PostAsync(string engine, string path, SecretData data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.SecretsEngineDataPath.FormatWith(new { engine, path });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, vault_headers, headers, request_uri, data, _server_certificate_custom_validation_callback);
        JsonDocument response = await http_vault_client.SendAsync(cancellationToken);
        return response.Deserialize<Secret>() ?? new Secret();
    }

    public Secret Post(string engine, string path, SecretData data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.SecretsEngineDataPath.FormatWith(new { engine, path });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, vault_headers, headers, request_uri, data, _server_certificate_custom_validation_callback);
        JsonDocument response = http_vault_client.Send(cancellationToken);
        return response.Deserialize<Secret>() ?? new Secret();
    }
}
