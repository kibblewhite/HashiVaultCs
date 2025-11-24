using HashiVaultCs.Interfaces.Auth;
using HashiVaultCs.Models;
using HashiVaultCs.Models.Requests.Auth.Approle;

namespace HashiVaultCs.Clients.Auth;

public sealed class ApproleClient(IHttpClientFactory http_client_factory, HttpVaultHeaders vault_headers, string base_address) : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>(vault_headers, base_address), IApproleClient
{
    public async Task<Secret> LoginAsync(Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellation_token = default)
    {
        InternalOperation<string> relative_url = FormattableUriProvider.AuthApproleLogin.GetPath([]);
        if (relative_url.HasFailed)
        {
            return Secret.Failure(relative_url.ErrorMessage ?? "An unknown error occurred while attempting to retrieve the AppRole login URL.");
        }

        Uri request_uri = new(base_uri, relative_url.Result);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, data);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellation_token);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret Login(Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellation_token = default)
    {
        InternalOperation<string> relative_url = FormattableUriProvider.AuthApproleLogin.GetPath([]);
        if (relative_url.HasFailed)
        {
            return Secret.Failure(relative_url.ErrorMessage ?? "An unknown error occurred while attempting to retrieve the AppRole login URL.");
        }

        Uri request_uri = new(base_uri, relative_url.Result);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, data);
        JsonDocumentResult response = http_vault_client.Send(cancellation_token);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public async Task<Secret> RoleIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellation_token = default)
    {
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Rolename, rolename }
        };

        InternalOperation<string> relative_url = FormattableUriProvider.AuthApproleRoleId.GetPath(values);
        if (relative_url.HasFailed)
        {
            return Secret.Failure(relative_url.ErrorMessage ?? "An unknown error occurred while attempting to retrieve the AppRole login URL.");
        }

        Uri request_uri = new(base_uri, relative_url.Result);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Get, vault_headers, headers, request_uri, null);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellation_token);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret RoleId(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellation_token = default)
    {
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Rolename, rolename }
        };

        InternalOperation<string> relative_url = FormattableUriProvider.AuthApproleRoleId.GetPath(values);
        if (relative_url.HasFailed)
        {
            return Secret.Failure(relative_url.ErrorMessage ?? "An unknown error occurred while attempting to retrieve the AppRole login URL.");
        }

        Uri request_uri = new(base_uri, relative_url.Result);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Get, vault_headers, headers, request_uri, null);
        JsonDocumentResult response = http_vault_client.Send(cancellation_token);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public async Task<Secret> SecretIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellation_token = default)
    {
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Rolename, rolename }
        };

        InternalOperation<string> relative_url = FormattableUriProvider.AuthApproleSecretId.GetPath(values);
        if (relative_url.HasFailed)
        {
            return Secret.Failure(relative_url.ErrorMessage ?? "An unknown error occurred while attempting to retrieve the AppRole login URL.");
        }

        Uri request_uri = new(base_uri, relative_url.Result);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, new { });
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellation_token);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret SecretId(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellation_token = default)
    {
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Rolename, rolename }
        };

        InternalOperation<string> relative_url = FormattableUriProvider.AuthApproleSecretId.GetPath(values);
        if (relative_url.HasFailed)
        {
            return Secret.Failure(relative_url.ErrorMessage ?? "An unknown error occurred while attempting to retrieve the AppRole login URL.");
        }

        Uri request_uri = new(base_uri, relative_url.Result);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, new { });
        JsonDocumentResult response = http_vault_client.Send(cancellation_token);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }
}
