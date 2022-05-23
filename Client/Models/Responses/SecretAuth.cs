namespace HashiVaultCs.Models.Responses;

/// <summary>
/// 
/// </summary>
/// <remarks>https://github.com/hashicorp/vault/blob/main/api/secret.go</remarks>
public sealed class SecretAuth
{
    [JsonPropertyName("client_token")]
    public string? ClientToken { get; init; } = default!;

    [JsonPropertyName("accessor")]
    public string? Accessor { get; init; } = default!;

    [JsonPropertyName("policies")]
    public IEnumerable<string>? Policies { get; init; }

    [JsonPropertyName("token_policies")]
    public IEnumerable<string>? TokenPolicies { get; init; }

    [JsonPropertyName("identity_policies")]
    public IEnumerable<string>? IdentityPolicies { get; init; }

    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; init; }

    [JsonPropertyName("orphan")]
    public bool Orphan { get; init; }

    [JsonPropertyName("entity_id")]
    public string? EntityID { get; init; } = default!;

    [JsonPropertyName("lease_duration")]
    public int LeaseDuration { get; init; }

    [JsonPropertyName("renewable")]
    public bool Renewable { get; init; }

    [Obsolete("Currently unsupported")]
    [JsonPropertyName("mfa_requirement")]
    public object? MFARequirement { get; init; }
}
