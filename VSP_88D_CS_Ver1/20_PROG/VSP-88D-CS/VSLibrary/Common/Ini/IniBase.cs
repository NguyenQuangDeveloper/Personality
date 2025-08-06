namespace VSLibrary.Common.Ini;

/// <summary>
/// Data structure representing a single line of an INI file.
/// A line can be a comment, blank, section, or key-value pair.
/// </summary>
internal class IniLine
{
    /// <summary>
    /// Data structure representing a single line of an INI file.
    /// A line can be a comment, blank, section, or key-value pair.
    /// </summary>
    public IniLineType Type { get; set; }

    /// <summary>
    /// The original line string.  
    /// Preserves the original format and any inline comments.
    /// </summary>
    public string Raw { get; set; } = string.Empty;

    /// <summary>
    /// The name of the section this line belongs to.  
    /// Valid for Section or KeyValue line types.
    /// </summary>
    public string? SectionName { get; set; }

    /// <summary>
    /// The key of the key-value pair.  
    /// Only valid for KeyValue line types.
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// The value of the key-value pair.  
    /// Only valid for KeyValue line types.
    /// </summary>
    public string? Value { get; set; }
}

/// <summary>
/// Abstract base class for INI configuration files.
/// Provides common key-value storage and Get/Set logic.
/// </summary>
public abstract class IniBase : IIniProvider
{
    /// <summary>
    /// Internal key-value storage.
    /// All configuration entries are stored with normalized keys.
    /// </summary>
    protected readonly Dictionary<string, string> _settings = new();

    /// <summary>
    /// Loads the INI file from the specified path.
    /// </summary>
    /// <param name="path">File path to load from.</param>
    public abstract void Load(string path);

    /// <summary>
    /// Saves the INI file to the specified path.
    /// </summary>
    /// <param name="path">File path to save to.</param>
    public abstract void Save(string path);

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key to look up.</param>
    /// <returns>The value string, or null if not found.</returns>
    public virtual string? Get(string key)
    {
        var normalized = NormalizeKey(key);
        return _settings.TryGetValue(normalized, out var value) ? value : null;
    }

    /// <summary>
    /// Sets the value for the specified key.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="value">The value to store.</param>
    public virtual void Set(string key, string value)
    {
        var normalized = NormalizeKey(key);
        _settings[normalized] = value;
    }

    /// <summary>
    /// Normalizes the key for compatibility with the internal storage.
    /// For example, 'Section.Key' becomes 'section:key'.
    /// </summary>
    /// <param name="key">Input key string.</param>
    /// <returns>Normalized key string (lowercase, dot to colon).</returns>
    protected virtual string NormalizeKey(string key)
    {
        return key
            .Trim()
            .Replace('.', ':')
            .ToLowerInvariant(); 
    }
}
