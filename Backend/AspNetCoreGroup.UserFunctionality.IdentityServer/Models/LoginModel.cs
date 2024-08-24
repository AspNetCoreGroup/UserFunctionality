using System;
using System.ComponentModel.DataAnnotations;
namespace AspNetCoreGroup.UserFunctionality.IdentityServer.Models
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Fill email!")]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Fill password")]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		public bool RememberMe { get; set; }
    }
}

