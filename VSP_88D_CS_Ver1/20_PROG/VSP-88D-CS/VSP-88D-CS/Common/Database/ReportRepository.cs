using System.Data;
using VSLibrary.Common.MVVM.Interfaces;
using VSLibrary.Database;
using VSLibrary.UIComponent.VsLogin;
using VSLibrary.UIComponent.VsLogin.Repository;
using VSP_88D_CS.Models.Report;

namespace VSP_88D_CS.Common.Database;

public class ReportRepository : DynamicRepository<ReportLog>
{
    private readonly DBManager _dbManager = null!;
    private static ReportRepository? _instance;

    public static ReportRepository Instance
        => _instance ?? throw new InvalidOperationException("초기화되지 않았습니다.");

    private Dictionary<string, ReportLog> _reportCache = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, ReportLog> ReportCache { get => _reportCache; set => _reportCache = value; }

    /// <summary>
    /// DBManager를 기반으로 싱글톤을 초기화하고 캐시를 자동 로딩합니다.
    /// </summary>
    public static async void AutoLoad(DBManager db)
    {
        if (_instance == null)
        {
            var repo = new ReportRepository(db);
            await repo.InitializeCacheAsync(); // 캐시 자동 로딩
            _instance = repo;
        }

        Instance.GetAllReportFromCache();
    }

    /// <summary>
    /// DB 매니저를 기반으로 ReportRepository를 초기화합니다.
    /// </summary>
    /// <param name="db">DBManager 인스턴스</param>
    public ReportRepository(DBManager db) : base(db)
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
        _reportCache.Clear();
        var reports = await GetAllAsync();
        foreach (var report in reports)
        {
            _reportCache[report.Id.ToString()] = report;
        }
    }

    /// <summary>
    /// 사용자 정보를 캐시에서 조회합니다.
    /// </summary>
    /// <param name="id">사용자 ID</param>
    /// <returns>ReportLog 또는 null</returns>
    public ReportLog? FindUser(string id)
    {
        _reportCache.TryGetValue(id, out var user);
        return user;
    }

    /// <summary>
    /// 사용자 정보를 추가하거나 업데이트합니다.
    /// </summary>
    /// <param name="item">ReportLog 객체</param>
    /// <returns>성공 여부</returns>
    public async Task<bool> UpsertAsync(ReportLog item)
    {
        bool result;

        if (_reportCache.ContainsKey(item.Id.ToString()))
        {
            result = await UpdateAsync(item);
        }
        else
        {
            result = await InsertAsync(item);
        }

        if (result)
        {
            _reportCache[item.Id.ToString()] = item;
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
            _reportCache.Remove(id);
        }

        return result;
    }

    /// <summary>
    /// 전체 사용자 캐시를 가져옵니다.
    /// </summary>
    /// <returns>ReportLog 목록</returns>
    public IEnumerable<ReportLog> GetAllReportFromCache()
        => _reportCache.Values.ToList();

    public async Task<T?> GetReportAsync<T>(string queryOption, string queryWhere, params IDbDataParameter[] parameters) where T : new()
    {
        var results = await GetReportsAsync<T>(queryOption, queryWhere, parameters);
        return results.FirstOrDefault();
    }

    public async Task<IEnumerable<T>> GetReportsAsync<T>(string queryOption, string queryWhere, params IDbDataParameter[] parameters) where T : new()
    {
        string query = $"SELECT {queryOption} FROM {typeof(ReportLog).Name} {queryWhere}";

        var results = await _dbManager.QueryAsync<T>(query, parameters);
        return results;
    }

    public async Task<IEnumerable<T>> GetReportsAsync<T>(string query,  params IDbDataParameter[] parameters) where T : new()
    {
        string qry = query;

        var results = await _dbManager.QueryAsync<T>(qry, parameters);
        return results;
    }

}
