namespace DataFederator.Core.Models;

/// <summary>
/// Represents the connection state of a protocol device.
/// </summary>
public enum DeviceState
{
    /// <summary>Not connected to device</summary>
    Disconnected,
    
    /// <summary>Connection attempt in progress</summary>
    Connecting,
    
    /// <summary>Successfully connected and ready</summary>
    Connected,
    
    /// <summary>Connection failed or communication error</summary>
    Error
}

