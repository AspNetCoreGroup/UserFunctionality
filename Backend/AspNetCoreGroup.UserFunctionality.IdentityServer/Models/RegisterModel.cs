using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreGroup.UserFunctionality.IdentityServer.Models
{
	public class RegisterModel
	{
		[Required]
		[Display(Name = "Имя пользователя")]
		[MaxLength(15)]
		public string UserName { get; set; }

		[EmailAddress]
		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Пароль")]
		public string Password { get; set; }

		[Display(Name = "Аккаунт телеграмм")]
		public string? Telegramm { get; set; }

		[Required]
		[Display(Name = "Зарегистрироваться как админ")]
		public bool IsAdmin { get; set; }
	}
}

