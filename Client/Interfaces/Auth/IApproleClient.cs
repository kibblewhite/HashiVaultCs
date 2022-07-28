using HashiVaultCs.Models.Requests.Auth.Approle;

namespace HashiVaultCs.Interfaces.Auth;

public interface IApproleClient : IVaultClient
{
    Task<Secret> LoginAsync(Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);
    Secret Login(Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);

    Task<Secret> RoleIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);
    Secret RoleId(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);

    Task<Secret> SecretIdAsync(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);
    Secret SecretId(string rolename, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);
}
