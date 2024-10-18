namespace Noizera.Shared.Contracts.Errors;

public class AppException : Exception
{
    public ErrorType ErrorType { get; }
    public ErrorCode ErrorCode { get; }

    public AppException(string? message, ErrorType errorType, ErrorCode errorCode = ErrorCode.Common)
        : base(message) => (ErrorType, ErrorCode) = (errorType, errorCode);

    public int HttpCode => ErrorType switch
    {
        ErrorType.BusinessRule or ErrorType.Validation or ErrorType.NullArgument or ErrorType.BadRequest => 400,
        ErrorType.NotFound => 404,
        ErrorType.Authorization => 403,
        ErrorType.Unknown or ErrorType.Internal or _ => 500,
    };

    public AppException()
    {
    }

    public AppException(string message) : base(message)
    {
    }

    public AppException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
