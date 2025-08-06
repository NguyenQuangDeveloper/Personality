using ConfigurationLib.Shared;

namespace ConfigurationLib.Interfaces;

public interface IConfigProvider<T> where T : class, new()
{
    ConfigStorageType ProviderType { get; }
    Task<T?> LoadAsync();
    Task SaveAsync(T config);
}
