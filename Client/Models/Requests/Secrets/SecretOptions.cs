namespace HashiVaultCs.Models.Requests.Secrets;

public sealed class SecretOptions
{
    [JsonPropertyName(name: "cas")]
    public int CAS { get; init; }
}
