using System;
using System.Text;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Microsoft.IdentityModel.Tokens;

namespace AspNetCoreGroup.UserFunctionality.IdentityServer
{
	public static class Config
	{
        public static IEnumerable<Client> GetClients(WebApplicationBuilder builder)
        {
            return new[]
            {
                new Client
                {
                    ClientId = "react", 
                    ClientName = "React Client", 
                    ClientUri = builder.Configuration.GetValue<string>("IdentityServer:ClientUri"), 
                    AllowedGrantTypes = GrantTypes.Implicit, 
                    RequireClientSecret = false,
                    RequireConsent = false,
                    RedirectUris =
                    {
                        builder.Configuration.GetValue<string>("IdentityServer:ClientUri"),
                    },
                    PostLogoutRedirectUris = { builder.Configuration.GetValue<string>("IdentityServer:ClientUri") }, 
                    AllowedCorsOrigins = { builder.Configuration.GetValue<string>("IdentityServer:ClientUri") },
                    AllowedScopes =
                    {
                       IdentityServerConstants.StandardScopes.OpenId,
                       IdentityServerConstants.StandardScopes.Profile,
                       "authorization"
                    },
                    AllowAccessTokensViaBrowser = true
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("authorization", "authorization")
            };
        }
    }

    public static class AuthOptions
    {
        public static bool ValidateIssuer = true;
        public static string ValidIssuer = "IS4";
        public static bool ValidateAudience = true;
        public static string ValidAudience = "Client";
        public static bool ValidateLifetime = false;
        public static SymmetricSecurityKey IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret"));
        public static bool ValidateIssuerSigningKey = true;
    }
}

