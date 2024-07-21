using System.ComponentModel.DataAnnotations;

namespace AspNetCoreGroup.UserFunctionality.IdentityServer.Models
{
    public class UpdateClaimsModel
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Telegramm { get; set; }
    }
}
