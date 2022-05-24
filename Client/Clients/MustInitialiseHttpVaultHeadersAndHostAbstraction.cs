using HashiVaultCs.Interfaces;

namespace HashiVaultCs.Clients;

public abstract class MustInitialiseHttpVaultHeadersAndHostAbstraction<T> where T : IHttpVaultHeaders
{
    protected readonly T vault_headers;
    protected readonly Uri base_uri;
    public MustInitialiseHttpVaultHeadersAndHostAbstraction(T vault_headers, string base_address)
    {
        this.vault_headers = vault_headers;
        base_uri = new Uri(base_address);
    }
}
