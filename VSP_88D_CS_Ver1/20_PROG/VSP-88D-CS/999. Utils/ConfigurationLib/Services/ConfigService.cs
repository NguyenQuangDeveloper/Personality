using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationLib.Interfaces;
using ConfigurationLib.Shared;

namespace ConfigurationLib.Services;

public class ConfigService : IConfigService
{
    private readonly IConfigProviderFactory _factory;

    public ConfigService(IConfigProviderFactory factory)
    {
        _factory = factory;
    }

    public Task<T?> LoadAsync<T>(ConfigStorageType type, string path) where T : class, new()
        => _factory.GetProvider<T>(type, path).LoadAsync();

    public Task SaveAsync<T>(T config, ConfigStorageType type, string path) where T : class, new()
        => _factory.GetProvider<T>(type, path).SaveAsync(config);

}
