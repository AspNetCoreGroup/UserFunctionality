using CommonLibrary.Interfaces.Services;
using ModelLibrary.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class DevicesController : ControllerBase
{
    private ILogger Logger { get; }

    private IDevicesService DevicesService { get; set; }


    public DevicesController(ILoggerFactory loggerFactory, IDevicesService devicesService)
    {
        Logger = loggerFactory.CreateLogger<DevicesController>();
        DevicesService = devicesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var device = await DevicesService.GetDevicesAsync();

        return Ok(device);
    }

    [HttpGet("{deviceID}")]
    public async Task<IActionResult> GetAsync(int deviceID)
    {
        var devices = await DevicesService.GetDeviceAsync(deviceID);

        return Ok(devices);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(DeviceDto device)
    {
        await DevicesService.CreateDeviceAsync(device);

        return Ok();
    }

    [HttpPut("{deviceID}")]
    public async Task<IActionResult> UpdateAsync(DeviceDto device, int deviceID)
    {
        await DevicesService.UpdateDeviceAsync(deviceID, device);

        return Ok();
    }

    [HttpDelete("{deviceID}")]
    public async Task<IActionResult> DeleteAsync(int deviceID)
    {
        await DevicesService.DeleteDeviceAsync(deviceID);

        return Ok();
    }
}