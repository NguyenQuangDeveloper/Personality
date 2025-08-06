namespace VSLibrary.Common.Ini;

/// <summary>
/// Common interface for simple INI configuration file providers.
/// Supports standard key-based access and can handle sections internally.
/// </summary>
public interface IIniProvider
{
    /// <summary>
    /// Loads the INI configuration file from the specified path.
    /// </summary>
    /// <param name="path">Path to the INI file to load.</param>
    void Load(string path);

    /// <summary>
    /// Saves the current configuration state to the specified INI file path.
    /// </summary>
    /// <param name="path">Path to save the INI file to.</param>
    void Save(string path);

    /// <summary>
    /// Gets the string value associated with the specified key.
    /// </summary>
    /// <param name="key">Configuration key (e.g., "Section.Key" or "section:key").</param>
    /// <returns>The value found, or null if not present.</returns>
    string? Get(string key);

    /// <summary>
    /// Sets the string value for the specified key.
    /// </summary>
    /// <param name="key">Configuration key to set.</param>
    /// <param name="value">String value to store.</param>
    void Set(string key, string value);
}

/// <summary>
/// Advanced interface for managing section-based INI configuration files.
/// Provides key-value access, list parsing, raw string output, save, and more.
/// </summary>
public interface IIniManager
{
    /// <summary>
    /// Loads the INI file and parses section/key-value data into memory.
    /// </summary>
    /// <param name="filePath">INI file path.</param>
    void Load(string filePath);

    /// <summary>
    /// Gets the value for the specified section and key.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <param name="key">Key name.</param>
    /// <returns>The configuration value, or null if not found.</returns>
    string? GetValue(string section, string key);

    /// <summary>
    /// Returns all key-value pairs in a section as a comma-separated "key=value" string.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <returns>Comma-separated list of key=value pairs.</returns>
    string? GetRaw(string section);

    /// <summary>
    /// Returns an array of string values, split by a comma or a custom delimiter.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <param name="key">Key name.</param>
    /// <param name="delimiter">Delimiter character (default: ',').</param>
    /// <returns>List of string values.</returns>
    IEnumerable<string> GetList(string section, string key, char delimiter = ',');

    /// <summary>
    /// Sets the value for the specified section and key.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <param name="key">Key name.</param>
    /// <param name="value">Value to store.</param>
    void SetValue(string section, string key, string value);

    /// <summary>
    /// Saves the current configuration to a file.
    /// If no path is specified, saves to the last loaded file path.
    /// </summary>
    /// <param name="filePath">Path to save to (nullable).</param>
    void Save(string? filePath = null);

    /// <summary>
    /// Returns all section names currently present in the INI file.
    /// </summary>
    /// <returns>
    /// An enumerable collection of section names (as strings).
    /// </returns>
    IEnumerable<string> GetSectionNames();

    /// <summary>
    /// Returns all key names under the specified section.
    /// </summary>
    /// <param name="section">Section name.</param>
    /// <returns>An enumerable collection of key names (strings).</returns>
    IEnumerable<string> GetKeys(string section);
}
