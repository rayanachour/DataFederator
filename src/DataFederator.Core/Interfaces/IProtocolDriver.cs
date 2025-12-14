using DataFederator.Core.Models;

namespace DataFederator.Core.Interfaces;

/// <summary>
/// Defines the contract for all protocol drivers (Modbus, DNP3, Allen-Bradley, etc.).
/// Each protocol implements this interface to provide a unified way to read device data.
/// </summary>
public interface IProtocolDriver : IAsyncDisposable
{
    /// <summary>
    /// Unique identifier for this device instance.
    /// </summary>
    string DeviceId { get; }

    /// <summary>
    /// Current connection state of the device.
    /// </summary>
    DeviceState State { get; }

    /// <summary>
    /// Connects to the device asynchronously.
    /// </summary>
    Task ConnectAsync(CancellationToken ct = default);

    /// <summary>
    /// Disconnects from the device gracefully.
    /// </summary>
    Task DisconnectAsync(CancellationToken ct = default);

    /// <summary>
    /// Reads the specified tags from the device.
    /// </summary>
    /// <param name="tagIds">List of tag identifiers to read</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of tag values with quality and timestamp</returns>
    Task<IReadOnlyList<TagValue>> ReadTagsAsync(
        IEnumerable<string> tagIds,
        CancellationToken ct = default);

    /// <summary>
    /// Fired when the device connection state changes.
    /// </summary>
    event Action<DeviceState>? StateChanged;
}

