using HashiVaultCs.Interfaces.Secrets;

namespace HashiVaultCs.Clients.Secrets;

public sealed class DataClient : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>, IDataClient
{
    public DataClient(HttpVaultHeaders vault_headers, string base_address) : base(vault_headers, base_address) { }

    public async Task<Secret> GetAsync(string engine, string path, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.SecretsEngineDataPath.FormatWith(new
        {
            engine,
            path
        });
        Uri request_uri = new(_base_uri, relative_url);

        HttpVaultClient http_vault_client = new(HttpMethod.Get, _vault_headers, headers, request_uri);
        JsonDocument response = await http_vault_client.SendAsync(cancellationToken);
        return response.Deserialize<Secret>() ?? new Secret();
    }
}
