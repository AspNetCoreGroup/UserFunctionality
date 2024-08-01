using BackendCommonLibrary.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Model;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class NetworksController : ControllerBase
{
    private ILogger Logger { get; }

    private INetworksService NetworksService { get; set; }


    public NetworksController(ILoggerFactory loggerFactory, INetworksService networksService)
    {
        Logger = loggerFactory.CreateLogger<NetworksController>();
        NetworksService = networksService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(int requestingUserID)
    {
        var network = await NetworksService.GetNetworksAsync(requestingUserID);

        return Ok(network);
    }

    [HttpGet("{networkID}")]
    public async Task<IActionResult> GetAsync(int requestingUserID, int networkID)
    {
        var networks = await NetworksService.GetNetworkAsync(requestingUserID, networkID);

        return Ok(networks);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int requestingUserID, NetworkDto network)
    {
        await NetworksService.CreateNetworkAsync(requestingUserID, network);

        return Ok();
    }

    [HttpPut("{networkID}")]
    public async Task<IActionResult> UpdateAsync(int requestingUserID, int networkID, NetworkDto network)
    {
        await NetworksService.UpdateNetworkAsync(requestingUserID, networkID, network);

        return Ok();
    }

    [HttpDelete("{networkID}")]
    public async Task<IActionResult> DeleteAsync(int requestingUserID, int networkID)
    {
        await NetworksService.DeleteNetworkAsync(requestingUserID, networkID);

        return Ok();
    }
}