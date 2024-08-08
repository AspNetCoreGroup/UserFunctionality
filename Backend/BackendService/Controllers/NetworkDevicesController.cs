using BackendCommonLibrary.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Model;

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

    #region Прямой вызов

    [HttpGet("NetworkDevices")]
    public async Task<IActionResult> GetAsync(int requestingUserID)
    {
        var networkDevice = await NetworkDevicesService.GetNetworkDevicesAsync(requestingUserID);

        return Ok(networkDevice);
    }

    [HttpGet("NetworkDevices/{networkDeviceID}")]
    public async Task<IActionResult> GetAsync(int requestingUserID, int networkDeviceID)
    {
        var networkDevices = await NetworkDevicesService.GetNetworkDeviceAsync(requestingUserID, networkDeviceID);

        return Ok(networkDevices);
    }

    [HttpPost("NetworkDevices")]
    public async Task<IActionResult> AddAsync(int requestingUserID, NetworkDeviceDto networkDevice)
    {
        await NetworkDevicesService.CreateNetworkDeviceAsync(requestingUserID, networkDevice);

        return Ok();
    }

    [HttpPut("NetworkDevices/{networkDeviceID}")]
    public async Task<IActionResult> UpdateAsync(int requestingUserID, int networkDeviceID, NetworkDeviceDto networkDevice)
    {
        await NetworkDevicesService.UpdateNetworkDeviceAsync(requestingUserID, networkDeviceID, networkDevice);

        return Ok();
    }

    [HttpDelete("NetworkDevices/{networkDeviceID}")]
    public async Task<IActionResult> DeleteAsync(int requestingUserID, int networkDeviceID)
    {
        await NetworkDevicesService.DeleteNetworkDeviceAsync(requestingUserID, networkDeviceID);

        return Ok();
    }

    #endregion

    #region Вызов через Network

    [HttpGet("Networks/{networkID}/Devices")]
    public async Task<IActionResult> Get2Async(int requestingUserID, int networkID)
    {
        var networkDevices = await NetworkDevicesService.GetNetworkDevicesAsync(requestingUserID, networkID);

        return Ok(networkDevices);
    }

    [HttpGet("Networks/{networkID}/Devices/{deviceID}")]
    public async Task<IActionResult> Get2Async(int requestingUserID, int networkID, int deviceID)
    {
        var networkDeviceID = await NetworkDevicesService.FindNetworkDeviceIDAsync(networkID, deviceID);
        var networkDevices = await NetworkDevicesService.GetNetworkDeviceAsync(requestingUserID, networkDeviceID);

        return Ok(networkDevices);
    }

    [HttpPost("Networks/{networkID}/Devices")]
    public async Task<IActionResult> Add2Async(int requestingUserID, int networkID, NetworkDeviceDto networkDevice)
    {
        ValidateNetworkID(networkDevice, networkID);

        await NetworkDevicesService.CreateNetworkDeviceAsync(requestingUserID, networkDevice);

        return Ok();
    }

    [HttpPut("Networks/{networkID}/Devices/{deviceID}")]
    public async Task<IActionResult> Update2Async(int requestingUserID, int networkID, int deviceID, NetworkDeviceDto networkDevice)
    {
        ValidateNetworkID(networkDevice, networkID);
        ValidateDeviceID(networkDevice, deviceID);

        var networkDeviceID = await NetworkDevicesService.FindNetworkDeviceIDAsync(networkID, deviceID);
        await NetworkDevicesService.UpdateNetworkDeviceAsync(requestingUserID, networkDeviceID, networkDevice);

        return Ok();
    }

    [HttpDelete("Networks/{networkID}/Devices/{deviceID}")]
    public async Task<IActionResult> Delete2Async(int requestingUserID, int networkID, int deviceID)
    {
        var networkDeviceID = await NetworkDevicesService.FindNetworkDeviceIDAsync(networkID, deviceID);
        await NetworkDevicesService.DeleteNetworkDeviceAsync(requestingUserID, networkDeviceID);

        return Ok();
    }

    #endregion

    #region Вспомогательное

    private void ValidateNetworkID(NetworkDeviceDto networkDevice, int networkID)
    {
        if (networkDevice.NetworkID != networkID)
        {
            throw new Exception("Свойство NetworkDevice.NetworkID не совпадает с параметрами запроса.");
        }
    }

    private void ValidateDeviceID(NetworkDeviceDto networkDevice, int deviceID)
    {
        if (networkDevice.DeviceID != deviceID)
        {
            throw new Exception("Свойство NetworkDevice.DeviceID не совпадает с параметрами запроса.");
        }
    }

    #endregion
}