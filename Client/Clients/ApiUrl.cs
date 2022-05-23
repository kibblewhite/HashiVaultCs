namespace HashiVaultCs.Clients;

public static class ApiUrl
{
    public const string AuthUserpassLogin = "/v1/auth/userpass/login/{username}";
    public const string AuthApproleRoleId = "/v1/auth/approle/role/{rolename}/role-id";
    public const string AuthApproleSecretId = "/v1/auth/approle/role/{rolename}/secret-id";
    public const string AuthApproleLogin = "/v1/auth/approle/login";
    public const string SecretsEngineDataPath = "/v1/{engine}/data/{path}";
}
