using Microsoft.AspNetCore.Identity;

namespace AspNetCoreGroup.UserFunctionality.IdentityServer;

public class UserDTO : IdentityUser
{
    public string? Telegramm { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsSignedIn { get; set; }

}
