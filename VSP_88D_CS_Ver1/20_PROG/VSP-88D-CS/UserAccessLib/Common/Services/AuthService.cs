using UserAccessLib.Common.Enum;
using UserAccessLib.Common.Interfaces;
using UserAccessLib.Models;

namespace UserAccessLib.Common.Services
{
    public class AuthService : IAuthService
    {
       
        private readonly string _loginCacheFolder;
        private readonly string _loginCacheFilePath;

        public LoginInfo? CurrentUser { get; set; }

        public AuthService()
        {
           
        }     
        public async Task<LoginInfo?> AuthenticateAsync(string username, string password, bool rememberMe)
        {
           
            await Task.Delay(500);

            
            if (username == "admin" && password == "123")
                return new LoginInfo { Username = "admin", UserRole = UserRole.Admin };

            if (username == "user" && password == "123")
                return new LoginInfo { Username = "admin", UserRole = UserRole.Operator };

            return null;
        }

        public void Logout()
        {
         
        }
    }
    }
