namespace VSLibrary.Common.MVVM.Interfaces;

/// <summary>
/// The RegionAware interface provides functionality to get or set the Region name.
/// </summary>
public interface IRegionAware
{
    /// <summary>
    /// Gets or sets the name of the Region.
    /// </summary>
    string RegionName { get; set; }
}
