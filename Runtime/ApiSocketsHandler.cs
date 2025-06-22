using System;
using UnityEngine;

namespace ApiHandling.Runtime
{


    public interface IApiHandler<TDto>
    {
        TDto PostRequest(TDto dto);
        TDto PatchRequest(TDto dto);
        TDto GetRequest(TDto dto);
        TDto DeleteRequest(TDto dto);
    }
}