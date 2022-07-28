namespace HashiVaultCs.Interfaces;

public interface IHttpVaultClient
{
    Task<JsonDocument> SendAsync(CancellationToken cancellationToken = default);

    JsonDocument Send(CancellationToken cancellationToken = default);
}
