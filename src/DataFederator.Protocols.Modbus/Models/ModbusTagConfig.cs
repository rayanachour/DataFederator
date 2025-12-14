namespace DataFederator.Protocols.Modbus.Models;

/// <summary>
/// Configuration for a single Modbus tag/register.
/// </summary>
public class ModbusTagConfig
{
    /// <summary>
    /// Unique identifier for this tag.
    /// </summary>
    public required string TagId { get; set; }

    /// <summary>
    /// Display name for the tag.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Modbus register type.
    /// </summary>
    public ModbusRegisterType RegisterType { get; set; } = ModbusRegisterType.HoldingRegister;

    /// <summary>
    /// Starting register address (0-based).
    /// </summary>
    public ushort Address { get; set; }

    /// <summary>
    /// Data type to interpret the register value as.
    /// </summary>
    public ModbusDataType DataType { get; set; } = ModbusDataType.Int16;

    /// <summary>
    /// Scaling factor to apply after reading (value * Scale).
    /// </summary>
    public double Scale { get; set; } = 1.0;

    /// <summary>
    /// Offset to apply after scaling (value * Scale + Offset).
    /// </summary>
    public double Offset { get; set; } = 0.0;
}

/// <summary>
/// Modbus register types.
/// </summary>
public enum ModbusRegisterType
{
    /// <summary>Coils (read/write single bit) - Function codes 01, 05, 15</summary>
    Coil,
    
    /// <summary>Discrete inputs (read-only single bit) - Function code 02</summary>
    DiscreteInput,
    
    /// <summary>Holding registers (read/write 16-bit) - Function codes 03, 06, 16</summary>
    HoldingRegister,
    
    /// <summary>Input registers (read-only 16-bit) - Function code 04</summary>
    InputRegister
}

/// <summary>
/// Data types for interpreting Modbus register values.
/// </summary>
public enum ModbusDataType
{
    /// <summary>Single bit (from coil or discrete input)</summary>
    Bool,
    
    /// <summary>Signed 16-bit integer (1 register)</summary>
    Int16,
    
    /// <summary>Unsigned 16-bit integer (1 register)</summary>
    UInt16,
    
    /// <summary>Signed 32-bit integer (2 registers)</summary>
    Int32,
    
    /// <summary>Unsigned 32-bit integer (2 registers)</summary>
    UInt32,
    
    /// <summary>32-bit floating point (2 registers)</summary>
    Float32,
    
    /// <summary>64-bit floating point (4 registers)</summary>
    Float64
}

