using VSLibrary.Database;
using VSP_88D_CS.Models.Recipe;

namespace VSP_88D_CS.Common.Database
{
    /// <summary>
    /// A repository class that manages data in the Device table.
    /// </summary>
    public class DeviceRepository : DynamicRepository<DeviceItem>
    {
        private const string TableName = "DeviceItem";
        private readonly DBManager _dbManager = null!;
        private static DeviceRepository? _instance;
        private static readonly object _lock = new();

        private Dictionary<string, DeviceItem> _deviceCache = new();
        public Dictionary<string, DeviceItem> DeviceCache { get => _deviceCache; set => _deviceCache = value; }

        public static DeviceRepository Instance
            => _instance ?? throw new InvalidOperationException("DeviceRepository is not initialized.");

        /// <summary>
        /// Initializes the singleton instance and preloads the cache.
        /// </summary>
        public static async void AutoLoad(DBManager db)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance != null) return;
                    _instance = new DeviceRepository(db);
                }

                await _instance.InitializeCacheAsync();
            }
        }

        /// <summary>
        /// Release the singleton instance and dispose the DB connection.
        /// </summary>
        public static void Release()
        {
            if (_instance?._dbManager is IDisposable disposable)
                disposable.Dispose();

            _instance = null;
        }

        public DeviceRepository(DBManager databaseManager) : base(databaseManager)
        {
            _dbManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
            _ = EnsureTableAsync();
        }

        /// <summary>
        /// Load all device items from database into memory cache.
        /// </summary>
        public async Task InitializeCacheAsync()
        {
            _deviceCache.Clear();
            var devices = await Task.Run(() => GetAllAsync());
            foreach (var device in devices)
            {
                _deviceCache[device.Name] = device;
            }
        }

        /// <summary>
        /// Get a device by ID from cache.
        /// </summary>
        public DeviceItem? GetDevice(string id)
        {
            _deviceCache.TryGetValue(id, out var device);
            return device;
        }

        /// <summary>
        /// Get a device by name from database.
        /// </summary>
        public DeviceItem? GetDeviceByName(string name)
        {
            return DeviceCache.Values.Where(x=>x.Name == name).FirstOrDefault();
        }

        /// <summary>
        /// Add or update a device item.
        /// </summary>
        public async Task<bool> UpsertDeviceAsync(DeviceItem device)
        {
            bool result;

            if (_deviceCache.ContainsKey(device.Name))
            {
                result = await Task.Run(() => UpdateAsync(device));
            }
            else
            {
                device.Id = await _dbManager.InsertAsync(device);
                result = device.Id > 0;
            }

            if (result)
            {
                _deviceCache[device.Name] = device;
            }

            return result;
        }


        public async Task<bool> DeleteDeviceAsync(string id)
        {
            var result = await DeleteAsync(id);

            if (result)
            {
                _deviceCache.Remove(id);
            }

            return result;
        }

        /// <summary>
        /// Get all devices from cache.
        /// </summary>
        public IEnumerable<DeviceItem> GetAllDevicesFromCache()
            => _deviceCache.Values;

        public async Task<DeviceItem?> GetDeviceById(int id) 
        {
           var result= await GetByIdAsync(id);
           return result;
        }
       
    }

}
