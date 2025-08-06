using VSLibrary.Database;
using VSP_88D_CS.Models.Recipe;

namespace VSP_88D_CS.Common.Database
{
    /// <summary>
    /// A repository class that queries and manages data in the Recipe table.
    /// </summary>
    public class RecipeRepository : DynamicRepository<RecipeItem>
    {
        private readonly DBManager _dbManager = null!;
        private static RecipeRepository? _instance;

        public static RecipeRepository Instance
            => _instance ?? throw new InvalidOperationException("초기화되지 않았습니다.");

        private Dictionary<string, RecipeItem> _recipeCache = new(StringComparer.OrdinalIgnoreCase);

        public Dictionary<string, RecipeItem> RecipeCache { get => _recipeCache; set => _recipeCache = value; }

        /// <summary>
        /// DBManager를 기반으로 싱글톤을 초기화하고 캐시를 자동 로딩합니다.
        /// </summary>
        public static async void AutoLoad(DBManager db)
        {
            if (_instance == null)
            {
                var repo = new RecipeRepository(db);
                await repo.InitializeCacheAsync(); // 캐시 자동 로딩
                _instance = repo;
            }

            Instance.GetAllRecipesFromCache();
        }

        /// <summary>
        /// DB 매니저를 기반으로 RecipeRepository를 초기화합니다.
        /// </summary>
        /// <param name="db">DBManager 인스턴스</param>
        public RecipeRepository(DBManager db) : base(db)
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
            _recipeCache.Clear();
            var recipes = await GetAllAsync();
            foreach (var recipe in recipes)
            {
                _recipeCache[recipe.Id.ToString()] = recipe;
            }
        }

        /// <summary>
        /// 사용자 정보를 캐시에서 조회합니다.
        /// </summary>
        /// <param name="id">사용자 ID</param>
        /// <returns>RecipeItem 또는 null</returns>
        public RecipeItem? FindRecipe(string id)
        {
            _recipeCache.TryGetValue(id, out var user);
            return user;
        }

        /// <summary>
        /// 사용자 정보를 추가하거나 업데이트합니다.
        /// </summary>
        /// <param name="item">RecipeItem 객체</param>
        /// <returns>성공 여부</returns>
        public async Task<bool> UpsertAsync(RecipeItem item)
        {
            bool result;

            if (_recipeCache.ContainsKey(item.Recipe))
            {
                result = await UpdateAsync(item);
            }
            else
            {
                result = await InsertAsync(item);
            }

            if (result)
            {
                _recipeCache[item.Recipe] = item;
            }

            return result;
        }

        public async Task<bool> DeleteRecipeAsync(RecipeItem recipe)
        {
            var result = await DeleteAsync(recipe.Id);

            if (result)
            {
                _recipeCache.Remove(recipe.Recipe);
            }

            return result;
        }

        public async Task<int> InsertGetIdAsync(RecipeItem item )
        {
            return await _dbManager.InsertAsync(item);
        }

        /// <summary>
        /// 전체 사용자 캐시를 가져옵니다.
        /// </summary>
        /// <returns>RecipeItem 목록</returns>
        public IEnumerable<RecipeItem> GetAllRecipesFromCache()
            => _recipeCache.Values.ToList();

    }
}
