using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAccessLib.Models;

namespace UserAccessLib.Common.Interfaces
{
    public interface IAuthService
    {
        Task<LoginInfo?> AuthenticateAsync(string username, string password, bool rememberMe);
        void Logout();
    }
}
