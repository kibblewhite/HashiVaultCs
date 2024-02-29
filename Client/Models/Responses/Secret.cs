namespace HashiVaultCs.Models.Responses;

/// <summary>
/// 
/// </summary>
/// <remarks>https://github.com/hashicorp/vault/blob/main/api/secret.go</remarks>
public sealed class Secret
{
    [JsonPropertyName("request_id")]
    public required string RequestId { get; init; }

    [JsonPropertyName("lease_id")]
    public required string LeaseId { get; init; }

    [JsonPropertyName("lease_duration")]
    public int LeaseDuration { get; init; }

    [JsonPropertyName("renewable")]
    public bool Renewable { get; init; }

    public bool Failed { get; init; } = false;
    public string? Error { get; init; }

    /// <summary>
    /// Data is the actual contents of the secret.
    /// The format of the data is arbitrary and up to the secret backend.
    /// </summary>
    [JsonPropertyName("data")]
    public required JsonDocument Data { get; init; }

    /// <summary>
    /// Warnings contains any warnings related to the operation.
    /// These are not issues that caused the command to fail,
    /// but that the client should be aware of.
    /// </summary>
    [JsonPropertyName("warnings")]
    public required IEnumerable<string> Warnings { get; init; }

    /// <summary>
    /// Auth, if non-nil, means that there was authentication information attached to this response.
    /// </summary>
    [JsonPropertyName("auth")]
    public required SecretAuth Auth { get; init; }

    /// <summary>
    /// WrapInfo, if non-nil, means that the initial response was wrapped in the
    /// cubbyhole of the given token(which has a TTL of the given number of seconds)
    /// </summary>
    [JsonPropertyName("wrap_info")]
    public required SecretWrapInfo WrapInfo { get; init; }

    public static Secret Failure(string error) => new()
    {
        Auth = new(),
        Data = default!,
        Error = error,
        Failed = true,
        LeaseDuration = 0,
        LeaseId = default!,
        Renewable = false,
        RequestId = default!,
        Warnings = default!,
        WrapInfo = default!
    };

    public static Secret Create(JsonDocument response)
    {
        try
        {
            return response.Deserialize<Secret>()
                ?? throw new InvalidOperationException("JsonSerializer.Deserialize() for secret returned null");
        }
        catch (Exception ex)
        {
            return Failure(ex.Message);
        }
    }
}
