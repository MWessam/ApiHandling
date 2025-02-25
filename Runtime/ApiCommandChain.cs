using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ApiHandling.Runtime
{
    public class ApiCommandChain
    {
        public virtual async UniTask<Result> Execute()
        {
            return Result.Success();
        }
    }
    public class ApiCommandChain<T> : ApiCommandChain
    {
        private Action<T> _onSuccess;
        private Action<ErrorMessage> _onFailure;
        private IFetchCommand<T> _fetchCommand;
        private CancellationToken _token = default;
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

        public async UniTask<Result<T>> Fetch()
        {
            var result = await _fetchCommand.FetchAsync(_token);
            if (!result.IsSuccess)
            {
                if (!IsErrorLocalized)
                {
                    EventBus<ApiErrorEvent>.Raise(new (result.ErrorMessage));
                }
                _onFailure?.Invoke(result.ErrorMessage);
                return result;
            }
            _onSuccess?.Invoke(result.Value);
            return result;
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