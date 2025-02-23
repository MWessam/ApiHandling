using System.Threading;
using Cysharp.Threading.Tasks;

namespace ApiHandling.Runtime
{
    public interface IApiCommand
    {
        UniTask<Result> Execute(CancellationToken cancellationToken = default);
    }
    public abstract class BaseApiCommand<T> : IFetchCommand<T>
    {
        public abstract UniTask<Result<T>> FetchAsync(CancellationToken token = default);
        public async UniTask<Result> Execute(CancellationToken cancellationToken = default)
        {
            return (await FetchAsync()).ToResult();
        }
    }
}