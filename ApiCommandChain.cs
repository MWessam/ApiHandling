using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public class ApiCommandChain<T>
{
    private Action<T> _onSuccess;
    private Action<ErrorMessage> _onFailure;
    private IFetchCommand<T> _fetchCommand;
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

    public async UniTask<Result<T>> Fetch(CancellationToken cancellationToken = default)
    {
        var result = await _fetchCommand.FetchAsync(cancellationToken);
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
}