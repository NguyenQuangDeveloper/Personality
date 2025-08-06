using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using VSLibrary.Common.Log;
using VSLibrary.Common.MVVM.Interfaces;

namespace VSLibrary.Common.Ini;

/// <summary>
/// Static class for globally managing INI configuration.
/// Can be accessed via DI container or standalone initialization.
/// </summary>
public static class VsIniManager
{
    /// <summary>
    /// Internal proxy for INI management operations.
    /// </summary>
    private static IIniManager? _proxy;

    /// <summary>
    /// External DI container instance for injection (optional).
    /// </summary>
    private static IContainer? _container;

    /// <summary>
    /// Sets the DI container and registers the internal INI manager.
    /// </summary>
    /// <param name="container">DI container instance.</param>
    public static void SetContainer(IContainer container)
    {
        _container = container;
        _proxy = new VsIniManagerProxy(); 
        _container.RegisterInstance<IIniManager>(_proxy);
    }

    /// <summary>
    /// Initializes the INI manager directly (without DI).
    /// </summary>
    /// <param name="path">INI file path.</param>
    public static void Initialize(string path)
    {
        _proxy ??= new VsIniManagerProxy();
        _proxy.Load(path);
    }

    /// <summary>
    /// Loads the INI file.
    /// </summary>
    /// <param name="path">INI file path.</param>
    public static void Load(string path) => _proxy?.Load(path);

    /// <summary>
    /// Gets the configuration value for the specified section and key.
    /// </summary>
    /// <param name="section">INI section name.</param>
    /// <param name="key">INI key name.</param>
    /// <returns>Configuration value string, or null if not found.</returns>
    public static string? Get(string section, string key) => _proxy?.GetValue(section, key);

    /// <summary>
    /// Sets the configuration value. Overwrites if the key already exists.
    /// Throws an exception if the INI manager has not been initialized.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <param name="key">Key name.</param>
    /// <param name="value">Value string.</param>
    public static void Set(string section, string key, string value)
    {
        if (_proxy == null)
            throw new InvalidOperationException("IniManager is not initialized. Please call Load or Initialize first.");
        _proxy.SetValue(section, key, value);
    }

    /// <summary>
    /// Gets a list of values, splitting the configuration string by the specified delimiter (comma by default).
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <param name="key">Key name.</param>
    /// <param name="delimiter">Delimiter character (default: ',').</param>
    /// <returns>List of strings.</returns>
    public static IEnumerable<string> GetList(string section, string key, char delimiter = ',') =>
        _proxy?.GetList(section, key, delimiter) ?? Enumerable.Empty<string>();

    /// <summary>
    /// Saves the configuration to file.
    /// </summary>
    /// <param name="path">File path to save to (if null, uses the initial loading path).</param>
    public static void Save(string? path = null) => _proxy?.Save(path);

    /// <summary>
    /// Returns all section names currently present in the loaded INI file.
    /// </summary>
    /// <returns>
    /// An enumerable collection of section names (as strings).
    /// </returns>
    public static IEnumerable<string> GetSectionNames()
        => _proxy?.GetSectionNames() ?? Enumerable.Empty<string>();

    /// <summary>
    /// Returns all key names under the specified section.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <returns>An enumerable collection of key names (strings).</returns>
    public static IEnumerable<string> GetKeys(string section)
        => _proxy?.GetKeys(section) ?? Enumerable.Empty<string>();
}

/// <summary>
/// Implementation class responsible for loading and saving actual INI files and managing internal cache.
/// Maintains all sections and key-value pairs in memory, preserving comments and structure.
/// </summary>
public class VsIniManagerProxy : IIniManager
{
    /// <summary>
    /// Stores all lines from the INI file in order, including comments, blanks, sections, and key/value lines.
    /// </summary>
    private readonly List<IniLine> _lines = new();

    /// <summary>
    /// Cache dictionary for quick lookup of sections and key-value pairs.
    /// Used during value access in logic.
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, string>> _data = new();

    /// <summary>
    /// Full path of the currently loaded INI file.
    /// Used as the base path when saving.
    /// </summary>
    private string? _filePath;

    /// <summary>
    /// Loads the INI file, parses section/key-value/comment info, and stores them in memory.
    /// </summary>
    /// <param name="filePath">Path to the INI file to load.</param>
    public void Load(string filePath)
    {
        _filePath = filePath;
        _data.Clear();
        _lines.Clear();

        string? currentSection = null;
        var sectionRegex = new Regex(@"\[(.*?)\]"); // 섹션 이름만 뽑음

        foreach (var rawLine in File.ReadAllLines(filePath))
        {
            var line = rawLine.Trim();

            if (string.IsNullOrWhiteSpace(line))
            {
                _lines.Add(new IniLine { Type = IniLineType.Empty, Raw = rawLine });
            }
            else
            {
                var match = sectionRegex.Match(line);
                if (match.Success && match.Index == 0)
                {
                    currentSection = match.Groups[1].Value.Trim();

                    if (!_data.ContainsKey(currentSection))
                        _data[currentSection] = new Dictionary<string, string>();

                    _lines.Add(new IniLine
                    {
                        Type = IniLineType.Section,
                        Raw = rawLine,
                        SectionName = currentSection
                    });
                }
                else if (line.Contains('=') && !line.Contains(';'))
                {
                    if (currentSection == null)
                        continue;

                    var idx = line.IndexOf('=');
                    var key = line[..idx].Trim();
                    var value = line[(idx + 1)..].Trim();

                    if (!_data.ContainsKey(currentSection))
                        _data[currentSection] = new Dictionary<string, string>();

                    if (_data[currentSection].ContainsKey(key))
                    {
                        LogManager.WriteDirect(@"D:\Logs\VsLog.txt", $"[{filePath}] Key \"{key}\" already exists in section [{currentSection}]. Keeping previous value \"{_data[currentSection][key]}\", new value \"{value}\" is ignored and commented.", LogType.Warn);
                        _lines.Add(new IniLine { Type = IniLineType.Comment, Raw = rawLine });
                        continue;
                    }

                    _data[currentSection][key] = value;

                    _lines.Add(new IniLine
                    {
                        Type = IniLineType.KeyValue,
                        Raw = rawLine,
                        SectionName = currentSection,
                        Key = key,
                        Value = value
                    });
                }
                else
                {
                    _lines.Add(new IniLine { Type = IniLineType.Comment, Raw = rawLine });
                }
            }
        }
    }


