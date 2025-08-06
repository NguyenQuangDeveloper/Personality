using VSLibrary.Database;
using VSP_88D_CS.Models.Recipe;

namespace VSP_88D_CS.Common.Database;

public class CleaningRepository : DynamicRepository<CleaningItem>
{
    private readonly DBManager _dbManager = null!;
    private static CleaningRepository? _instance;

    public static CleaningRepository Instance
        => _instance ?? throw new InvalidOperationException("초기화되지 않았습니다.");

    private Dictionary<string, CleaningItem> _cleanItemCache = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, CleaningItem> CleanItemCache { get => _cleanItemCache; set => _cleanItemCache = value; }

    /// <summary>
    /// DBManager를 기반으로 싱글톤을 초기화하고 캐시를 자동 로딩합니다.
    /// </summary>
    public static async void AutoLoad(DBManager db)
    {
        if (_instance == null)
        {
            var repo = new CleaningRepository(db);
            await repo.InitializeCacheAsync(); // 캐시 자동 로딩
            _instance = repo;
        }

        Instance.GetAllItemsFromCache();
    }

    public CleaningRepository(DBManager db) : base(db)
    {
        _dbManager = db ?? throw new ArgumentNullException(nameof(db), "DBManager가 null입니다.");
        _ = EnsureTableAsync();
    }

    public static void Release()
    {
        _instance?._dbManager?.Dispose();
        _instance = null;
    }

    public async Task InitializeCacheAsync()
    {
        _cleanItemCache.Clear();
        var items = await GetAllAsync();
        foreach (var item in items)
        {
            _cleanItemCache[item.Name] = item;
        }
    }

    public async Task<CleaningItem> FindItem(string name)
    {
        var matchedItem = await WhereAsync($"Name = '{name}'");
        return matchedItem.FirstOrDefault() ?? new();
    }

    public async Task<bool> UpsertAsync(CleaningItem item, bool isAdd = false)
    {
        var result = false;
        if (isAdd || !_cleanItemCache.ContainsKey(item.Name))
        {
            item.Id = await _dbManager.InsertAsync(item);
            result = item.Id > 0;
        }
        else
        {
            result = await UpdateAsync(item);
        }

        if (result)
            _cleanItemCache[item.Name] = item;

        return result;
    }

    public async Task<bool> DeleteItemAsync(object id)
    {
        var result = await DeleteAsync(id);

        if (result)
        {
            _cleanItemCache.Remove(id.ToString()!);
        }

        return result;
    }

    public IEnumerable<CleaningItem> GetAllItemsFromCache()
    => _cleanItemCache.Values.ToList();

    public CleaningItem GetCleaningByName(string name)
    {
        return _cleanItemCache.Values.FirstOrDefault(x => x.Name == name) ?? new();
    }
}
