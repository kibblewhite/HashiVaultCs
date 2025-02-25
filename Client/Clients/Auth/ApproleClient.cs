using HashiVaultCs.Interfaces.Auth;
using HashiVaultCs.Models;
using HashiVaultCs.Models.Requests.Auth.Approle;

namespace HashiVaultCs.Clients.Auth;

public sealed class ApproleClient(IHttpClientFactory http_client_factory, HttpVaultHeaders vault_headers, string base_address) : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>(vault_headers, base_address), IApproleClient
{
    public async Task<Secret> LoginAsync(Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        Uri request_uri = new(base_uri, ApiUrl.AuthApproleLogin);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, data);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret Login(Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        Uri request_uri = new(base_uri, ApiUrl.AuthApproleLogin);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, data);
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public async Task<Secret> RoleIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleRoleId.FormatWith(new { rolename });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Get, vault_headers, headers, request_uri, null);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret RoleId(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleRoleId.FormatWith(new { rolename });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Get, vault_headers, headers, request_uri, null);
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public async Task<Secret> SecretIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleSecretId.FormatWith(new { rolename });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, new { });
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret SecretId(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        string relative_url = ApiUrl.AuthApproleSecretId.FormatWith(new { rolename });
        Uri request_uri = new(base_uri, relative_url);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, new { });
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }
}
