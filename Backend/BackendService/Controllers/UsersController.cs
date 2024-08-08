using BackendCommonLibrary.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using ModelLibrary.Model;

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
        //throw new Exception("Добавление пользователей доступно только через API авторизации.");

        ///*
        await UsersService.CreateUserAsync(user);

        return Ok();
        //*/
    }

    [HttpPut("{userID}")]
    public Task<IActionResult> UpdateAsync(UserDto user, int userID)
    {
        throw new Exception("Изменение пользователей доступно только через API авторизации.");

        /*
        await UsersService.UpdateUserAsync(userID, user);

        return Ok();
        */
    }

    [HttpDelete("{userID}")]
    public Task<IActionResult> DeleteAsync(int userID)
    {
        throw new Exception("Удаление пользователей доступно только через API авторизации.");

        /*
        await UsersService.DeleteUserAsync(userID);

        return Ok();
        */
    }
}