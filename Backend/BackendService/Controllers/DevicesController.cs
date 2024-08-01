using BackendCommonLibrary.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Model;

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
    public Task<IActionResult> AddAsync(DeviceDto device)
    {
        throw new Exception("Добавление устройств доступно только через API получения данных.");

        //await DevicesService.CreateDeviceAsync(device);

        //return Ok();
    }

    [HttpPut("{deviceID}")]
    public Task<IActionResult> UpdateAsync(DeviceDto device, int deviceID)
    {
        throw new Exception("Добавление устройств доступно только через API получения данных.");

        //await DevicesService.UpdateDeviceAsync(deviceID, device);

        //return Ok();
    }

    [HttpDelete("{deviceID}")]
    public Task<IActionResult> DeleteAsync(int deviceID)
    {
        throw new Exception("Добавление устройств доступно только через API получения данных.");

        //await DevicesService.DeleteDeviceAsync(deviceID);

        //return Ok();
    }
}