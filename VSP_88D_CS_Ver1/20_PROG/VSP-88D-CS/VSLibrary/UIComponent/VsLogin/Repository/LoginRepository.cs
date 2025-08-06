using VSLibrary.Database;

namespace VSLibrary.UIComponent.VsLogin.Repository;

/// <summary>
/// 로그인 및 사용자 정보를 관리하는 리포지토리 클래스입니다.
/// DB 조회를 최소화하기 위해 내부 캐시(Dictionary)를 사용합니다.
/// </summary>
public class LoginRepository : DynamicRepository<UserItem>
{
    private DBManager _dbManager = null!;
    private static LoginRepository? _instance;

    public static LoginRepository Instance
        => _instance ?? throw new InvalidOperationException("초기화되지 않았습니다.");

    private Dictionary<string, UserItem> _userCache = new(StringComparer.OrdinalIgnoreCase);
    
    public Dictionary<string, UserItem> UserCache { get=> _userCache; set => _userCache = value; }

    /// <summary>
    /// DBManager를 기반으로 싱글톤을 초기화하고 캐시를 자동 로딩합니다.
    /// </summary>
    public static async void AutoLoad(DBManager db)
    {
        if (_instance == null)
        {
            var repo = new LoginRepository(db);
            await repo.InitializeCacheAsync(); // 캐시 자동 로딩
            _instance = repo;
        }

        var user = new UserItem
        {
            UserID = "vs",
            UserName = "관리자",
            Grade = 2,
            Password = "vs",
            Department = "S/W1팀"
        };

        await Instance.UpsertAsync(user);
        Instance.GetAllUsersFromCache();
    }

    /// <summary>
    /// DB 매니저를 기반으로 LoginRepository를 초기화합니다.
    /// </summary>
    /// <param name="db">DBManager 인스턴스</param>
    public LoginRepository(DBManager db) : base(db)
    {
        _dbManager = db ?? throw new ArgumentNullException(nameof(db), "DBManager가 null입니다.");
        _ = EnsureTableAsync(); // 테이블이 없으면 자동 생성 (비동기 호출)
    }

    /// <summary>
    /// 리소스를 명시적으로 해제합니다.
    /// </summary>
    public static void Release()
    {
        _instance?._dbManager?.Dispose();
        _instance = null;
    }

    /// <summary>
    /// DB에서 모든 사용자 정보를 로드하여 캐시에 저장합니다.
    /// 최초 로드시 사용됩니다.
    /// </summary>
    public async Task InitializeCacheAsync()
    {
        _userCache.Clear();
        var users = await GetAllAsync();
        foreach (var user in users)
        {
            _userCache[user.UserID] = user;
        }
    }

    /// <summary>
    /// 사용자 로그인 유효성 검사를 수행합니다.
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <param name="pw">비밀번호</param>
    /// <returns>유효하면 true, 아니면 false</returns>
    public async Task<bool> ValidateAsync(string id, string pw)
    {
        if (_userCache.Count == 0)
            await InitializeCacheAsync();

        return _userCache.TryGetValue(id, out var user) && user.Password == pw;
    }

    /// <summary>
    /// 사용자 정보를 캐시에서 조회합니다.
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <returns>UserItem 또는 null</returns>
    public UserItem? FindUser(string id)
    {
        _userCache.TryGetValue(id, out var user);
        return user;
    }

    /// <summary>
    /// 사용자 정보를 추가하거나 업데이트합니다.
    /// </summary>
    /// <param name="item">UserItem 객체</param>
    /// <returns>성공 여부</returns>
    public async Task<bool> UpsertAsync(UserItem item)
    {
        bool result;

        if (_userCache.ContainsKey(item.UserID))
        {
            result = await UpdateAsync(item);
        }
        else
        {
            result = await InsertAsync(item);
        }

        if (result)
        {
            _userCache[item.UserID] = item;
        }

        return result;
    }

    /// <summary>
    /// 사용자 정보를 삭제합니다.
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <returns>성공 여부</returns>
    public async Task<bool> DeleteUserAsync(string id)
    {
        var result = await DeleteAsync(id);

        if (result)
        {
            _userCache.Remove(id);
        }

        return result;
    }

    /// <summary>
    /// 전체 사용자 캐시를 가져옵니다.
    /// </summary>
    /// <returns>UserItem 목록</returns>
    public IEnumerable<UserItem> GetAllUsersFromCache()
        => _userCache.Values.ToList();
}