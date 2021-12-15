using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServer
{
    public static class Configuration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>//Добавление до идентити токена
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "rc.scope",
                    UserClaims =
                    {
                        "rc.garndma"
                    }
                }
            };
        public static IEnumerable<ApiScope> GetScopes() =>
                new List<ApiScope> {
                        new ApiScope("ApiOneScopes"),//Это тоже добавлено для минимальной проверки
                        new ApiScope("ApiTwoScopes", new string[] { "rc.api.garndma" }),//Это тоже добавлено для минимальной проверки
                        new ApiScope(IdentityServerConstants.StandardScopes.OpenId),//Это тоже добавлено для минимальной проверки
                        new ApiScope(IdentityServerConstants.StandardScopes.Profile),//Это тоже добавлено для минимальной проверки
                        new ApiScope("rc.scope",new List<string> {"rc.garndma"}),//Это тоже добавлено для минимальной проверки
                //new ApiResource("ApiTwo"),
                };
        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource> {//добавление к AcssesToken
                new ApiResource("ApiOne"/*Audience имя ресурса в  */) { 
                    Scopes = new [] { "ApiOneScopes" } //Добавлен для доп проверки
                },
                new ApiResource("ApiTwo"){
                    Scopes = new [] { "ApiTwoScopes" } //Добавлен для доп проверки
                },
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client> {
                new Client {
                    ClientId = "client_id",
                    ClientSecrets = { new Secret("client_secret".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,//это машина к машыне

                    AllowedScopes = {
                           "ApiOneScopes" 
                       // , "ApiTwo" 
                    }
                },
                new Client {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },

                    AllowedGrantTypes = GrantTypes.Code,//юзер участвует в комуникации
                    //RequirePkce = true,

                    RedirectUris = { "https://localhost:44322/signin-oidc" },
                    //PostLogoutRedirectUris = { "https://localhost:44322/Home/Index" },

                    AllowedScopes = {
                        "ApiOneScopes",
                        "ApiTwoScopes",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "rc.scope",
                    },

                    // puts all the claims in the id token
                    //AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,//Взять клеймы и включить в IdToken
                    //AllowOfflineAccess = true,
                    RequireConsent = false,//это для добавления какихто данных с фейсбука (нужно только для участия узера в true)
                },
            };
    }
}
