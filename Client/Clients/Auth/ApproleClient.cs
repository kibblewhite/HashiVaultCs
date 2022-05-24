using HashiVaultCs.Interfaces.Auth;
using HashiVaultCs.Models.Requests.Auth.Approle;

namespace HashiVaultCs.Clients.Auth;

public sealed class ApproleClient : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>, IApproleClient
{
    public ApproleClient(HttpVaultHeaders vault_headers, string base_address) : base(vault_headers, base_address) { }

    public async Task<Secret> LoginAsync(Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        Uri request_uri = new(_base_uri, ApiUrl.AuthApproleLogin);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, _vault_headers, headers, request_uri, data);
        JsonDocument response = await http_vault_client.SendAsync(cancellationToken);
        return response.Deserialize<Secret>() ?? new Secret();
    }

    public async Task<Secret> RoleIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleRoleId.FormatWith(new { rolename });
        Uri request_uri = new(_base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Get, _vault_headers, headers, request_uri);
        JsonDocument response = await http_vault_client.SendAsync(cancellationToken);
        return response.Deserialize<Secret>() ?? new Secret();
    }

    public async Task<Secret> SecretIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleSecretId.FormatWith(new { rolename });
        Uri request_uri = new(_base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, _vault_headers, headers, request_uri, new { });
        JsonDocument response = await http_vault_client.SendAsync(cancellationToken);
        return response.Deserialize<Secret>() ?? new Secret();
    }
}
