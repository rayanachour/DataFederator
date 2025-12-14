using DataFederator.Core.Models;
using DataFederator.Protocols.Modbus;
using DataFederator.Protocols.Modbus.Models;
using Microsoft.Extensions.Options;

namespace DataFederator.App;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly List<ModbusDeviceConfig> _deviceConfigs;
    private readonly List<ModbusTcpDriver> _drivers = [];

    public Worker(
        ILogger<Worker> logger,
        IOptions<List<ModbusDeviceConfig>> deviceConfigs)
    {
        _logger = logger;
        _deviceConfigs = deviceConfigs.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting DataFederator Worker...");

        // Create drivers for each configured device
        foreach (var config in _deviceConfigs)
        {
            var driver = new ModbusTcpDriver(config);
            driver.StateChanged += state => 
                _logger.LogInformation("[{DeviceId}] State changed: {State}", config.DeviceId, state);
            _drivers.Add(driver);
        }

        // Connect to all devices
        foreach (var driver in _drivers)
        {
            var config = _deviceConfigs.First(c => c.DeviceId == driver.DeviceId);
            try
            {
                _logger.LogInformation("[{DeviceId}] Connecting to {Host}:{Port}...", 
                    driver.DeviceId, config.Host, config.Port);
                await driver.ConnectAsync(stoppingToken);
                _logger.LogInformation("[{DeviceId}] Connected successfully!", driver.DeviceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{DeviceId}] Failed to connect: {Message}", 
                    driver.DeviceId, ex.Message);
            }
        }

        // Main polling loop
        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var driver in _drivers)
            {
                if (driver.State != DeviceState.Connected)
                    continue;

                var config = _deviceConfigs.First(c => c.DeviceId == driver.DeviceId);
                var tagIds = config.Tags.Select(t => t.TagId).ToList();

                try
                {
                    var values = await driver.ReadTagsAsync(tagIds, stoppingToken);
                    
                    foreach (var tagValue in values)
                    {
                        var icon = tagValue.Quality == TagQuality.Good ? "✓" : "✗";
                        _logger.LogInformation(
                            "[{DeviceId}] {Icon} {TagId} = {Value} ({Quality})",
                            driver.DeviceId,
                            icon,
                            tagValue.TagId,
                            tagValue.Value,
                            tagValue.Quality);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[{DeviceId}] Error reading tags: {Message}", 
                        driver.DeviceId, ex.Message);
                }

                await Task.Delay(config.PollingIntervalMs, stoppingToken);
            }

            // If no connected drivers, wait a bit before retrying
            if (!_drivers.Any(d => d.State == DeviceState.Connected))
            {
                await Task.Delay(5000, stoppingToken);
            }
        }

        // Cleanup on shutdown
        _logger.LogInformation("Shutting down, disconnecting from devices...");
        foreach (var driver in _drivers)
        {
            await driver.DisposeAsync();
        }
    }
}
