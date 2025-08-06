using ConfigurationLib.Interfaces;
using ConfigurationLib.Shared;

namespace ConfigurationLib.Implementations;

public class ConfigProviderFactory : IConfigProviderFactory
{

    public IConfigProvider<T> GetProvider<T>(ConfigStorageType type, string path) where T : class, new()
        => type switch
        {
            ConfigStorageType.Json => new JsonConfigProvider<T>(path),
            ConfigStorageType.Ini => new IniConfigProvider<T>(path),
            ConfigStorageType.Csv => new CsvConfigProvider<T>(path),
            ConfigStorageType.Database => new DatabaseConfigProvider<T>(),
            _ => throw new NotSupportedException($"Unsupported config type: {type}")
        };
}
