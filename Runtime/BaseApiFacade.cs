using System.Collections.Generic;

namespace ApiHandling.Runtime
{
    public abstract class BaseApiFacade
    {
        private Queue<IApiCommand> _apiCommandQueue = new();
        protected ApiCommandChain<T> CreateApiCommandChain<T>(IFetchCommand<T> fetchCommand)
        {
            var apiCommandChain = new ApiCommandChain<T>(fetchCommand);
            // _apiCommandQueue.Enqueue();
            return apiCommandChain;
        }
    }
}