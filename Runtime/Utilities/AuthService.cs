using System;
using VContainer;

namespace ApiHandling.Runtime.Utilities
{
    public interface IAuthService
    {
        string GetToken();
        void SetToken(string token);
        void ClearToken();
        bool HasToken();
    }

    public class AuthService : IAuthService
    {
        private string _jwtToken;

        public string GetToken() => _jwtToken;
        
        public void SetToken(string token)
        {
            _jwtToken = token;
        }
        
        public void ClearToken()
        {
            _jwtToken = null;
        }
        
        public bool HasToken() => !string.IsNullOrEmpty(_jwtToken);
    }
} 