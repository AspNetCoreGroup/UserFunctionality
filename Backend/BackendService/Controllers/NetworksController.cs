using BackendCommonLibrary.Interfaces.Services;
using BackendModelLibrary.Model;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetAsync()
    {
        var network = await NetworksService.GetNetworksAsync();

        return Ok(network);
    }

    [HttpGet("{networkID}")]
    public async Task<IActionResult> GetAsync(int networkID)
    {
        var networks = await NetworksService.GetNetworkAsync(networkID);

        return Ok(networks);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(NetworkDto network)
    {
        await NetworksService.CreateNetworkAsync(network);

        return Ok();
    }

    [HttpPut("{networkID}")]
    public async Task<IActionResult> UpdateAsync(NetworkDto network, int networkID)
    {
        await NetworksService.UpdateNetworkAsync(networkID, network);

        return Ok();
    }

    [HttpDelete("{networkID}")]
    public async Task<IActionResult> DeleteAsync(int networkID)
    {
        await NetworksService.DeleteNetworkAsync(networkID);

        return Ok();
    }
}