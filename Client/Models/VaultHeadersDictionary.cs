namespace HashiVaultCs.Models;

internal sealed class VaultHeadersDictionary : Dictionary<string, string>
{
    internal bool TryAddHeader(string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value) is true)
        {
            return false;
        }

        Remove(key);
        Add(key, value);
        return true;
    }
}
