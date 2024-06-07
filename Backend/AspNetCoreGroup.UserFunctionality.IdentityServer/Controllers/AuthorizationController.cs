using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreGroup.UserFunctionality.IdentityServer.Models;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace AspNetCoreGroup.UserFunctionality.IdentityServer;

public class AuthorizationController : Controller
{
    private readonly UserManager<UserDTO> _userManager;
    private readonly SignInManager<UserDTO> _signInManager;
    private readonly AppDbContext _dbContext;
    private readonly IMapper _mapper;

    public AuthorizationController(
        UserManager<UserDTO> userManager,
        SignInManager<UserDTO> signInManager,
        AppDbContext dbContext,
        IMapper mapper
        )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    [HttpGet("api/users")]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(_dbContext.Users);
    }

    [HttpGet("/api/users/{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return BadRequest($"No user with id {id} was found");

        return Ok(user);
    }

    [HttpPost("/api/users/Register")]
    public async Task<IActionResult> RegisterAsync(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest($"Email was not provided");
            else if (await _userManager.Users.AnyAsync(u => u.Email.Equals(model.Email)))
                return BadRequest($"User with email {model.Email} already exists");

            var userDTO = _mapper.Map<UserDTO>(model);

            var res = await _userManager.CreateAsync(userDTO, model.Password);

            if (res.Succeeded)
            {
                var adminClaim = new Claim("IsAdmin", "true");
                var tgClaim = new Claim("TG", model.Telegramm);
                var emailClaim = new Claim("Email", model.Email);

                await _userManager.AddClaimsAsync(
                    userDTO,
                    new[] { adminClaim, tgClaim, emailClaim }
                    );

                await _signInManager.SignInAsync(userDTO, false);
                try
                {
                    return Ok($"Successfully registered: {res}");
                }
                catch
                {
                    return Ok($"Successfully registered: {res}");
                }
            }
            else
            {
                return BadRequest($"Errors have ocurred: {string.Join("; ", res.Errors.Select(e => e.Description))}");
            }
        }
        
        return BadRequest("Bad model");
    }

    [HttpPatch("/api/users/Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var dto = _mapper.Map<UserDTO>(model);
            var res = await _signInManager.PasswordSignInAsync(dto, model.Password, model.RememberMe, false);
            
            if (res.Succeeded)
            {
                return Ok($"Login successful {model.Email}: {res}");
            }
            else
            {
                return BadRequest($"An error has been ocurred. result: {res}");
            }
        }
        
        return BadRequest("Bad model");
    }

    [HttpPatch("/api/users/Logout")]
    public async Task<IActionResult> LogoutAsync([DataType(DataType.EmailAddress), Required] string? email)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(email));

        if (user == null)
        {
            return BadRequest($"No user with email {email}");
        }
        else
        {
            await _signInManager.SignOutAsync();

            return Ok($"Signed out user {email}");
        }
    }

    [HttpPost("api/users/Delete")]
    public async Task<IActionResult> DeleteAsync(string email)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(email));

        if (user == null)
        {
            return BadRequest($"No user with email {email}");
        }

        var res = await _userManager.DeleteAsync(user);

        if (res.Succeeded)
        {
            return Ok($"Successfully deleted user {email}");
        }
        else
        {
            return Problem($"Cant delete user {email}");
        }
    }

    [HttpGet("/api/users/CanSignIn")]
    public async Task<IActionResult> IsAuthorizedAsync(LoginModel model)
    {
        var dto = _mapper.Map<UserDTO>(model);

        return Ok(await _signInManager.CanSignInAsync(dto));
    }

    [HttpGet("api/users/Claims")]
    public async Task<IActionResult> ClaimsAsync([DataType(DataType.EmailAddress), Required] string? email)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(email));

        if (user != null)
            return Ok(await _userManager.GetClaimsAsync(user));
        else
            return (BadRequest($"No user found with email {email}"));
    }

    [HttpGet("api/users/IsSignedIn")]
    public async Task<IActionResult> IsSignedInAsync()
    {
        return Ok(_signInManager.IsSignedIn(HttpContext.User));
    }
}