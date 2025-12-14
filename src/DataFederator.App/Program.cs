using DataFederator.App;
using DataFederator.Protocols.Modbus.Models;

var builder = Host.CreateApplicationBuilder(args);

// Load Modbus device configurations from appsettings.json
builder.Services.Configure<List<ModbusDeviceConfig>>(
    builder.Configuration.GetSection("ModbusDevices"));

// Register the worker service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

Console.WriteLine("╔═══════════════════════════════════════════════╗");
Console.WriteLine("║         DataFederator - Modbus Reader         ║");
Console.WriteLine("║              Press Ctrl+C to stop             ║");
Console.WriteLine("╚═══════════════════════════════════════════════╝");
Console.WriteLine();

host.Run();
