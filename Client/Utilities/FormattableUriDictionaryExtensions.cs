namespace HashiVaultCs.Utilities;

internal static class FormattableUriDictionaryExtensions
{
    internal static int GetCacheIdentifier(this Dictionary<FormattableArgument, string> formattable_dictionary)
    {
        if (formattable_dictionary == null)
        {
            return 0;
        }

        HashCode hash = new();
        foreach (KeyValuePair<FormattableArgument, string> kvp in formattable_dictionary.OrderBy(kvp => kvp.Key))
        {
            hash.Add(kvp.Key);
            hash.Add(kvp.Value);
        }

        return hash.ToHashCode();
    }
}
