using UserAccessLib.Common.Enum;

namespace UserAccessLib.Models
{
    public class LoginInfo
    {
        public long Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserRole UserRole { get; set; }
        public string Permissions { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

}
