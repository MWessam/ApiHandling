using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public interface IFetchCommand<T> : IApiCommand
{
    UniTask<Result<T>> FetchAsync(CancellationToken cancellationToken = default);
}
