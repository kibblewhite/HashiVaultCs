namespace HashiVaultCs.Utilities;

internal sealed class FormattableUri
{
    public required FormattableString Value { get; init; }
    public required Dictionary<int, string> CachedUris { get; init; }
}
