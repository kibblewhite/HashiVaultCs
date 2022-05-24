using HashiVaultCs.Interfaces.Auth;
using HashiVaultCs.Models.Requests.Auth.Userpass;

namespace HashiVaultCs.Clients.Auth;

public sealed class UserpassClient : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>, IUserpassClient
{
    public UserpassClient(HttpVaultHeaders vault_headers, string base_address) : base(vault_headers, base_address) { }

    public async Task<Secret> LoginAsync(string username, Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthUserpassLogin.FormatWith(new { username });
        Uri request_uri = new(_base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, _vault_headers, headers, request_uri, data);
        JsonDocument response = await http_vault_client.SendAsync(cancellationToken);
        return response.Deserialize<Secret>() ?? new Secret();  // todo (2022-05-23|kibble): What to return when Deserialize() fails, this method shouldn't return empty secrets...
    }
}
