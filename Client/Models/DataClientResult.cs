using HashiVaultCs.Clients.Secrets;

namespace HashiVaultCs.Models;

public sealed class DataClientResult
{
    public required DataClient Client { get; init; }
    public required bool Failed { get; init; }
    public required string Error { get; init; }

    public static DataClientResult Success(DataClient client) => new()
    {
        Client = client,
        Error = string.Empty,
        Failed = false
    };

    public static DataClientResult Failure(string? error) => new()
    {
        Client = default!,
        Error = string.IsNullOrWhiteSpace(error) is true
            ? "No error message provided"
            : error,
        Failed = true
    };
}
