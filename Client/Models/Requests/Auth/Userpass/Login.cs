namespace HashiVaultCs.Models.Requests.Auth.Userpass;

public sealed class Login
{
    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
