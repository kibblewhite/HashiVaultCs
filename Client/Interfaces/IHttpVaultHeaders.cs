using HashiVaultCs;

namespace HashiVaultCs.Interfaces;

public interface IHttpVaultHeaders
{
    bool RequestHeaderPresent { get; }
    KeyValuePair<string, string> Request { get; }

    bool TokenHeaderPresent { get; }
    KeyValuePair<string, string> Token { get; }

    bool NamespaceHeaderPresent { get; }
    KeyValuePair<string, string> Namespace { get; }

    bool WrapTimeToLiveHeaderPresent { get; }
    KeyValuePair<string, string> WrapTimeToLive { get; }

    HttpVaultHeaders Build(IReadOnlyDictionary<string, string> headers);
}
