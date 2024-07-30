using BackendService.DataSources;
using BackendService.Model.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class GraphsController : ControllerBase
{
    private ILogger Logger { get; }


    public GraphsController(ILogger<GraphsController> logger)
    {
        Logger = logger;
    }
}