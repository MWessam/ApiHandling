using UnityEngine;

namespace ApiHandling.Runtime
{
    [CreateAssetMenu(menuName = "Create ApiConfigSO", fileName = "ApiConfigSO", order = 0)]
    internal class ApiConfigSO : ScriptableObject
    {
        public string ApiUrl;
        public string SocketUri;
    }
}