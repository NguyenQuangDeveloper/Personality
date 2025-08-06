using System.IO;
using Newtonsoft.Json;
using UserAccessLib.Common.Interfaces;
using UserAccessLib.Models;
using VSP_88D_CS.Common.Database;

namespace VSP_88D_CS.Common.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserRepository _repo;
        private readonly string _loginCacheFolder;
        private readonly string _loginCacheFilePath;
        public LoginInfo? CurrentUser { get; set; }

        public AuthService()
        {
            _repo = UserRepository.Instance;
            _loginCacheFolder = Path.Combine(
                                 Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                 "VisionSemicon",
                                 "VSP-88D-CS"
                             );
            _loginCacheFilePath = Path.Combine(_loginCacheFolder, "login_cache.json");
        }

        public async Task<LoginInfo?> AuthenticateAsync(string username, string password, bool rememberMe)
        {
            await Task.Delay(100);

            string tempPassword = PasswordHelper.Hash(password);
            string query = $"SELECT * FROM User WHERE Username = @Username and PasswordHash= @PasswordHash";
            query = "Username = @Username and Password= @Password";
            var parameters = new { Username = username, Password = tempPassword };
            var allUser =  await _repo.WhereAsync(query, parameters);
            var user= allUser.FirstOrDefault();
            //var user=allUser.sê
            //var user = _repo.GetUser(query, parameters);
          //  var user = _repo.UserCache.Select(x => x.Value).ToList().FirstOrDefault(x => x.UserName == username && x.Password == tempPassword);
            CurrentUser = user == null ? null : new LoginInfo
            {
                Id = user.Id,
                Username = user.UserName,
                UserRole = user.Role,
                RememberMe = rememberMe,
                Permissions = user.Permissions ?? string.Empty
            };

            if (CurrentUser?.RememberMe == true)
            {
                SaveCache(CurrentUser);
            }

            return CurrentUser;
        }

        public void Logout()
        {
            CurrentUser = null;
            ClearCache();
        }

        private void ClearCache()
        {
            if (File.Exists(_loginCacheFilePath))
                File.Delete(_loginCacheFilePath);
        }

        private void SaveCache(LoginInfo info)
        {
            if (!Directory.Exists(_loginCacheFolder))
                Directory.CreateDirectory(_loginCacheFolder);

            var json = JsonConvert.SerializeObject(info);
            File.WriteAllText(_loginCacheFilePath, json);
        }
    }
}
