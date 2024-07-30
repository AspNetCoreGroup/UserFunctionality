using CommonLibrary.Interfaces.Services;
using ModelLibrary.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private ILogger Logger { get; }

    private IUsersService UsersService { get; set; }


    public UsersController(ILoggerFactory loggerFactory, IUsersService usersService)
    {
        Logger = loggerFactory.CreateLogger<UsersController>();
        UsersService = usersService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var user = await UsersService.GetUsersAsync();

        return Ok(user);
    }

    [HttpGet("{userID}")]
    public async Task<IActionResult> GetAsync(int userID)
    {
        var users = await UsersService.GetUserAsync(userID);

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(UserDto user)
    {
        await UsersService.CreateUserAsync(user);

        return Ok();
    }

    [HttpPut("{userID}")]
    public async Task<IActionResult> UpdateAsync(UserDto user, int userID)
    {
        await UsersService.UpdateUserAsync(userID, user);

        return Ok();
    }

    [HttpDelete("{userID}")]
    public async Task<IActionResult> DeleteAsync(int userID)
    {
        await UsersService.DeleteUserAsync(userID);

        return Ok();
    }
}