namespace HashiVaultCs.Models.Responses;

public sealed class InternalOperation<T>
{
    public required T Result { get; init; }
    public bool HasFailed { get; init; }
    public bool IsSuccessful => HasFailed is false;
    public string? ErrorMessage { get; init; }

    public static InternalOperation<T> Failure(string error) => new()
    {
        HasFailed = true,
        ErrorMessage = error,
        Result = default!
    };

    public static InternalOperation<T> Success(T result) => new()
    {
        HasFailed = false,
        Result = result
    };
}
