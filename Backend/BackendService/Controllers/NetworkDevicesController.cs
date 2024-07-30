using CommonLibrary.Interfaces.Services;
using ModelLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers;

[ApiController]
public class NetworkDevicesController : ControllerBase
{
    private ILogger Logger { get; }

    private INetworkDevicesService NetworkDevicesService { get; set; }


    public NetworkDevicesController(ILoggerFactory loggerFactory, INetworkDevicesService networkDevicesService)
    {
        Logger = loggerFactory.CreateLogger<NetworkDevicesController>();
        NetworkDevicesService = networkDevicesService;
    }

    [HttpGet("Networks/{networkID}/Devices")]
    public async Task<IActionResult> GetAsync(int networkID)
    {
        var networkDevice = await NetworkDevicesService.GetNetworkDevicesAsync();

        return Ok(networkDevice);
    }

    [HttpGet("Networks/{networkID}/Devices/{networkDeviceID}")]
    public async Task<IActionResult> GetAsync(int networkID, int networkDeviceID)
    {
        var networkDevices = await NetworkDevicesService.GetNetworkDeviceAsync(networkDeviceID);

        return Ok(networkDevices);
    }

    [HttpPost("Networks/{networkID}/devices")]
    public async Task<IActionResult> AddAsync(int networkID, NetworkDeviceDto networkDevice)
    {
        await NetworkDevicesService.CreateNetworkDeviceAsync(networkDevice);

        return Ok();
    }

    [HttpPut("Networks/{networkID}/Devices/{networkDeviceID}")]
    public async Task<IActionResult> UpdateAsync(int networkID, int networkDeviceID, NetworkDeviceDto networkDevice)
    {
        await NetworkDevicesService.UpdateNetworkDeviceAsync(networkDeviceID, networkDevice);

        return Ok();
    }

    [HttpDelete("Networks/{networkID}/Devices/{networkDeviceID}")]
    public async Task<IActionResult> DeleteAsync(int networkID, int networkDeviceID)
    {
        await NetworkDevicesService.DeleteNetworkDeviceAsync(networkDeviceID);

        return Ok();
    }
}