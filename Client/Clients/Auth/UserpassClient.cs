using HashiVaultCs.Interfaces.Auth;
using HashiVaultCs.Models;
using HashiVaultCs.Models.Requests.Auth.Userpass;

namespace HashiVaultCs.Clients.Auth;

public sealed class UserpassClient(IHttpClientFactory http_client_factory, HttpVaultHeaders vault_headers, string base_address) : MustInitialiseHttpVaultHeadersAndHostAbstraction<HttpVaultHeaders>(vault_headers, base_address), IUserpassClient
{
    public async Task<Secret> LoginAsync(string username, Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Username, username }
        };

        InternalOperation<string> relative_url = FormattableUriProvider.AuthUserpassLogin.GetPath(values);
        if (relative_url.HasFailed)
        {
            return Secret.Failure(relative_url.ErrorMessage ?? "An unknown error occurred while attempting to retrieve the AppRole login URL.");
        }

        Uri request_uri = new(base_uri, relative_url.Result);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, data);
        JsonDocumentResult response = await http_vault_client.SendAsync(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }

    public Secret Login(string username, Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default)
    {
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Username, username }
        };

        InternalOperation<string> relative_url = FormattableUriProvider.AuthUserpassLogin.GetPath(values);
        if (relative_url.HasFailed)
        {
            return Secret.Failure(relative_url.ErrorMessage ?? "An unknown error occurred while attempting to retrieve the AppRole login URL.");
        }

        Uri request_uri = new(base_uri, relative_url.Result);
        HttpVaultClient http_vault_client = new(http_client_factory, HttpMethod.Post, vault_headers, headers, request_uri, data);
        JsonDocumentResult response = http_vault_client.Send(cancellationToken);
        return response.Failed is true
            ? Secret.Failure(response.Error)
            : Secret.Create(response.Result);
    }
}
