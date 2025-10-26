namespace RpWeave.Server.Core.Results;

public class Result : IProcessResult
{
    public bool IsSuccess { get; }
    public Error? Error { get; }
    
    private Result(bool isSuccess, Error? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() =>
        new Result(true);
    
    public static Result Failure(string code, string message) =>
        new Result(false, new Error(code, message));
}