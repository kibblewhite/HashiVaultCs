using HashiVaultCs.Interfaces;

namespace HashiVaultCs.Clients;

public abstract class MustInitialiseHttpVaultHeadersAndHostAbstraction<T> where T : IHttpVaultHeaders
{
    protected readonly T _vault_headers;
    protected readonly Uri _base_uri;
    public MustInitialiseHttpVaultHeadersAndHostAbstraction(T vault_headers, string base_address)
    {
        _vault_headers = vault_headers;
        _base_uri = new Uri(base_address);
    }
}
