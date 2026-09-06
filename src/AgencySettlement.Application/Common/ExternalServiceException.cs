namespace AgencySettlement.Application.Common;

public sealed class ExternalServiceException(
    string code,
    string message,
    int statusCode,
    Exception? innerException = null) : Exception(message, innerException)
{
    public string Code { get; } = code;
    public int StatusCode { get; } = statusCode;
}
