using HashiVaultCs.Models.Requests.Secrets;

namespace HashiVaultCs.Interfaces.Secrets;

public interface IDataClient : IVaultClient
{
    Task<Secret> GetAsync(string engine, string path, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);
    Secret Get(string engine, string path, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);

    Task<Secret> PostAsync(string engine, string path, SecretData data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);
    Secret Post(string engine, string path, SecretData data, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);
}
