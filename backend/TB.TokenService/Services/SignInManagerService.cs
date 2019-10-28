using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TB.Entities;

namespace TB.TokenService.Services
{
    public class SignInManagerService : ISignInManagerService
    {
        private readonly SignInManager<User> _signInManager;

        public SignInManagerService(SignInManager<User> signInManager)
        {
            _signInManager = signInManager;
        }

        public Task<SignInResult> PasswordSignInAsync(string userName, string password, bool rememberLogin)
        {
            return _signInManager.PasswordSignInAsync(userName, password, rememberLogin, lockoutOnFailure: true);
        }

        public Task SignOutAsync()
        {
            return _signInManager.SignOutAsync();
        }
    }
}
