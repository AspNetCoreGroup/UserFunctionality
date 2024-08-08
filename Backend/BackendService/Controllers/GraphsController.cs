using BackendCommonLibrary.Interfaces.Services;
using BackendService.Services;
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
    public async Task<IActionResult> GetAsync()
    {
        var result = await GraphsService.GetGraph(null);

        return Ok(result);
    }
}