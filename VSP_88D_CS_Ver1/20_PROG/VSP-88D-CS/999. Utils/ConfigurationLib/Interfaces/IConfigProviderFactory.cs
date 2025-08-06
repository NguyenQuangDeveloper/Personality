using ConfigurationLib.Shared;

namespace ConfigurationLib.Interfaces;

public interface IConfigProviderFactory
{
    IConfigProvider<T> GetProvider<T>(ConfigStorageType type, string path) where T : class, new();
}