    /// <summary>
    /// Retrieves the value for the given section and key.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <param name="key">Key name.</param>
    /// <returns>Configuration value string, or null if not found.</returns>
    public string? GetValue(string section, string key)
        => _data.TryGetValue(section, out var dict) && dict.TryGetValue(key, out var val) ? val : null;

    /// <summary>
    /// Returns the value as a string array, split by the specified delimiter (default: comma).
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <param name="key">Key name.</param>
    /// <param name="delimiter">Delimiter character (default: comma).</param>
    /// <returns>String array, or empty array if value is not found.</returns>
    public IEnumerable<string> GetList(string section, string key, char delimiter = ',')
        => GetValue(section, key)?.Split(delimiter, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

    /// <summary>
    /// Sets the value for the specified section and key.  
    /// Overwrites if the key already exists.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <param name="key">Key name.</param>
    /// <param name="value">Value to store.</param>
    public void SetValue(string section, string key, string value)
    {
        if (!_data.ContainsKey(section))
            _data[section] = new Dictionary<string, string>();

        _data[section][key] = value;

        bool found = false;
        string? currentSection = null;
        for (int i = 0; i < _lines.Count; i++)
        {
            var line = _lines[i];
          
            if (line.Type == IniLineType.Section)
            {
                currentSection = line.SectionName;
            }
           
            if (line.Type == IniLineType.KeyValue && currentSection == section && line.Key == key)
            {
                _lines[i] = new IniLine
                {
                    Type = IniLineType.KeyValue,
                    Raw = $"{key}={value}",
                    SectionName = section,
                    Key = key,
                    Value = value
                };
                found = true;
                break;
            }
        }
               
        if (!found)
        {           
            if (!_lines.Any(l => l.Type == IniLineType.Section && l.SectionName == section))
            {
                _lines.Add(new IniLine
                {
                    Type = IniLineType.Section,
                    Raw = $"[{section}]",
                    SectionName = section
                });
            }
            _lines.Add(new IniLine
            {
                Type = IniLineType.KeyValue,
                Raw = $"{key}={value}",
                SectionName = section,
                Key = key,
                Value = value
            });
        }
    }

    /// <summary>
    /// Saves the INI data currently in memory to a file.
    /// Comments and line order are preserved.
    /// </summary>
    /// <param name="path">File path to save to. If null, saves to the last loaded file path.</param>
    public void Save(string? path = null)
    {
        path ??= _filePath;
        if (path == null) return;

        var sb = new StringBuilder();

        foreach (var line in _lines)
        {
            switch (line.Type)
            {
                case IniLineType.Comment:
                    var comment = line.Raw.TrimStart();
                    if (!comment.StartsWith(";") && !comment.StartsWith("#"))
                        sb.AppendLine($"; {line.Raw}");
                    else
                        sb.AppendLine(line.Raw);
                    break;
                case IniLineType.Empty:                      
                    sb.AppendLine(line.Raw);
                    break;

                case IniLineType.Section:
                    sb.AppendLine($"[{line.SectionName}]");
                    break;

                case IniLineType.KeyValue:
                    if (line.Key != null && line.Value != null)
                        sb.AppendLine($"{line.Key}={line.Value}");
                    break;
            }
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
    }

    /// <summary>
    /// Returns all key-value pairs in the given section as a comma-separated "key=value" string.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <returns>Comma-separated string or null if not found.</returns>
    public string? GetRaw(string section)
    {
        if (_data.TryGetValue(section, out var dict))
        {
            return string.Join(",", dict.Select(kvp => $"{kvp.Key}={kvp.Value}"));
        }

        return null;
    }

    /// <summary>
    /// Returns all section names currently present in the INI file.
    /// </summary>
    /// <returns>
    /// An enumerable collection of section names (as strings).
    /// </returns>
    public IEnumerable<string> GetSectionNames()
    {
        return _data.Keys;
    }

    /// <summary>
    /// Returns all key names under the specified section.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <returns>An enumerable collection of key names (strings).</returns>
    public IEnumerable<string> GetKeys(string section)
    {
        return _data.TryGetValue(section, out var dict) ? dict.Keys : Enumerable.Empty<string>();
    }

}
