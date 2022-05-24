namespace HashiVaultCs.Extentions;

public static class HttpRequestMessageExtentions
{
    public static void AddVaultHttpHeaders(this HttpRequestMessage http_request_message, HttpVaultHeaders vault_headers)
    {
        ArgumentNullException.ThrowIfNull(vault_headers);

        if (vault_headers.RequestHeaderPresent is true)
        {
            http_request_message.Headers.Remove(vault_headers.Request.Key);
            http_request_message.Headers.Add(vault_headers.Request.Key, vault_headers.Request.Value);
        }

        if (vault_headers.TokenHeaderPresent is true)
        {
            http_request_message.Headers.Remove(vault_headers.Token.Key);
            http_request_message.Headers.Add(vault_headers.Token.Key, vault_headers.Token.Value);
        }

        if (vault_headers.NamespaceHeaderPresent is true)
        {
            http_request_message.Headers.Remove(vault_headers.Namespace.Key);
            http_request_message.Headers.Add(vault_headers.Namespace.Key, vault_headers.Namespace.Value);
        }

        if (vault_headers.WrapTimeToLiveHeaderPresent is true)
        {
            http_request_message.Headers.Remove(vault_headers.WrapTimeToLive.Key);
            http_request_message.Headers.Add(vault_headers.WrapTimeToLive.Key, vault_headers.WrapTimeToLive.Value);
        }
    }
}
