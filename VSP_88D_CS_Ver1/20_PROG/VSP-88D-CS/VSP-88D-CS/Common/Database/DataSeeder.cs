namespace VSP_88D_CS.Common.Database;

public class DataSeeder
{
    private readonly UserRepository _userRepository;
   
    public DataSeeder(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void Seed()
    {
        if (_userRepository.UserCache.Any()) return;

        _userRepository.UserCache.Add
            ("admin", new() { UserName = "admin", Password = PasswordHelper.Hash("copper88"), Role = UserAccessLib.Common.Enum.UserRole.Maker });
    }
}
