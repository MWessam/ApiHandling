using System;
using System.Threading;
using ApiHandling.Generated.Facade;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ApiHandling.Runtime
{
    public class ApiCommandChain
    {
        public virtual async UniTask<Result> Execute()
        {
            return Result.Success();
        }
    }

    public interface IApiCommandChain<T>
    {
        bool IsErrorLocalized { get; }
        ApiCommandChain<T> LocalizeError(bool localize);
        ApiCommandChain<T> OnSuccess(Action<T> action);
        ApiCommandChain<T> OnFailure(Action<ErrorMessage> action);
        ApiCommandChain<T> SetTimeout(int timeout);
        ApiCommandChain<T> SetRetryCount(int retryCount);
        UniTask<Result<T>> Fetch();
    }

    public class ApiCommandChain<T> : ApiCommandChain, IApiCommandChain<T>
    {
        private Action<T> _onSuccess;
        private Action<ErrorMessage> _onFailure;
        private IFetchCommand<T> _fetchCommand;
        private CancellationToken _token = default;
        private int _timeout = 0;
        private int _retryCount;
        public bool IsErrorLocalized { get; private set; } = false;
        internal ApiCommandChain(IFetchCommand<T> fetchCommand)
        {
            _fetchCommand = fetchCommand;
        }
        public ApiCommandChain<T> LocalizeError(bool localize)
        {
            IsErrorLocalized = localize;
            return this;
        }

        public ApiCommandChain<T> OnSuccess(Action<T> action)
        {
            _onSuccess = action;
            return this;
        }

        public ApiCommandChain<T> OnFailure(Action<ErrorMessage> action)
        {
            _onFailure = action;
            return this;
        }
        public ApiCommandChain<T> SetTimeout(int timeout)
        {
            _timeout = timeout;
            return this;
        }
        public ApiCommandChain<T> SetRetryCount(int retryCount)
        {
            _retryCount = retryCount;
            return this;
        }

        public virtual async UniTask<Result<T>> Fetch()
        {
            Result<T> result = Result<T>.Failure(EResultError.Unknown, "Unknown error");

            try
            {
                if (_retryCount == 0)
                {
                    result = await _fetchCommand.FetchAsync(_token);
                }
                else
                {
                    var timesRetried = 0;
                    bool shouldRetry = timesRetried < _retryCount;
                    while (shouldRetry)
                    {
                        result = await _fetchCommand.FetchAsync(_token);
                        if (result.IsSuccess)
                        {
                            break;
                        }
                        timesRetried++;
                    }
                }

                if (!result.IsSuccess)
                {
                    Debug.LogError("Error: " + result.Error);
                    if (!IsErrorLocalized)
                    {
                        ApiEventBus<ApiErrorEvent>.Raise(new (result.ErrorMessage));
                    }
                    _onFailure?.Invoke(result.ErrorMessage);
                    return result;
                }
                _onSuccess?.Invoke(result.Value);
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return Result<T>.Failure(EResultError.Unknown, e.Message);
            }

        }

        public override async UniTask<Result> Execute()
        {
            return (await Fetch()).ToResult();
        }

        internal ApiCommandChain<T> SetCancellationToken(CancellationToken token = default)
        {
            _token = token;
            return this;
        }
    }
}
