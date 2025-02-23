using System;

namespace ApiHandling.Runtime
{
    public readonly struct Result
    {
        public ErrorMessage Error { get; }
        public bool IsSuccess { get; }

        private Result(EResultError errorType = EResultError.None, string errorMessage = "")
        {
            // E = errorCode;
            Error = new(errorMessage, errorType);
            IsSuccess = errorType == EResultError.None ? true : false;
        }
        public static Result Success() => new();
        public static Result Failure(EResultError errorCode, string errorMessage) =>
            new Result(errorCode, errorMessage);
    }
    public readonly struct Result<T>
    {
        private readonly Result _result;
        public T Value { get; }
        public string Error => _result.Error.Message;
        public EResultError ErrorCode => _result.Error.ErrorType;
        public ErrorMessage ErrorMessage => _result.Error;
        public bool IsSuccess => _result.IsSuccess;

        private Result(T value)
        {
            Value = value;
            _result = Result.Success();
        }

        private Result(EResultError errorCode, string errorMessage)
        {
            Value = default;
            _result = Result.Failure(errorCode, errorMessage);

        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(EResultError errorCode, string errorMessage) => new(errorCode, errorMessage);
        public static Result<T> Failure(ErrorMessage resultErrorMessage) =>
            new(resultErrorMessage.ErrorType, resultErrorMessage.Message);
    
        public Result<T> OnSuccess(Action<T> action)
        {
            if (IsSuccess) action(Value);
            return this;
        }

        // Runs a side-effect if failed
        public Result<T> OnFailure(Action<string> action)
        {
            if (!IsSuccess) action(Error);
            return this;
        }

        public Result ToResult()
        {
            return _result;
        }
    }

    public struct ErrorMessage
    {
        public string Message;
        public EResultError ErrorType;

        public ErrorMessage(string message, EResultError errorType)
        {
            Message = message;
            ErrorType = errorType;
        }
    }
    public enum EResultError
    {
        None,
        NotFound,
        InvalidInput,
        Unauthorized,
        Timeout,
        // Add more error types as needed
        ProtocolError
    }
}