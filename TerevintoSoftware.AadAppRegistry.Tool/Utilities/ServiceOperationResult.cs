using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace TerevintoSoftware.AadAppRegistry.Tool.Utilities;

[ExcludeFromCodeCoverage]
public class ServiceOperationResult
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ServiceOperationResultStatus Status { get; }
    public string Message { get; set; }

    public bool Success => Status == ServiceOperationResultStatus.Success;

    public ServiceOperationResult(ServiceOperationResultStatus status, string message)
    {
        Status = status;
        Message = message;
    }

    public ServiceOperationResult(ServiceOperationResult other) : this(other.Status, other.Message) { }

    public ServiceOperationResult()
    {
        Status = ServiceOperationResultStatus.Success;
    }

    public static implicit operator ServiceOperationResult(ServiceOperationResultStatus status)
    {
        return new(status, "");
    }
}

[ExcludeFromCodeCoverage]
public class ServiceOperationResult<T> : ServiceOperationResult where T : class
{
    public T Data { get; }

    public ServiceOperationResult(T data) { Data = data; }
    public ServiceOperationResult(ServiceOperationResultStatus status, string message) : base(status, message) { }
    public ServiceOperationResult(T data, ServiceOperationResult other) : base(other) { Data = data; }
    public ServiceOperationResult(ServiceOperationResult other) : base(other.Status, other.Message) { }

    public static implicit operator ServiceOperationResult<T>(T value) => new(value);
    public static implicit operator ServiceOperationResult<T>(ServiceOperationResultStatus status) => new(status, "");
}

public enum ServiceOperationResultStatus
{
    Success = 0,
    AppRegistrationPreviouslyCreated = 1,
    Failed = 2,
    NotFound = 3
}
