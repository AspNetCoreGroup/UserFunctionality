using System;
using AspNetCoreGroup.UserFunctionality.IdentityServer.Models;
using AutoMapper;

namespace AspNetCoreGroup.UserFunctionality.IdentityServer.Mapping
{
	public class LoginProfile : Profile
	{
		public LoginProfile()
		{
            CreateMap<LoginModel, UserDTO>();
        }
	}
}

