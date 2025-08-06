using ConfigurationLib.Interfaces;
using ConfigurationLib.Shared;

namespace ConfigurationLib.Implementations;

public class DatabaseConfigProvider<T> : IConfigProvider<T> where T : class, new()
{
    public ConfigStorageType ProviderType => ConfigStorageType.Database;
    public Task<T?> LoadAsync() => Task.FromResult(new T() as T);
    public Task SaveAsync(T config) => Task.CompletedTask;
}
