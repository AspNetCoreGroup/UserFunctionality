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
            return BadRequest();

        return Ok(user);
    }

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

                var token = Token(claims);
                await _userManager.SetAuthenticationTokenAsync(userDTO, "IS4", userDTO.Email + "_token", token);
                Response.Cookies.Append("token", token, new CookieOptions { HttpOnly = false });
                
                return Created(userDTO.Email, userDTO);
            }
            else
            {
                return BadRequest(model);
            }
        }
        
        return BadRequest(model);
    }

    [HttpPatch("/api/users/Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginModel model)
    {
        if (ModelState.IsValid)
        {
            var dto = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.Equals(model.Email));
            var res = await _signInManager.PasswordSignInAsync(dto, model.Password, model.RememberMe, false);
            
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

    [HttpPatch("/api/users/Logout")]
    public async Task<IActionResult> LogoutAsync([DataType(DataType.EmailAddress), Required] string? email)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(email));

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
            return BadRequest();
    }

    [HttpGet("api/users/IsSignedIn")]
    public async Task<IActionResult> IsSignedInAsync()
    {
        return Ok(_signInManager.IsSignedIn(HttpContext.User));
    }

    [HttpPost("api/users/Token")]
    public string Token(IEnumerable<Claim> claims)
    {
        var jwt = new JwtSecurityToken(
            issuer: "Server",
            audience: "Client",
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysupersecret_secretsecretsecretkey!123")),
                SecurityAlgorithms.HmacSha256)
            );
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}