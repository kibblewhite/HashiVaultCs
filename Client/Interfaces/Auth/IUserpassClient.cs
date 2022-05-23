using HashiVaultCs.Models.Requests.Auth.Userpass;

namespace HashiVaultCs.Interfaces.Auth;

public interface IUserpassClient : IVaultClient
{
    Task<Secret> LoginAsync(string username, Login data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);
}
