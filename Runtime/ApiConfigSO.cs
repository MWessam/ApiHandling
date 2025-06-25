using System.Collections.Generic;
using UnityEngine;

namespace ApiHandling.Runtime
{
    [CreateAssetMenu(menuName = "Create ApiConfigSO", fileName = "ApiConfigSO", order = 0)]
    public class ApiConfigSO : ScriptableObject
    {
        public string ApiUrl;
        public string SocketUri;
        public List<SocketData> SocketUris;
    }

    public class SocketData
    {
        public string SocketName;
        public string SocketUri;
    }
}