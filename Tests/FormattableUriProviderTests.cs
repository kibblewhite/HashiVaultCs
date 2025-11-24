using HashiVaultCs.Utilities;

namespace Tests;

[TestClass]
public class FormattableUriProviderTests
{
    [TestMethod]
    public void BasicCacheTest()
    {
        // Arrange
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Engine, "my-engine" },
            { FormattableArgument.Path, "my/path" }
        };

        Dictionary<FormattableArgument, string> second_values = new()
        {
            { FormattableArgument.Engine, "my-engine" },
            { FormattableArgument.Path, "my/path" }
        };

        // Act
        InternalOperation<string> result = FormattableUriProvider.SecretsEngineDataPath.GetPath(values);
        InternalOperation<string> second_result = FormattableUriProvider.SecretsEngineDataPath.GetPath(second_values);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("/v1/my-engine/data/my/path", result.Result);

        Assert.IsTrue(second_result.IsSuccessful);
        Assert.AreEqual("/v1/my-engine/data/my/path", second_result.Result);
    }

    [TestMethod]
    public void GetPath_WithValidArguments_ReturnsFormattedUri()
    {
        // Arrange
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Engine, "my-engine" },
            { FormattableArgument.Path, "my/path" }
        };

        // Act
        InternalOperation<string> result = FormattableUriProvider.SecretsEngineDataPath.GetPath(values);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("/v1/my-engine/data/my/path", result.Result);
    }

    [TestMethod]
    public void GetPath_WithCachedUri_ReturnsCachedUri()
    {
        // Arrange
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Engine, "cached-engine" },
            { FormattableArgument.Path, "cached/path" }
        };

        // Act - First call to set cache
        InternalOperation<string> firstResult = FormattableUriProvider.SecretsEngineDataPath.GetPath(values);
        // Second call should use cache
        InternalOperation<string> secondResult = FormattableUriProvider.SecretsEngineDataPath.GetPath(values);

        // Assert
        Assert.IsTrue(firstResult.IsSuccessful);
        Assert.IsTrue(secondResult.IsSuccessful);
        Assert.AreEqual(firstResult.Result, secondResult.Result);
        Assert.AreEqual("/v1/cached-engine/data/cached/path", secondResult.Result);
    }

    [TestMethod]
    public void GetPath_WithMissingArgument_ReturnsFailure()
    {
        // Arrange - Missing FormattableArgument.Path
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Engine, "my-engine" }
        };

        // Act
        InternalOperation<string> result = FormattableUriProvider.SecretsEngineDataPath.GetPath(values);

        // Assert
        Assert.IsTrue(result.HasFailed);
        Assert.IsFalse(result.IsSuccessful);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.IsTrue(result.ErrorMessage.Contains("Missing value for argument"));
    }

    [TestMethod]
    public void GetPath_WithEmptyArgumentValue_ReturnsFailure()
    {
        // Arrange
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Engine, "my-engine" },
            { FormattableArgument.Path, "" } // Empty value
        };

        // Act
        InternalOperation<string> result = FormattableUriProvider.SecretsEngineDataPath.GetPath(values);

        // Assert
        Assert.IsTrue(result.HasFailed);
        Assert.IsFalse(result.IsSuccessful);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.IsTrue(result.ErrorMessage.Contains("Missing value for argument"));
    }

    [TestMethod]
    public void GetPath_WithWhitespaceArgumentValue_ReturnsFailure()
    {
        // Arrange
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Engine, "my-engine" },
            { FormattableArgument.Path, "   " } // Whitespace value
        };

        // Act
        InternalOperation<string> result = FormattableUriProvider.SecretsEngineDataPath.GetPath(values);

        // Assert
        Assert.IsTrue(result.HasFailed);
        Assert.IsFalse(result.IsSuccessful);
        Assert.IsNotNull(result.ErrorMessage);
        Assert.IsTrue(result.ErrorMessage.Contains("Missing value for argument"));
    }

    [TestMethod]
    public void GetPath_AuthUserpassLogin_WithValidArgument_ReturnsFormattedUri()
    {
        // Arrange
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Username, "testuser" }
        };

        // Act
        InternalOperation<string> result = FormattableUriProvider.AuthUserpassLogin.GetPath(values);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("/v1/auth/userpass/login/testuser", result.Result);
    }

    [TestMethod]
    public void GetPath_AuthApproleRoleId_WithValidArgument_ReturnsFormattedUri()
    {
        // Arrange
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Rolename, "testrole" }
        };

        // Act
        InternalOperation<string> result = FormattableUriProvider.AuthApproleRoleId.GetPath(values);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("/v1/auth/approle/role/testrole/role-id", result.Result);
    }

    [TestMethod]
    public void GetPath_AuthApproleSecretId_WithValidArgument_ReturnsFormattedUri()
    {
        // Arrange
        Dictionary<FormattableArgument, string> values = new()
        {
            { FormattableArgument.Rolename, "testrole" }
        };

        // Act
        InternalOperation<string> result = FormattableUriProvider.AuthApproleSecretId.GetPath(values);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("/v1/auth/approle/role/testrole/secret-id", result.Result);
    }

    [TestMethod]
    public void GetPath_AuthApproleLogin_WithNoArguments_ReturnsFormattedUri()
    {
        // Arrange
        Dictionary<FormattableArgument, string> values = [];

        // Act
        InternalOperation<string> result = FormattableUriProvider.AuthApproleLogin.GetPath(values);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("/v1/auth/approle/login", result.Result);
    }

    [TestMethod]
    public void GetFormattableArgumentByDisplayName_WithValidDisplayName_ReturnsCorrectEnum()
    {
        // Act & Assert
        Assert.AreEqual(FormattableArgument.Username, FormattableUriProvider.GetFormattableArgumentByDisplayName("{username}"));
        Assert.AreEqual(FormattableArgument.Rolename, FormattableUriProvider.GetFormattableArgumentByDisplayName("{rolename}"));
        Assert.AreEqual(FormattableArgument.Engine, FormattableUriProvider.GetFormattableArgumentByDisplayName("{engine}"));
        Assert.AreEqual(FormattableArgument.Path, FormattableUriProvider.GetFormattableArgumentByDisplayName("{path}"));
    }

    [TestMethod]
    public void GetFormattableArgumentByDisplayName_WithInvalidDisplayName_ReturnsDefault()
    {
        // Act
        FormattableArgument result = FormattableUriProvider.GetFormattableArgumentByDisplayName("{invalid}");

        // Assert
        Assert.AreEqual(FormattableArgument.Default, result);
    }

    [TestMethod]
    public void GetFormattableArgumentByDisplayName_WithEmptyDisplayName_ReturnsDefault()
    {
        // Act
        FormattableArgument result = FormattableUriProvider.GetFormattableArgumentByDisplayName("");

        // Assert
        Assert.AreEqual(FormattableArgument.Default, result);
    }

    [TestMethod]
    public void GetFormattableArgumentByDisplayName_WithNullDisplayName_ReturnsDefault()
    {
        // Act
        FormattableArgument result = FormattableUriProvider.GetFormattableArgumentByDisplayName(null!);

        // Assert
        Assert.AreEqual(FormattableArgument.Default, result);
    }

    [TestMethod]
    public void GetFormattableArgumentByDisplayName_UsesCache_ForSubsequentCalls()
    {
        // Arrange
        string displayName = "{username}";

        // Act - First call
        FormattableArgument firstResult = FormattableUriProvider.GetFormattableArgumentByDisplayName(displayName);
        // Second call should use cache
        FormattableArgument secondResult = FormattableUriProvider.GetFormattableArgumentByDisplayName(displayName);

        // Assert
        Assert.AreEqual(FormattableArgument.Username, firstResult);
        Assert.AreEqual(firstResult, secondResult);
    }

    [TestMethod]
    public void InternalOperation_Success_CreatesSuccessfulOperation()
    {
        // Act
        InternalOperation<string> operation = InternalOperation<string>.Success("test result");

        // Assert
        Assert.IsTrue(operation.IsSuccessful);
        Assert.IsFalse(operation.HasFailed);
        Assert.AreEqual("test result", operation.Result);
        Assert.IsNull(operation.ErrorMessage);
    }

    [TestMethod]
    public void InternalOperation_Failure_CreatesFailedOperation()
    {
        // Act
        InternalOperation<string> operation = InternalOperation<string>.Failure("test error");

        // Assert
        Assert.IsFalse(operation.IsSuccessful);
        Assert.IsTrue(operation.HasFailed);
        Assert.AreEqual("test error", operation.ErrorMessage);
        Assert.AreEqual(default, operation.Result);
    }
}