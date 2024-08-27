using BackendCommonLibrary.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
public class GraphsController : ControllerBase
{
    private ILogger Logger { get; }

    private IGraphsService GraphsService { get; }


    public GraphsController(ILogger<GraphsController> logger, IGraphsService graphsService)
    {
        Logger = logger;
        GraphsService = graphsService;
    }

    [HttpGet("Graph")]
    public async Task<IActionResult> GetAsync(int? networkID, int? deviceID, DateTime? minDateTime, DateTime? maxDateTime)
    {
        if (networkID == null && deviceID == null)
        {
            return BadRequest("Необходимо указать NetworkID или DeviceID");
        }

        var result = await GraphsService.GetGraph(new ModelLibrary.Requests.GraphRequestWrapper()
        {
            NetworkID = networkID,
            DeviceID = deviceID,
            MaxDateTime = maxDateTime ?? DateTime.Now.AddMonths(1),
            MinDateTime = minDateTime ?? DateTime.Now.AddMonths(-1)
        });

        return Ok(result);
    }
}