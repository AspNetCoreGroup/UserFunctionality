using System;
using AspNetCoreGroup.UserFunctionality.IdentityServer.Models;
using AutoMapper;

namespace AspNetCoreGroup.UserFunctionality.IdentityServer.Mapping
{
	public class RegisterProfile : Profile
	{
		public RegisterProfile()
		{
			CreateMap<RegisterModel, UserDTO>();
		}
	}
}

