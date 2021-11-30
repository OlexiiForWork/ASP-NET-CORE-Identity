﻿using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Configuration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                //new IdentityResources.Profile(),
                //new IdentityResource
                //{
                //    Name = "rc.scope",
                //    UserClaims =
                //    {
                //        "rc.garndma"
                //    }
                //}
            };
        public static IEnumerable<ApiScope> GetScopes() =>
                new List<ApiScope> {
                        new ApiScope("ApiOne"),//Это тоже добавлено для минимальной проверки
                        new ApiScope(IdentityServerConstants.StandardScopes.OpenId),//Это тоже добавлено для минимальной проверки
                        new ApiScope(IdentityServerConstants.StandardScopes.Profile),//Это тоже добавлено для минимальной проверки
                       
                //new ApiResource("ApiTwo"),
                };
        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> {
                new ApiResource("ApiOne") { 
                    Scopes = new [] { "ApiOne" } //Добавлен для доп проверки
                },
                new ApiResource("ApiTwo"),
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client> {
                new Client {
                    ClientId = "client_id",
                    ClientSecrets = { new Secret("client_secret".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    AllowedScopes = { 
                           "ApiOne" 
                       // , "ApiTwo" 
                    }
                },
                new Client {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    RedirectUris = { "https://localhost:44322/signin-oidc" },
                    //PostLogoutRedirectUris = { "https://localhost:44322/Home/Index" },

                    AllowedScopes = {
                        "ApiOne",
                        "ApiTwo",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        //"rc.scope",
                    },

                    // puts all the claims in the id token
                    //AlwaysIncludeUserClaimsInIdToken = true,
                    //AllowOfflineAccess = true,
                    //RequireConsent = false,
                },
            };
    }
}
