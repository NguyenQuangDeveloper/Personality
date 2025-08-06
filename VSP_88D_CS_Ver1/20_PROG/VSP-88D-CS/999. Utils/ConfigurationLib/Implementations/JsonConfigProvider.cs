using Newtonsoft.Json;
using ConfigurationLib.Interfaces;
using ConfigurationLib.Shared;

namespace ConfigurationLib.Implementations;

public class JsonConfigProvider<T> : IConfigProvider<T> where T : class, new()
{
    private readonly string _filePath;

    public ConfigStorageType ProviderType => ConfigStorageType.Json;

    public JsonConfigProvider(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<T?> LoadAsync()
    {
        if (!File.Exists(_filePath))
        {
            var defaultInstance = new T();
            //await SaveAsync(defaultInstance);
            return defaultInstance;
        }

        var json = await File.ReadAllTextAsync(_filePath);

        var settings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        try
        {
            return JsonConvert.DeserializeObject<T>(json, settings) ?? new();
        }
        catch (Exception)
        {
            return new();
        }
    }

    private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);

    public async Task SaveAsync(T config)
    {
        await _fileLock.WaitAsync();

        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);

            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _fileLock.Release();
        }
    }


}
