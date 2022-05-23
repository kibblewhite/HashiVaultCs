namespace HashiVaultCs;

public static class HttpVaultHeaderKey
{
    public const string Request = "X-Vault-Request";
    public const string Token = "X-Vault-Token";
    public const string Namespace = "X-Vault-Namespace";
    public const string WrapTimeToLive = "X-Vault-Wrap-TTL";
}
