namespace Tests;

[TestClass]
public class UnitTest
{

    private readonly string _base_address;
    private readonly string _username;
    private readonly string _password;
    private readonly string _rolename;
    private readonly string _engine;
    private readonly string _secrets_path;

    public UnitTest()
    {
        _base_address = "http://vault-svc:8200";
        _username = "username";
        _password = "password";
        _rolename = "staging";
        _engine = "kv";
        _secrets_path = "staging/service-svc";
    }

    [TestMethod]
    public void UserpassClientUnitTest()
    {
        Dictionary<string, string> vault_headers = new();

        UserpassClient userpass_client = new(HttpVaultHeaders.Build(vault_headers), _base_address);
        Secret response = userpass_client.LoginAsync(_username, new HashiVaultCs.Models.Requests.Auth.Userpass.Login
        {
            Password = _password
        }, ImmutableDictionary<string, string>.Empty).Result;

        _ = response;
    }

    [TestMethod]
    public void TestMethod()
    {
        Dictionary<string, string> vault_headers = new();

        UserpassClient userpass_client = new(HttpVaultHeaders.Build(vault_headers), _base_address);
        Secret userpass_login_response = userpass_client.LoginAsync(_username, new HashiVaultCs.Models.Requests.Auth.Userpass.Login
        {
            Password = _password
        }, ImmutableDictionary<string, string>.Empty).Result;

        Assert.IsNotNull(userpass_login_response.Auth?.ClientToken);

        vault_headers.Remove(HttpVaultHeaderKey.Token);
        vault_headers.Add(HttpVaultHeaderKey.Token, userpass_login_response.Auth.ClientToken);

        ApproleClient approle_client = new(HttpVaultHeaders.Build(vault_headers), _base_address);

        Secret approle_roleid_response = approle_client.RoleIdAsync(_rolename, ImmutableDictionary<string, string>.Empty).Result;
        string? role_id = approle_roleid_response.Data?.RootElement.GetProperty("role_id").GetString();
        Assert.IsNotNull(role_id);

        Secret approle_secretid_response = approle_client.SecretIdAsync(_rolename, ImmutableDictionary<string, string>.Empty).Result;
        string? secret_id = approle_secretid_response.Data?.RootElement.GetProperty("secret_id").GetString();
        Assert.IsNotNull(secret_id);

        Secret approle_login_response = approle_client.LoginAsync(new HashiVaultCs.Models.Requests.Auth.Approle.Login
        {
            RoleId = role_id,
            SecretId = secret_id
        }, ImmutableDictionary<string, string>.Empty).Result;
        Assert.IsNotNull(approle_login_response.Auth?.ClientToken);

        vault_headers.Remove(HttpVaultHeaderKey.Token);
        vault_headers.Add(HttpVaultHeaderKey.Token, approle_login_response.Auth.ClientToken);

        DataClient data_client = new(HttpVaultHeaders.Build(vault_headers), _base_address);
        Secret data_get_response = data_client.GetAsync(_engine, _secrets_path, ImmutableDictionary<string, string>.Empty).Result;
        string? response_json = data_get_response.Data?.RootElement.GetProperty("data").GetRawText();
        _ = response_json;

    }
}