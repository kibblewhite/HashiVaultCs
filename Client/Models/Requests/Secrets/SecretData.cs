namespace HashiVaultCs.Models.Requests.Secrets;

public sealed class SecretData
{
    [JsonPropertyName(name: "options")]
    public SecretOptions? Options { get; init; }

    [JsonPropertyName("data")]
    public JsonDocument? Data { get; init; }

    public SecretData() { }

    public SecretData(object data, Type data_type) : this()
        => Data = JsonSerializer.SerializeToDocument(data, data_type);
}
