using HashiVaultCs.Interfaces;

namespace HashiVaultCs.Clients;

public abstract class MustInitialiseHttpVaultHeadersAndHostAbstraction<T>(T vault_headers, string base_address) where T : IHttpVaultHeaders
{
    protected readonly T vault_headers = vault_headers;
    protected readonly Uri base_uri = new(base_address);
}
