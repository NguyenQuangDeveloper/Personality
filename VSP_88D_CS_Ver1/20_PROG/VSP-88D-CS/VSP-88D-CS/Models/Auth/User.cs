using System.ComponentModel.DataAnnotations.Schema;
using UserAccessLib.Common.Enum;
using VSLibrary.Database;

namespace VSP_88D_CS.Models.Auth;

[Table(nameof(User))] 
public class User
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public string Password { get; set; } = string.Empty;

    public string Permissions { get; set; } = string.Empty;

}
