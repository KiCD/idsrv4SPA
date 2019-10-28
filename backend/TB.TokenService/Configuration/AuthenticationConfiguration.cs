using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TB.TokenService.Configuration
{
    public class AuthenticationConfiguration
    {
        public const int AspNetDefaultPeristentCookieLifetimeInDays = 14;
        public const bool AspNetDefaultCookieSlidingExpiration = true;
        public const int AspNetDefaultSecurityStampValidationIntervalInMinutes = 30;
        public static int SecurityStampValidationIntervalInMinutes = 10;
        public const int TokenLifetimeInSeconds = 600;
        public const int CookieLifetimeInMinutes = 30;
        public const bool CookieSlidingExpiration = AspNetDefaultCookieSlidingExpiration;
        public const bool CookiePersistent = false;
        public const int PeristentCookieLifetimeInDays = AspNetDefaultPeristentCookieLifetimeInDays;

        public static AuthenticationProperties CreateAuthenticationProperties()
        {
            return new AuthenticationProperties()
            {
                IsPersistent = AuthenticationConfiguration.CookiePersistent,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(AuthenticationConfiguration.CookieLifetimeInMinutes)
            };
        }
    }
}
