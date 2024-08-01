using BackendCommonLibrary.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Model;

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

    #region Прямой вызов

    [HttpGet("NetworkUsers")]
    public async Task<IActionResult> GetAsync(int requestingUserID)
    {
        var networkUser = await NetworkUsersService.GetNetworkUsersAsync(requestingUserID);

        return Ok(networkUser);
    }

    [HttpGet("NetworkUsers/{networkUserID}")]
    public async Task<IActionResult> GetAsync(int requestingUserID, int networkUserID)
    {
        var networkUsers = await NetworkUsersService.GetNetworkUserAsync(requestingUserID, networkUserID);

        return Ok(networkUsers);
    }

    [HttpPost("NetworkUsers")]
    public async Task<IActionResult> AddAsync(int requestingUserID, NetworkUserDto networkUser)
    {
        await NetworkUsersService.CreateNetworkUserAsync(requestingUserID, networkUser);

        return Ok();
    }

    [HttpPut("NetworkUsers/{networkUserID}")]
    public async Task<IActionResult> UpdateAsync(int requestingUserID, int networkUserID, NetworkUserDto networkUser)
    {
        await NetworkUsersService.UpdateNetworkUserAsync(requestingUserID, networkUserID, networkUser);

        return Ok();
    }

    [HttpDelete("NetworkUsers/{networkUserID}")]
    public async Task<IActionResult> DeleteAsync(int requestingUserID, int networkUserID)
    {
        await NetworkUsersService.DeleteNetworkUserAsync(requestingUserID, networkUserID);

        return Ok();
    }

    #endregion

    #region Вызов через Network

    [HttpGet("Networks/{networkID}/Users")]
    public async Task<IActionResult> Get2Async(int requestingUserID, int networkID)
    {
        var networkUsers = await NetworkUsersService.GetNetworkUsersAsync(requestingUserID, networkID);

        return Ok(networkUsers);
    }

    [HttpGet("Networks/{networkID}/Users/{userID}")]
    public async Task<IActionResult> Get2Async(int requestingUserID, int networkID, int userID)
    {
        var networkUserID = await NetworkUsersService.FindNetworkUserIDAsync(networkID, userID);
        var networkUsers = await NetworkUsersService.GetNetworkUserAsync(requestingUserID, networkUserID);

        return Ok(networkUsers);
    }

    [HttpPost("Networks/{networkID}/Users")]
    public async Task<IActionResult> Add2Async(int requestingUserID, int networkID, NetworkUserDto networkUser)
    {
        ValidateNetworkID(networkUser, networkID);

        await NetworkUsersService.CreateNetworkUserAsync(requestingUserID, networkUser);

        return Ok();
    }

    [HttpPut("Networks/{networkID}/Users/{userID}")]
    public async Task<IActionResult> Update2Async(int requestingUserID, int networkID, int userID, NetworkUserDto networkUser)
    {
        ValidateNetworkID(networkUser, networkID);
        ValidateUserID(networkUser, userID);

        var networkUserID = await NetworkUsersService.FindNetworkUserIDAsync(networkID, userID);
        await NetworkUsersService.UpdateNetworkUserAsync(requestingUserID, networkUserID, networkUser);

        return Ok();
    }

    [HttpDelete("Networks/{networkID}/Users/{userID}")]
    public async Task<IActionResult> Delete2Async(int requestingUserID, int networkID, int userID)
    {
        var networkUserID = await NetworkUsersService.FindNetworkUserIDAsync(networkID, userID);
        await NetworkUsersService.DeleteNetworkUserAsync(requestingUserID, networkUserID);

        return Ok();
    }

    #endregion

    #region Вспомогательное

    private void ValidateNetworkID(NetworkUserDto networkUser, int networkID)
    {
        if (networkUser.NetworkID != networkID)
        {
            throw new Exception("Свойство NetworkUser.NetworkID не совпадает с параметрами запроса.");
        }
    }


    private void ValidateUserID(NetworkUserDto networkUser, int userID)
    {
        if (networkUser.UserID != userID)
        {
            throw new Exception("Свойство NetworkUser.UserID не совпадает с параметрами запроса.");
        }
    }

    #endregion
}