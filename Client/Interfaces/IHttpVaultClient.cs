using HashiVaultCs.Models;

namespace HashiVaultCs.Interfaces;

public interface IHttpVaultClient
{
    Task<JsonDocumentResult> SendAsync(CancellationToken cancellationToken = default);

    JsonDocumentResult Send(CancellationToken cancellationToken = default);
}
