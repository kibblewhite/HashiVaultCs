namespace HashiVaultCs.Models;

public sealed class JsonDocumentResult
{
    public required JsonDocument Result { get; init; }
    public required bool Failed { get; init; }
    public required string Error { get; init; }

    public static JsonDocumentResult Success(JsonDocument result) => new()
    {
        Error = string.Empty,
        Failed = false,
        Result = result
    };

    public static JsonDocumentResult Failure(string error) => new()
    {
        Error = error,
        Failed = true,
        Result = JsonDocument.Parse("{}")
    };
}
