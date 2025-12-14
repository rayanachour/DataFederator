namespace DataFederator.Protocols.Modbus.Models;

/// <summary>
/// Configuration for a Modbus TCP device connection.
/// </summary>
public class ModbusDeviceConfig
{
    /// <summary>
    /// Unique identifier for this device.
    /// </summary>
    public required string DeviceId { get; set; }

    /// <summary>
    /// Display name for the device.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// IP address or hostname of the Modbus device.
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// TCP port (default: 502).
    /// </summary>
    public int Port { get; set; } = 502;

    /// <summary>
    /// Modbus unit/slave ID (default: 1).
    /// </summary>
    public byte UnitId { get; set; } = 1;

    /// <summary>
    /// Connection timeout in milliseconds.
    /// </summary>
    public int ConnectionTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// Read timeout in milliseconds.
    /// </summary>
    public int ReadTimeoutMs { get; set; } = 1000;

    /// <summary>
    /// Polling interval in milliseconds.
    /// </summary>
    public int PollingIntervalMs { get; set; } = 1000;

    /// <summary>
    /// List of tags to read from this device.
    /// </summary>
    public List<ModbusTagConfig> Tags { get; set; } = [];
}

