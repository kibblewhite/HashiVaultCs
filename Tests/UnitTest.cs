using HashiVaultCs.Extentions;
using HashiVaultCs.Models;

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
    private readonly IHttpClientFactory _http_client_factory;

    public UnitTest()
    {
        _base_address = "https://vault.svc.internal.local:8200";
        _username = "testing";
        _password = "development-password";
        _rolename = "testing";
        _engine = "kv";
        _secrets_path = "testing/service.svc";
        _http_client_factory = new CustomHttpClientFactory();
    }

    [TestMethod]
    public void ReadSecretTestMethod()
    {
        DataClientCredentials data_client_credentials = new(_base_address, _password, _rolename, _username);

        DataClientResult data_client_result = data_client_credentials.CreateDataClient(_http_client_factory);
        Assert.IsFalse(data_client_result.Failed, data_client_result.Error);

        DataClient data_client = data_client_result.Client;
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

        Secret data_get_response = data_client.Get(_engine, _secrets_path, ImmutableDictionary<string, string>.Empty);
        Assert.IsFalse(data_get_response.Failed);

        string? response_json = data_get_response.Data?.RootElement.GetProperty("data").GetRawText();

        //  TestingModel? response_model = JsonSerializer.Deserialize<TestingModel>(response_json ?? "{}");

        //Assert.AreEqual(model.Id, response_model?.Id);
        //Assert.AreEqual(model.Name, response_model?.Name);
        //Assert.AreEqual(model.Password, response_model?.Password);
        //Assert.AreEqual(model.NestedValue.Value, response_model?.NestedValue?.Value);
    }

    [TestMethod]
    public void WriteSecretAndReadTestMethod()
    {
        DataClientCredentials data_client_credentials = new(_base_address, _password, _rolename, _username);

        DataClientResult data_client_result = data_client_credentials.CreateDataClient(_http_client_factory);
        Assert.IsFalse(data_client_result.Failed, data_client_result.Error);

        DataClient data_client = data_client_result.Client;
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
        Assert.IsFalse(data_post_response.Failed);

        bool? destroyed = data_post_response.Data?.RootElement.GetProperty("destroyed").GetBoolean();

        Secret data_get_response = data_client.Get(_engine, _secrets_path, ImmutableDictionary<string, string>.Empty);
        Assert.IsFalse(data_get_response.Failed);

        string? response_json = data_get_response.Data?.RootElement.GetProperty("data").GetRawText();

        TestingModel? response_model = JsonSerializer.Deserialize<TestingModel>(response_json ?? "{}");

        Assert.IsFalse(destroyed);
        Assert.AreEqual(model.Id, response_model?.Id);
        Assert.AreEqual(model.Name, response_model?.Name);
        Assert.AreEqual(model.Password, response_model?.Password);
        Assert.AreEqual(model.NestedValue.Value, response_model?.NestedValue?.Value);
    }
}
