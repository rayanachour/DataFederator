namespace DataFederator.Core.Models;

/// <summary>
/// Represents a single tag value read from a device.
/// This is the universal data container used across all protocols.
/// </summary>
/// <param name="TagId">Unique identifier for the tag (e.g., "Device1.Register0")</param>
/// <param name="Value">The actual value read (int, float, bool, string, etc.)</param>
/// <param name="Quality">Quality indicator for this reading</param>
/// <param name="Timestamp">When this value was read</param>
public record TagValue(
    string TagId,
    object? Value,
    TagQuality Quality,
    DateTimeOffset Timestamp
)
{
    /// <summary>
    /// Creates a TagValue with Good quality and current timestamp.
    /// </summary>
    public static TagValue Good(string tagId, object? value) =>
        new(tagId, value, TagQuality.Good, DateTimeOffset.UtcNow);

    /// <summary>
    /// Creates a TagValue with Bad quality and current timestamp.
    /// </summary>
    public static TagValue Bad(string tagId, string? errorMessage = null) =>
        new(tagId, errorMessage, TagQuality.Bad, DateTimeOffset.UtcNow);
}

