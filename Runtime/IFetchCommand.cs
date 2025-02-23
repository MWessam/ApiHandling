using System.Threading;
using Cysharp.Threading.Tasks;

namespace ApiHandling.Runtime
{
    public interface IFetchCommand<T> : IApiCommand
    {
        UniTask<Result<T>> FetchAsync(CancellationToken cancellationToken = default);
    }
}
