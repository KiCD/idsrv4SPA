using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TB.TokenService.Services
{
    public interface ISignInManagerService
    {
        Task<SignInResult> PasswordSignInAsync(string username, string password, bool rememberLogin);
        Task SignOutAsync();
    }
}
