﻿using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

internal class ApiSocketsHandler : MonoBehaviour
{
    // private SocketIOUnity _socketIOUnity;
    void Main()
    {
        var uri = new Uri("https://www.example.com");

    }
}

public interface IApiHandler<TDto>
{
    TDto PostRequest(TDto dto);
    TDto PatchRequest(TDto dto);
    TDto GetRequest(TDto dto);
    TDto DeleteRequest(TDto dto);
}