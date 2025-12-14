using System.Net.Sockets;
using DataFederator.Core.Interfaces;
using DataFederator.Core.Models;
using DataFederator.Protocols.Modbus.Models;
using NModbus;

namespace DataFederator.Protocols.Modbus;

/// <summary>
/// Modbus TCP protocol driver implementation.
/// Connects to Modbus TCP devices and reads register values.
/// </summary>
public class ModbusTcpDriver : IProtocolDriver
{
    private readonly ModbusDeviceConfig _config;
    private readonly Dictionary<string, ModbusTagConfig> _tagLookup;
    private TcpClient? _tcpClient;
    private IModbusMaster? _modbusMaster;
    private DeviceState _state = DeviceState.Disconnected;

    public string DeviceId => _config.DeviceId;
    
    public DeviceState State
    {
        get => _state;
        private set
        {
            if (_state != value)
            {
                _state = value;
                StateChanged?.Invoke(value);
            }
        }
    }

    public event Action<DeviceState>? StateChanged;

    public ModbusTcpDriver(ModbusDeviceConfig config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _tagLookup = config.Tags.ToDictionary(t => t.TagId, t => t);
    }

    public async Task ConnectAsync(CancellationToken ct = default)
    {
        if (State == DeviceState.Connected)
            return;

        State = DeviceState.Connecting;

        try
        {
            _tcpClient = new TcpClient();
            
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(_config.ConnectionTimeoutMs);

            await _tcpClient.ConnectAsync(_config.Host, _config.Port, cts.Token);
            
            var factory = new ModbusFactory();
            _modbusMaster = factory.CreateMaster(_tcpClient);
            _modbusMaster.Transport.ReadTimeout = _config.ReadTimeoutMs;
            _modbusMaster.Transport.WriteTimeout = _config.ReadTimeoutMs;

            State = DeviceState.Connected;
        }
        catch (Exception)
        {
            State = DeviceState.Error;
            Cleanup();
            throw;
        }
    }

    public Task DisconnectAsync(CancellationToken ct = default)
    {
        Cleanup();
        State = DeviceState.Disconnected;
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<TagValue>> ReadTagsAsync(
        IEnumerable<string> tagIds,
        CancellationToken ct = default)
    {
        if (State != DeviceState.Connected || _modbusMaster is null)
            throw new InvalidOperationException("Not connected to device");

        var results = new List<TagValue>();

        foreach (var tagId in tagIds)
        {
            if (!_tagLookup.TryGetValue(tagId, out var tagConfig))
            {
                results.Add(TagValue.Bad(tagId, "Tag not found in configuration"));
                continue;
            }

            try
            {
                var value = await ReadTagAsync(tagConfig, ct);
                results.Add(TagValue.Good(tagId, value));
            }
            catch (Exception ex)
            {
                results.Add(TagValue.Bad(tagId, ex.Message));
            }
        }

        return results;
    }

    private async Task<object?> ReadTagAsync(ModbusTagConfig tag, CancellationToken ct)
    {
        if (_modbusMaster is null)
            throw new InvalidOperationException("Modbus master not initialized");

        var unitId = _config.UnitId;
        var address = tag.Address;
        var registerCount = GetRegisterCount(tag.DataType);

        return tag.RegisterType switch
        {
            ModbusRegisterType.Coil => 
                (await _modbusMaster.ReadCoilsAsync(unitId, address, 1))[0],
            
            ModbusRegisterType.DiscreteInput => 
                (await _modbusMaster.ReadInputsAsync(unitId, address, 1))[0],
            
            ModbusRegisterType.HoldingRegister => 
                ConvertRegisters(
                    await _modbusMaster.ReadHoldingRegistersAsync(unitId, address, registerCount),
                    tag),
            
            ModbusRegisterType.InputRegister => 
                ConvertRegisters(
                    await _modbusMaster.ReadInputRegistersAsync(unitId, address, registerCount),
                    tag),
            
            _ => throw new NotSupportedException($"Register type {tag.RegisterType} not supported")
        };
    }

    private static ushort GetRegisterCount(ModbusDataType dataType) => dataType switch
    {
        ModbusDataType.Bool => 1,
        ModbusDataType.Int16 => 1,
        ModbusDataType.UInt16 => 1,
        ModbusDataType.Int32 => 2,
        ModbusDataType.UInt32 => 2,
        ModbusDataType.Float32 => 2,
        ModbusDataType.Float64 => 4,
        _ => 1
    };

    private static object ConvertRegisters(ushort[] registers, ModbusTagConfig tag)
    {
        var rawValue = tag.DataType switch
        {
            ModbusDataType.Int16 => (double)(short)registers[0],
            ModbusDataType.UInt16 => (double)registers[0],
            ModbusDataType.Int32 => (double)((registers[0] << 16) | registers[1]),
            ModbusDataType.UInt32 => (double)((uint)(registers[0] << 16) | registers[1]),
            ModbusDataType.Float32 => (double)BitConverter.ToSingle(
                BitConverter.GetBytes((uint)((registers[0] << 16) | registers[1])), 0),
            ModbusDataType.Float64 => BitConverter.ToDouble(
                GetBytesFromRegisters(registers), 0),
            _ => (double)registers[0]
        };

        // Apply scaling: (rawValue * Scale) + Offset
        return (rawValue * tag.Scale) + tag.Offset;
    }

    private static byte[] GetBytesFromRegisters(ushort[] registers)
    {
        var bytes = new byte[registers.Length * 2];
        for (int i = 0; i < registers.Length; i++)
        {
            var regBytes = BitConverter.GetBytes(registers[i]);
            bytes[i * 2] = regBytes[1];
            bytes[i * 2 + 1] = regBytes[0];
        }
        return bytes;
    }

    private void Cleanup()
    {
        _modbusMaster?.Dispose();
        _modbusMaster = null;
        _tcpClient?.Dispose();
        _tcpClient = null;
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
        GC.SuppressFinalize(this);
    }
}

