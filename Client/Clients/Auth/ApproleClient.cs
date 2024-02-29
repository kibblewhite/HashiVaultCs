using HashiVaultCs.Interfaces.Auth;
using HashiVaultCs.Models;
using HashiVaultCs.Models.Requests.Auth.Approle;

namespace HashiVaultCs.Clients.Auth;

public sealed class ApproleClient(HttpVaultHeaders vault_headers, string base_address, Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? server_certificate_custom_validation_callback = null) : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>(vault_headers, base_address, server_certificate_custom_validation_callback), IApproleClient
{
    public async Task<Secret> LoginAsync(Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        Uri request_uri = new(base_uri, ApiUrl.AuthApproleLogin);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, vault_headers, headers, request_uri, data, _server_certificate_custom_validation_callback);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret Login(Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        Uri request_uri = new(base_uri, ApiUrl.AuthApproleLogin);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, vault_headers, headers, request_uri, data, _server_certificate_custom_validation_callback);
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public async Task<Secret> RoleIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleRoleId.FormatWith(new { rolename });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Get, vault_headers, headers, request_uri, null, _server_certificate_custom_validation_callback);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret RoleId(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleRoleId.FormatWith(new { rolename });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Get, vault_headers, headers, request_uri, null, _server_certificate_custom_validation_callback);
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public async Task<Secret> SecretIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleSecretId.FormatWith(new { rolename });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, vault_headers, headers, request_uri, new { }, _server_certificate_custom_validation_callback);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret SecretId(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleSecretId.FormatWith(new { rolename });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(HttpMethod.Post, vault_headers, headers, request_uri, new { }, _server_certificate_custom_validation_callback);
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }
}
