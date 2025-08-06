namespace VSLibrary.Common.Ini;

/// <summary>
/// Enumeration that represents the type of a single line in an INI file.
/// Each line is classified as either a comment, empty, section, or key-value pair.
/// </summary>
public enum IniLineType
{
    /// <summary>
    /// A comment line, starting with semicolon (;), hash (#), or other unofficial formats.
    /// Example: "; this is a comment"
    /// </summary>
    Comment,

    /// <summary>
    /// An empty line.
    /// Included to preserve user formatting.
    /// </summary>
    Empty,

    /// <summary>
    /// A section line, e.g., "[Database]".
    /// The section name is stored in the SectionName field.
    /// </summary>
    Section,

    /// <summary>
    /// A regular key-value line separated by an '=' character.
    /// Example: "username = admin"
    /// </summary>
    KeyValue
}
