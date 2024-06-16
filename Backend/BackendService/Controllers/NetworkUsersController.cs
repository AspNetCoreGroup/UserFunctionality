using BackendCommonLibrary.Interfaces.Services;
using BackendModelLibrary.Model;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
public class NetworkUsersController : ControllerBase
{
    private ILogger Logger { get; }

    private INetworkUsersService NetworkUsersService { get; set; }


    public NetworkUsersController(ILoggerFactory loggerFactory, INetworkUsersService networkUsersService)
    {
        Logger = loggerFactory.CreateLogger<NetworkUsersController>();
        NetworkUsersService = networkUsersService;
    }

    [HttpGet("Networks/{networkID}/Users")]
    public async Task<IActionResult> GetAsync(int networkID)
    {
        var networkUser = await NetworkUsersService.GetNetworkUsersAsync();

        return Ok(networkUser);
    }

    [HttpGet("Networks/{networkID}/Users/{networkUserID}")]
    public async Task<IActionResult> GetAsync(int networkID, int networkUserID)
    {
        var networkUsers = await NetworkUsersService.GetNetworkUserAsync(networkUserID);

        return Ok(networkUsers);
    }

    [HttpPost("Networks/{networkID}/Users")]
    public async Task<IActionResult> AddAsync(int networkID, NetworkUserDto networkUser)
    {
        await NetworkUsersService.CreateNetworkUserAsync(networkUser);

        return Ok();
    }

    [HttpPut("Networks/{networkID}/Users/{networkUserID}")]
    public async Task<IActionResult> UpdateAsync(int networkID, int networkUserID, NetworkUserDto networkUser)
    {
        await NetworkUsersService.UpdateNetworkUserAsync(networkUserID, networkUser);

        return Ok();
    }

    [HttpDelete("Networks/{networkID}/Users/{networkUserID}")]
    public async Task<IActionResult> DeleteAsync(int networkID, int networkUserID)
    {
        await NetworkUsersService.DeleteNetworkUserAsync(networkUserID);

        return Ok();
    }
}