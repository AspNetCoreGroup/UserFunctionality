using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AspNetCoreGroup.UserFunctionality.IdentityServer.Models;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authentication;

namespace AspNetCoreGroup.UserFunctionality.IdentityServer;

public class AuthorizationController : Controller
{
    public static class TokenProps
    {
        public static string LoginProvider = "IS4";
        public static string TokenName = "token";
        public static string TokenProvider = "IS4";
        public static string Purpose = "IS4";
    }

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
        var token = await _userManager.GenerateUserTokenAsync(userDTO, TokenProps.TokenProvider, TokenProps.Purpose);//Token(claims);
        var tokenJwt = Token(claims);

        await _userManager.SetAuthenticationTokenAsync(userDTO, TokenProps.LoginProvider, userDTO.Email + "_token", token);
        
        Response.Cookies.Append(
            "token",
            tokenJwt,
            new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.None,
                Expires = DateTime.Now.AddYears(1),
                Secure = true,
                IsEssential = true
            });
        Response.Cookies.Append(
            ".AspNetCore.Token",
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
                var userIdClaim = new Claim("UserID", userDTO.Id);

                var claims = new[] { adminClaim, tgClaim, emailClaim, userIdClaim };

                await _userManager.AddClaimsAsync(
                    userDTO,
                    claims
                    );

                userDTO.IsSignedIn = true;
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
                dto.IsSignedIn = true;
                await _userManager.UpdateAsync(dto);
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
    public async Task<IActionResult> LogoutAsync()
    {
        Response.Cookies.Delete("token");
        var claimsPrincipal = _signInManager.Context.User;
        var user = await _signInManager.UserManager.GetUserAsync(claimsPrincipal);

        if (user == null)
        {
            return BadRequest();
        }

        user.IsSignedIn = false;
        await _userManager.UpdateAsync(user);

        await _signInManager.SignOutAsync();
        
        return Ok(user);
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

        return Ok((await _signInManager.CanSignInAsync(dto) && !dto.IsSignedIn));
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

    [HttpPatch("api/users/Claims/Update")]
    public async Task<IActionResult> UpdateClaimsAsync([FromBody] UpdateClaimsModel model)
    {
        var user = _userManager.Users.FirstOrDefault(u => u.Email.Equals(model.Email));

        if (user == null)
        {
            return BadRequest(model);
        }

        var claims = await _userManager.GetClaimsAsync(user);
        var tgClaim = claims.FirstOrDefault(c => c.Type.Equals(Claims.TG.ToString()));

        if (tgClaim.Value.Equals(model.Telegramm))
        {
            return Ok();
        }

        user.Telegramm = model.Telegramm;

        await _userManager.UpdateAsync(user);
        await _userManager.ReplaceClaimAsync(
            user,
            tgClaim,
            new Claim(Claims.TG.ToString(), model.Telegramm)
            );
        await SetTokenCookieAsync(claims, user);


        return Ok();
    }

    [HttpGet("api/users/CheckToken")]
    public async Task<IActionResult> CheckUserTokenAsync([FromQuery, Required]string token)
    {
        var userToken = await _dbContext.UserTokens.FirstOrDefaultAsync(t => t.Value.Equals(System.Web.HttpUtility.UrlDecode(token)));

        if (userToken == null)
        {
            return BadRequest(new { Message = "Invalid token"} );
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id.Equals(userToken.UserId));

        if (user == null)
        {
            return BadRequest(new { Message = "User not found" });
        }


        return Ok(new 
        { 
            Username = user.UserName,
            Email = user.Email,
            Telegramm = user.Telegramm,
            UserID = user.Id
        });
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