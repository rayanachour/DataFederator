# DataFederator

A .NET 10 industrial data acquisition service that reads from Modbus TCP devices and logs tag values.

## What It Does

DataFederator connects to Modbus devices, reads register values continuously, and outputs them — acting as a bridge between industrial equipment and your software.

## Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/rayanachour/DataFederator.git
cd DataFederator
```

### 2. Run the Modbus Simulator (Terminal 1)

You need a Modbus device to connect to. We provide a Python simulator for testing:

```bash
pip3 install pymodbus
python3 tools/modbus_simulator.py
```

### 3. Run DataFederator (Terminal 2)
```bash
dotnet run --project src/DataFederator.App
```

### Expected Output
```
╔═══════════════════════════════════════════════╗
║         DataFederator - Modbus Reader         ║
║              Press Ctrl+C to stop             ║
╚═══════════════════════════════════════════════╝

[Simulator1] Connecting to 127.0.0.1:5020...
[Simulator1] Connected successfully!
[Simulator1] ✓ Register0 = 523 (Good)
[Simulator1] ✓ Register1 = 187 (Good)
[Simulator1] ✓ Temperature = 25.3 (Good)
```

## Project Structure

```
DataFederator/
├── src/
│   ├── DataFederator.Core/           # Shared interfaces & models
│   │   ├── Interfaces/
│   │   │   └── IProtocolDriver.cs    # Contract for all protocols
│   │   └── Models/
│   │       ├── TagValue.cs           # Universal data container
│   │       ├── TagQuality.cs         # Good/Bad/Uncertain
│   │       └── DeviceState.cs        # Connection states
│   │
│   ├── DataFederator.Protocols.Modbus/   # Modbus implementation
│   │   ├── ModbusTcpDriver.cs        # Main Modbus driver
│   │   └── Models/
│   │       ├── ModbusDeviceConfig.cs # Device settings
│   │       └── ModbusTagConfig.cs    # Tag settings
│   │
│   └── DataFederator.App/            # Entry point
│       ├── Program.cs                # Bootstrap
│       ├── Worker.cs                 # Main polling loop
│       └── appsettings.json          # Configuration
│
├── tests/                            # Unit tests
├── tools/
│   └── modbus_simulator.py           # Test simulator
└── docs/                             # Documentation
```

## Configuration

Edit `src/DataFederator.App/appsettings.json` to configure devices:

```json
{
  "ModbusDevices": [
    {
      "DeviceId": "MyPLC",
      "Host": "192.168.1.100",
      "Port": 502,
      "Tags": [
        { "TagId": "Temperature", "Address": 0, "DataType": "Int16", "Scale": 0.1 }
      ]
    }
  ]
}
```

## Requirements

- .NET 10 SDK
- Python 3 + pymodbus (for simulator only)

## Roadmap

- [x] Phase 1: Modbus TCP Reader
- [ ] Phase 2: OPC UA Server Output
- [ ] Phase 3: PostgreSQL Configuration
- [ ] Phase 4: DNP3 & Allen-Bradley Protocols
- [ ] Phase 5: MQTT & Data Archiving

## License

MIT

