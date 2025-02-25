using System;
using System.Collections.Generic;
using System.Threading;

namespace ApiHandling.Runtime
{
    public abstract class BaseApiFacade
    {
        private Queue<ApiCommandChain> _apiCommandQueue = new();
        protected ApiCommandChain<T> CreateApiCommandChain<T>(IFetchCommand<T> fetchCommand, CancellationToken token = default)
        {
            var apiCommandChain = new ApiCommandChain<T>(fetchCommand);
            apiCommandChain.SetCancellationToken(token);
            _apiCommandQueue.Enqueue(apiCommandChain);
            return apiCommandChain;
        }
    }

    public class GenerateApiCommand : Attribute
    {
        
    }
}