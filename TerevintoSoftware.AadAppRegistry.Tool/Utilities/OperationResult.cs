using System.Diagnostics.CodeAnalysis;

namespace TerevintoSoftware.AadAppRegistry.Tool.Utilities;

[ExcludeFromCodeCoverage]
public class OperationResult<T>
{
    public T Data { get; }
    public OperationResultStatus Status { get; }
    public string ErrorMessage { get; set; }

    public bool Success => Status == OperationResultStatus.Success;

    public OperationResult(OperationResultStatus status, string errorMessage)
    {
        Status = status;
        ErrorMessage = errorMessage;
    }

    public OperationResult(T data, OperationResultStatus status = OperationResultStatus.Success)
    {
        Data = data;
        Status = status;
    }

    public static implicit operator OperationResult<T>(T value)
    {
        return new(value, OperationResultStatus.Success);
    }

    public static implicit operator OperationResult<T>(OperationResultStatus status)
    {
        return new(status, "");
    }
}

public enum OperationResultStatus
{
    Success = 0,
    AppRegistrationPreviouslyCreated = 1,
    Failed = 2,
    NotFound = 3
}
