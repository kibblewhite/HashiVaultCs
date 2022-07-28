namespace Tests;

[TestClass]
public partial class UnitTest
{

    private readonly string _base_address;
    private readonly string _username;
    private readonly string _password;
    private readonly string _rolename;
    private readonly string _engine;
    private readonly string _secrets_path;

    public UnitTest()
    {
        _base_address = "https://localhost:8200";
        _username = "username";
        _password = "password";
        _rolename = "staging";
        _engine = "kv";
        _secrets_path = "staging/service-svc";
    }

    /// <summary>
    /// The code is for demostration purposes.
    /// Follow the comments in the code to understand what it is doing, as long as you have already read the previous README.md
    /// </summary>
    /// <returns></returns>
    private DataClient CreateDataClient()
    {
        // We'll use this to build out HTTP vault headers
        Dictionary<string, string> vault_headers = new();

        // Login to the vault using our userpass authentication creditials
        UserpassClient userpass_client = new(HttpVaultHeaders.Build(vault_headers), _base_address, HttpClientHandler.DangerousAcceptAnyServerCertificateValidator);
        Secret userpass_login_response = userpass_client.Login(_username, new HashiVaultCs.Models.Requests.Auth.Userpass.Login
        {
            Password = _password
        }, ImmutableDictionary<string, string>.Empty);

        // Check that the ClientToken is present and use this next for our Vault Token
        Assert.IsNotNull(userpass_login_response.Auth?.ClientToken);

        // Set the vault header to contain the ClientToken
        vault_headers.Remove(HttpVaultHeaderKey.Token);
        vault_headers.Add(HttpVaultHeaderKey.Token, userpass_login_response.Auth.ClientToken);

        // Create the AppRole client in order to send requests against
        ApproleClient approle_client = new(HttpVaultHeaders.Build(vault_headers), _base_address, HttpClientHandler.DangerousAcceptAnyServerCertificateValidator);

        // Get the role-id and generate a secret-id for the approle 'staging' as logged in user
        Secret approle_roleid_response = approle_client.RoleId(_rolename, ImmutableDictionary<string, string>.Empty);
        string? role_id = approle_roleid_response.Data?.RootElement.GetProperty("role_id").GetString();
        Assert.IsNotNull(role_id);

        // Generate a secret-id
        Secret approle_secretid_response = approle_client.SecretId(_rolename, ImmutableDictionary<string, string>.Empty);
        string? secret_id = approle_secretid_response.Data?.RootElement.GetProperty("secret_id").GetString();
        Assert.IsNotNull(secret_id);

        // Generate a token using the previously retrieved role-id and newly created secret-id. 
        Secret approle_login_response = approle_client.Login(new HashiVaultCs.Models.Requests.Auth.Approle.Login
        {
            RoleId = role_id,
            SecretId = secret_id
        }, ImmutableDictionary<string, string>.Empty);
        Assert.IsNotNull(approle_login_response.Auth?.ClientToken);

        // Set the vault header to contain the newly created ClientToken from the AppRole Login (ApproleClient.LoginAsync(...))
        vault_headers.Remove(HttpVaultHeaderKey.Token);
        vault_headers.Add(HttpVaultHeaderKey.Token, approle_login_response.Auth.ClientToken);

        // Create the Data Client to use for reading and writing kv secrets
        DataClient data_client = new(HttpVaultHeaders.Build(vault_headers), _base_address, HttpClientHandler.DangerousAcceptAnyServerCertificateValidator);
        return data_client;
    }

    [TestMethod]
    public void WriteSecretAndReadTestMethod()
    {
        DataClient data_client = CreateDataClient();

        TestingModel model = new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = Guid.NewGuid().ToString(),
            Password = Guid.NewGuid().ToString(),
            NestedValue = new()
            {
                Value = Guid.NewGuid().ToString()
            }
        };
        SecretData secret_data = new(model, typeof(TestingModel));

        Secret data_post_response = data_client.Post(_engine, _secrets_path, secret_data, ImmutableDictionary<string, string>.Empty);
        bool? destroyed = data_post_response.Data?.RootElement.GetProperty("destroyed").GetBoolean();

        Secret data_get_response = data_client.Get(_engine, _secrets_path, ImmutableDictionary<string, string>.Empty);
        string? response_json = data_get_response.Data?.RootElement.GetProperty("data").GetRawText();

        TestingModel? response_model = JsonSerializer.Deserialize<TestingModel>(response_json ?? "{}");

        Assert.IsFalse(destroyed);
        Assert.AreEqual(model.Id, response_model?.Id);
        Assert.AreEqual(model.Name, response_model?.Name);
        Assert.AreEqual(model.Password, response_model?.Password);
        Assert.AreEqual(model.NestedValue.Value, response_model?.NestedValue?.Value);
    }
}
