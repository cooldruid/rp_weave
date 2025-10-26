namespace RpWeave.Server.Core.Results;

public class ValueResult<TValue> : IProcessResult
{
    public bool IsSuccess { get; }
    public Error? Error { get; }
    public TValue? Value { get; }

    protected ValueResult(bool isSuccess, TValue? value = default, Error? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }

    public static ValueResult<TValue> Success(TValue value) =>
        new ValueResult<TValue>(true, value);
    
    public static ValueResult<TValue> Failure(string code, string message) =>
        new ValueResult<TValue>(false, default, new Error(code, message));
}