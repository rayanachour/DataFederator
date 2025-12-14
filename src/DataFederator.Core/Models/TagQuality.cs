namespace DataFederator.Core.Models;

/// <summary>
/// Represents the quality status of a tag value.
/// Maps to OPC UA quality codes.
/// </summary>
public enum TagQuality
{
    /// <summary>Value is valid and reliable</summary>
    Good,
    
    /// <summary>Value could not be read or is invalid</summary>
    Bad,
    
    /// <summary>Value was read but reliability is questionable</summary>
    Uncertain
}

