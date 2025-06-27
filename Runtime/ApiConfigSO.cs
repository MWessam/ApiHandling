using System;
using System.Collections.Generic;
using UnityEngine;

namespace ApiHandling.Runtime
{
    [CreateAssetMenu(menuName = "Create ApiConfigSO", fileName = "ApiConfigSO", order = 0)]
    public class ApiConfigSO : ScriptableObject
    {
        public string ApiUrl;
        public List<SocketData> SocketUris;
    }

    [Serializable]
    public class SocketData
    {
        public string SocketName;
        public string SocketUri;
    }
}