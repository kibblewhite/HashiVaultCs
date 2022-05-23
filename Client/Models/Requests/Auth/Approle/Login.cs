namespace HashiVaultCs.Models.Requests.Auth.Approle;

public sealed class Login
{
    [JsonPropertyName("role_id")]
    public string? RoleId { get; set; }

    [JsonPropertyName("secret_id")]
    public string? SecretId { get; set; }
}
