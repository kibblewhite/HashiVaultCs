namespace HashiVaultCs;

public sealed class EmptyHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => throw new NotImplementedException();
}
