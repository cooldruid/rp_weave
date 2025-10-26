namespace RpWeave.Server.Core.Results;

public interface IProcessResult
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }
}