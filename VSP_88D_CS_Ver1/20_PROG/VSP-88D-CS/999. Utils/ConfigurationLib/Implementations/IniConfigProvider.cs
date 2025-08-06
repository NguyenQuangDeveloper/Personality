using System.Reflection;
using ConfigurationLib.Interfaces;
using ConfigurationLib.Shared;

namespace ConfigurationLib.Implementations;

public class IniConfigProvider<T> : IConfigProvider<T> where T : class, new()
{
    private readonly string _filePath;

    public ConfigStorageType ProviderType => ConfigStorageType.Ini;

    public IniConfigProvider(string filePath)
    {
        _filePath = filePath;
    }

    public Task<T?> LoadAsync()
    {
        var instance = new T();
        if (!File.Exists(_filePath))
        {
            return Task.FromResult<T?>(instance);
        }

        var lines = File.ReadAllLines(_filePath);
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || !line.Contains('=')) continue;

            var split = line.Split('=', 2);
            var key = split[0].Trim();
            var value = split[1].Trim();

            var prop = props.FirstOrDefault(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (prop != null && prop.CanWrite)
            {
                var convertedValue = Convert.ChangeType(value, prop.PropertyType);
                prop.SetValue(instance, convertedValue);
            }
        }

        return Task.FromResult<T?>(instance);
    }
    public Task SaveAsync(T config)
    {
        var lines = new List<string>();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            var value = prop.GetValue(config)?.ToString() ?? string.Empty;
            lines.Add($"{prop.Name}={value}");
        }

        File.WriteAllLines(_filePath, lines);
        return Task.CompletedTask;
    }
}
