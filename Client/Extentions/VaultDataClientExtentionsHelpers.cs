using HashiVaultCs.Clients;
using HashiVaultCs.Clients.Auth;
using HashiVaultCs.Clients.Secrets;
using HashiVaultCs.Models;

namespace HashiVaultCs.Extentions;

public static class VaultDataClientExtentionsHelpers
{
    /// <summary>
    /// Creates a <see cref="DataClient"/> instance using userpass and AppRole authentication against a Vault server.
    /// </summary>
    /// <param name="credentials">The credentials used to authenticate with the Vault server.</param>
    /// <param name="http_client_factory">The factory used to create <see cref="HttpClient"/> instances for making HTTP requests.</param>
    /// <returns>
    /// A <see cref="DataClientResult"/> containing either a successfully authenticated <see cref="DataClient"/> 
    /// or a failure result with an error message.
    /// </returns>
    public static DataClientResult CreateDataClient(this DataClientCredentials credentials, IHttpClientFactory http_client_factory)
    {
        // We'll use this to build out HTTP vault headers
        HttpVaultHeaders http_vault_headers;
        VaultHeadersDictionary vault_headers = [];

        // Login to the vault using our userpass authentication creditials
        http_vault_headers = HttpVaultHeaders.Build(vault_headers.AsReadOnly());
        UserpassClient userpass_client = new(http_client_factory, http_vault_headers, credentials.BaseAddress);
        Secret userpass_login_response = userpass_client.Login(credentials.Username, new Models.Requests.Auth.Userpass.Login
        {
            Password = credentials.Password
        }, ImmutableDictionary<string, string>.Empty);

        // Check that the ClientToken is present and use this next for our Vault Token
        if (userpass_login_response.Failed is true)
        {
            return DataClientResult.Failure($"UserpassClient.Login failed with error: {userpass_login_response.Error}");
        }

        // Set the vault header to contain the ClientToken
        if (vault_headers.TryAddHeader(HttpVaultHeaderKey.Token, userpass_login_response.Auth.ClientToken) is false)
        {
            return DataClientResult.Failure("Failed to add user client token.");
        }

        // Success the AppRole client in order to send requests against
        http_vault_headers = HttpVaultHeaders.Build(vault_headers.AsReadOnly());
        ApproleClient approle_client = new(http_client_factory, http_vault_headers, credentials.BaseAddress);

        // Get the role-id and generate a secret-id for the approle 'staging' as logged in user
        Secret approle_roleid_response = approle_client.RoleId(credentials.Rolename, ImmutableDictionary<string, string>.Empty);
        if (approle_roleid_response.Failed is true)
        {
            return DataClientResult.Failure($"ApproleClient.RoleId failed with error: {approle_roleid_response.Error}");
        }

        string? role_id = approle_roleid_response.Data?.RootElement.GetProperty("role_id").GetString();
        if (string.IsNullOrWhiteSpace(role_id) is true)
        {
            return DataClientResult.Failure($"Could not retrieve 'role_id' from returned response from: {ApiUrl.AuthApproleRoleId}");
        }

        // Generate a secret-id
        Secret approle_secretid_response = approle_client.SecretId(credentials.Rolename, ImmutableDictionary<string, string>.Empty);
        if (approle_secretid_response.Failed is true)
        {
            return DataClientResult.Failure($"ApproleClient.SecretId failed with error: {approle_secretid_response.Error}");
        }

        string? secret_id = approle_secretid_response.Data?.RootElement.GetProperty("secret_id").GetString();
        if (string.IsNullOrWhiteSpace(secret_id) is true)
        {
            return DataClientResult.Failure($"Could not retrieve 'secret_id' from returned response from: {ApiUrl.AuthApproleSecretId}");
        }

        // Generate a token using the previously retrieved role-id and newly created secret-id. 
        Secret approle_login_response = approle_client.Login(new Models.Requests.Auth.Approle.Login
        {
            RoleId = role_id,
            SecretId = secret_id
        }, ImmutableDictionary<string, string>.Empty);

        if (approle_login_response.Failed is true)
        {
            return DataClientResult.Failure($"ApproleClient.Login failed with error: {approle_login_response.Error}");
        }

        // Set the vault header to contain the newly created ClientToken from the AppRole Login (ApproleClient.LoginAsync(...))
        if (vault_headers.TryAddHeader(HttpVaultHeaderKey.Token, approle_login_response.Auth.ClientToken) is false)
        {
            return DataClientResult.Failure("Failed to add role client token.");
        }

        // Success the Data Client to use for reading and writing kv secrets
        http_vault_headers = HttpVaultHeaders.Build(vault_headers.AsReadOnly());
        DataClient data_client = new(http_client_factory, http_vault_headers, credentials.BaseAddress);

        return DataClientResult.Success(data_client);
    }
}
