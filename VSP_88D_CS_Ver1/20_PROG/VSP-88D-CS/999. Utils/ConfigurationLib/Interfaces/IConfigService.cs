using ConfigurationLib.Shared;

namespace ConfigurationLib.Interfaces;

public interface IConfigService
{
    Task<T?> LoadAsync<T>(ConfigStorageType type, string path) where T : class, new();
    Task SaveAsync<T>(T config, ConfigStorageType type, string path) where T : class, new();
}
