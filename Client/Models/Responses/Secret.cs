namespace HashiVaultCs.Models.Responses;

/// <summary>
/// 
/// </summary>
/// <remarks>https://github.com/hashicorp/vault/blob/main/api/secret.go</remarks>
public sealed class Secret
{
    [JsonPropertyName("request_id")]
    public string? RequestId { get; init; } = default!;

    [JsonPropertyName("lease_id")]
    public string? LeaseId { get; init; } = default!;

    [JsonPropertyName("lease_duration")]
    public int LeaseDuration { get; init; }

    [JsonPropertyName("renewable")]
    public bool Renewable { get; init; }

    /// <summary>
    /// Data is the actual contents of the secret.
    /// The format of the data is arbitrary and up to the secret backend.
    /// </summary>
    [JsonPropertyName("data")]
    public JsonDocument? Data { get; init; }

    /// <summary>
    /// Warnings contains any warnings related to the operation.
    /// These are not issues that caused the command to fail,
    /// but that the client should be aware of.
    /// </summary>
    [JsonPropertyName("warnings")]
    public IEnumerable<string>? Warnings { get; init; }

    /// <summary>
    /// Auth, if non-nil, means that there was authentication information attached to this response.
    /// </summary>
    [JsonPropertyName("auth")]
    public SecretAuth? Auth { get; init; }

    /// <summary>
    /// WrapInfo, if non-nil, means that the initial response was wrapped in the
    /// cubbyhole of the given token(which has a TTL of the given number of seconds)
    /// </summary>
    [JsonPropertyName("wrap_info")]
    public SecretWrapInfo? WrapInfo { get; init; }
}
