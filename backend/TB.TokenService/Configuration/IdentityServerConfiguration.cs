using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using TB.TokenService.Configuration;

namespace TB.TokenService.Identity
{
    public static class IdentityServerConfiguration
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("documentapi", "Document API")
            };
        }

        public static IEnumerable<Client> GetClients(Dictionary<string, ClientUriConfiguration> uriConfiguration)
        {
            var clientList = new List<Client>
            {
                new Client
                {
                    ClientId = "tbjsclient",
                    ClientName = "Angular 8 Client",
                    AllowedGrantTypes = GrantTypes.Code,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "openid","profile","documentapi" },
                    RequirePkce = true,
                    RequireConsent = false,
                    RequireClientSecret = false,
                    AllowAccessTokensViaBrowser = true

                }
            };
            foreach (var client in clientList)
            {
                if (!uriConfiguration.ContainsKey(client.ClientId)) continue;
                var uriConfig = uriConfiguration[client.ClientId];
                client.AllowedCorsOrigins = !string.IsNullOrWhiteSpace(uriConfig.Base)
                    ? new List<string>() { uriConfig.Base }
                    : null;
                client.RedirectUris = new List<string>() { uriConfig.Base};
                if (!string.IsNullOrWhiteSpace(uriConfig.PostLogin))
                {
                    client.RedirectUris.Add(uriConfig.PostLogin);
                }
                if (!string.IsNullOrWhiteSpace(uriConfig.SilentRenew))
                {
                    client.RedirectUris.Add(uriConfig.SilentRenew);
                }
                if (!string.IsNullOrWhiteSpace(uriConfig.PostLogout))
                {
                    client.PostLogoutRedirectUris = new List<string>() { uriConfig.PostLogout };
                }
            }
            return clientList;
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResources.OpenId()
            };
        }
    }
}
