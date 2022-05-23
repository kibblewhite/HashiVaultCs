namespace HashiVaultCs.Interfaces.Secrets;

public interface IDataClient : IVaultClient
{
    Task<Secret> GetAsync(string engine, string path, IImmutableDictionary<string, string> headers, CancellationToken cancellationToken = default);
}
