namespace HashiVaultCs.Models.Responses;

/// <summary>
/// 
/// </summary>
/// <remarks>https://github.com/hashicorp/vault/blob/main/api/secret.go</remarks>
public sealed class SecretWrapInfo
{
    [JsonPropertyName("token")]
    public string? Token { get; init; } = default!;

    [JsonPropertyName("accessor")]
    public string? Accessor { get; init; } = default!;

    [JsonPropertyName("ttl")]
    public int TTL { get; init; }

    [JsonPropertyName("creation_time")]
    public DateTime CreationTime { get; init; }

    [JsonPropertyName("creation_path")]
    public string? CreationPath { get; init; } = default!;

    [JsonPropertyName("wrapped_accessor")]
    public string? WrappedAccessor { get; init; } = default!;
}
