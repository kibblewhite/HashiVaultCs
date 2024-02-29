using HashiVaultCs.Interfaces.Auth;
using HashiVaultCs.Models;
using HashiVaultCs.Models.Requests.Auth.Userpass;

namespace HashiVaultCs.Clients.Auth;

public sealed class UserpassClient(HttpVaultHeaders vault_headers, string base_address, Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? server_certificate_custom_validation_callback = null) : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>(vault_headers, base_address, server_certificate_custom_validation_callback), IUserpassClient
{
    public async Task<Secret> LoginAsync(string username, Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthUserpassLogin.FormatWith(new { username });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, vault_headers, headers, request_uri, data, _server_certificate_custom_validation_callback);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret Login(string username, Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthUserpassLogin.FormatWith(new { username });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, vault_headers, headers, request_uri, data, _server_certificate_custom_validation_callback);
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }
}
