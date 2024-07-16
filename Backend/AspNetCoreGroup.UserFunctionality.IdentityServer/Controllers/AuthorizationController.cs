using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreGroup.UserFunctionality.IdentityServer.Models;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using System.Collections;

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

    /// <summary>
    /// Get all users in DB
    /// </summary>
    /// <returns></returns>
    [HttpGet("api/users")]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(_dbContext.Users);
    }

    /// <summary>
    /// Get user corresponding particular id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("/api/users/{id}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return BadRequest();

        return Ok(user);
    }

    /// <summary>
    /// Method which sets JWT token cookie via Set-Cookie header
    /// </summary>
    /// <param name="claims">User Claims collection</param>
    /// <param name="userDTO">User DTO</param>
    private async Task SetTokenCookieAsync(IEnumerable<Claim> claims, UserDTO userDTO)
    {
        var token = Token(claims);
        await _userManager.SetAuthenticationTokenAsync(userDTO, "IS4", userDTO.Email + "_token", token);
        Response.Cookies.Append(
            "token",
            token,
            new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddYears(1),
                Secure = true,
                IsEssential = true
            });
    }

    /// <summary>
    /// Register new user
    /// </summary>
    /// <param name="model">Registration Model</param>
    /// <returns></returns>
    [HttpPost("/api/users/Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(model);
            else if (await _userManager.Users.AnyAsync(u => u.Email.Equals(model.Email)))
                return BadRequest(model);

            var userDTO = _mapper.Map<UserDTO>(model);

            var res = await _userManager.CreateAsync(userDTO, model.Password);

            if (res.Succeeded)
            {
                var adminClaim = new Claim("IsAdmin", "true");
                var tgClaim = new Claim("TG", model.Telegramm);
                var emailClaim = new Claim("Email", model.Email);

                var claims = new[] { adminClaim, tgClaim, emailClaim };

                await _userManager.AddClaimsAsync(
                    userDTO,
                    claims
                    );

                await _signInManager.SignInAsync(userDTO, true);
                await SetTokenCookieAsync(claims, userDTO);
                
                return Created();
            }
            else
            {
                return BadRequest(model);
            }
        }
        
        return BadRequest(model);
    }

    /// <summary>
    /// Login via email and password. Get JWT
    /// </summary>
    /// <param name="model">Login Model</param>
    /// <returns></returns>
    [HttpPatch("/api/users/Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var dto = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.Equals(model.Email));
            var res = await _signInManager.PasswordSignInAsync(dto, model.Password, model.RememberMe, false);

            var claims = await _userManager.GetClaimsAsync(dto);
            await SetTokenCookieAsync(claims, dto);
            
            if (res.Succeeded)
            {
                return Ok(dto);
            }
            else
            {
                return BadRequest(dto);
            }
        }
        
        return BadRequest(model);
    }

    /// <summary>
    /// Logout from service
    /// </summary>
    /// <param name="email">User to face logout</param>
    /// <returns></returns>
    [HttpPatch("/api/users/Logout")]
    public async Task<IActionResult> LogoutAsync([DataType(DataType.EmailAddress), Required, FromQuery] string? email)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(email));
        Response.Cookies.Delete("token");

        if (user == null)
        {
            return BadRequest();
        }
        else
        {
            await _signInManager.SignOutAsync();

            return Ok();
        }
    }

    /// <summary>
    /// Delete the user entity from DB
    /// </summary>
    /// <param name="email">Email of user to be deleted</param>
    /// <returns></returns>
    [HttpPost("api/users/Delete")]
    public async Task<IActionResult> DeleteAsync(string email)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(email));

        if (user == null)
        {
            return BadRequest();
        }

        var res = await _userManager.DeleteAsync(user);

        if (res.Succeeded)
        {
            return Ok();
        }
        else
        {
            return Problem();
        }
    }

    /// <summary>
    /// Drop DataBase
    /// </summary>
    /// <returns></returns>
    [HttpPost("api/users/DropDB")]
    public async Task<IActionResult> DropAsync()
    {
        bool? isSuccess = true;
        foreach (var user in _userManager.Users)
        {
            var res = await _userManager.DeleteAsync(user);
            isSuccess &= res?.Succeeded;
        }

        if (isSuccess.Value)
        {
            return Ok();
        }
        else
        {
            return Problem();
        }
    }

    /// <summary>
    /// Check whether user can sign in
    /// </summary>
    /// <param name="model">Login Model</param>
    /// <returns></returns>
    [HttpGet("/api/users/CanSignIn")]
    public async Task<IActionResult> IsAuthorizedAsync(LoginModel model)
    {
        var dto = _mapper.Map<UserDTO>(model);

        return Ok(await _signInManager.CanSignInAsync(dto));
    }

    /// <summary>
    /// Get claims of user
    /// </summary>
    /// <param name="email">Email of user</param>
    /// <returns></returns>
    [HttpGet("api/users/Claims")]
    public async Task<IActionResult> ClaimsAsync([DataType(DataType.EmailAddress), Required] string? email)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(email));

        if (user != null)
            return Ok(await _userManager.GetClaimsAsync(user));
        else
            return BadRequest();
    }

    /// <summary>
    /// Check whether the user is signed in
    /// </summary>
    /// <returns></returns>
    [HttpGet("api/users/IsSignedIn")]
    public async Task<IActionResult> IsSignedInAsync()
    {
        return Ok(_signInManager.IsSignedIn(HttpContext.User));
    }

    /// <summary>
    /// Get JWT
    /// </summary>
    /// <param name="claims">Claims to be set into JWT</param>
    /// <returns></returns>
    [HttpPost("api/users/Token")]
    public string Token(IEnumerable<Claim> claims)
    {
        var jwt = new JwtSecurityToken(
            issuer: "Server",
            audience: "Client",
            claims: claims,
            expires: DateTime.Now.AddYears(100),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysupersecret_secretsecretsecretkey!123")),
                SecurityAlgorithms.HmacSha256)
            );
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}