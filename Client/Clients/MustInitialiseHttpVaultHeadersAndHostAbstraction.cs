using HashiVaultCs.Interfaces;

namespace HashiVaultCs.Clients;

public abstract class MustInitialiseHttpVaultHeadersAndHostAbstraction<T> where T : IHttpVaultHeaders
{
    protected Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? _server_certificate_custom_validation_callback;
    protected readonly T vault_headers;
    protected readonly Uri base_uri;

    public MustInitialiseHttpVaultHeadersAndHostAbstraction(T vault_headers, string base_address, Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool>? server_certificate_custom_validation_callback = null)
    {
        this.vault_headers = vault_headers;
        base_uri = new Uri(base_address);
        _server_certificate_custom_validation_callback = server_certificate_custom_validation_callback;
    }

    /// <summary>
    /// Set the HttpClientHandler ServerCertificateCustomValidationCallback.
    /// For dealing with self-signed certificates, it might be necessary to set this value to HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    /// </summary>
    public virtual void SetServerCertificateCustomValidationCallback(Func<HttpRequestMessage, X509Certificate2?, X509Chain?, SslPolicyErrors, bool> server_certificate_custom_validation_callback)
        => _server_certificate_custom_validation_callback = server_certificate_custom_validation_callback;

    public virtual void RemoveServerCertificateCustomValidationCallback() => _server_certificate_custom_validation_callback = null;
}
