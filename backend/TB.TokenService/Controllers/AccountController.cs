using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TB.Entities;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TB.TokenService.Models;
using TB.TokenService.Security;
using TB.TokenService.ViewModels;
using IdentityServer4.Events;
using TB.TokenService.Services;
using Microsoft.AspNetCore.Authentication;
using TB.TokenService.Configuration;
using System.Security.Claims;
using ClaimTypes = Common.Infrastructure.Configuration.ClaimTypes;
using Microsoft.AspNetCore.Http;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityModel;

namespace TB.TokenService.Controllers
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly UserManager<User> _userManager;
        private readonly ISignInManagerService _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AccountController> _logger;
        public AccountController(
            IIdentityServerInteractionService interaction, 
            IEventService eventService,
            UserManager<User> userManager,
            ISignInManagerService signInManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AccountController> logger)
        {
            _interaction = interaction;
            _events = eventService;
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {

            // build a model so we know what to show on the login page
            var vm = await BuildLoginViewModelAsync(returnUrl);

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid login model {model.Username}. Errors: {string.Join('.', ModelState.Values.Select(item => item.Errors))}.");
                var vm = await BuildLoginViewModelAsync(model);
                return View(vm);
            }

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
                ModelState.AddModelError("InvalidCredentials", BuildStringFromResource("InvalidCredentials"));
                var vm = await BuildLoginViewModelAsync(model);
                return View(vm);
            }
            var previousFailedAttempts = user.AccessFailedCount;
            var failedAttempts = previousFailedAttempts + 1;
            var password =
               await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberLogin);
            if (!password.Succeeded)
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials"));
                _logger.LogInformation($"Login failed for user {model.Username} at {DateTime.Now}. " +
                                       $"User agent is {HttpContext.Request.Headers["User-Agent"]}." +
                                       $"User entered the wrong password {failedAttempts} times.");
                var vm = await BuildLoginViewModelAsync(model);
                return View(vm);
            }
            if (user.LockoutEnd.HasValue && DateTimeOffset.Now < user.LockoutEnd.Value)
            {
                await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "account is locked"));
                ModelState.AddModelError("InvalidCredentials", BuildStringFromResource("AccountLocked"));
                var vm = await BuildLoginViewModelAsync(model);
                return View(vm);
            }

            AuthenticationProperties props = new AuthenticationProperties()
            {
                IsPersistent = AuthenticationConfiguration.CookiePersistent,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(AuthenticationConfiguration.CookieLifetimeInMinutes)
            };
            Claim claim = new Claim(ClaimTypes.SecurityStampClaim, user.SecurityStamp);

            await _events.RaiseAsync(
                new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName));
            await HttpContext.SignInAsync(user.Id.ToString(), user.UserName, props, claim);

            _logger.LogInformation(
                $"Login successful for user {user.UserName} at {DateTime.Now}. User agent is {HttpContext.Request.Headers["User-Agent"]}.");


            // make sure the returnUrl is still valid, and if yes - redirect back to authorize endpoint or a local page
            if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect("~/");
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.UserId == 0)
            {
                _logger.LogError("Invalid user id for reset password");
                ModelState.AddModelError("", "Invalid user id");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                _logger.LogError($"User with id {model.UserId} not found for reset password.");
                ModelState.AddModelError("","Invalid user id");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result != null && result.Errors.Any(item => item.Code.Contains("Password")))
            {
                foreach (var error in result.Errors.Where(item => item.Code.Contains("Password")))
                {
                    ModelState.AddModelError("Password", error.Description);
                }

                return View(model);
            }

            if (result == null)
            {
                ModelState.AddModelError("", BuildStringFromResource("ResetPasswordAuthorizeAndValidateFailed"));
                return View();
            }

            if (result.Succeeded)
            {
                _logger.LogInformation($"Reset password successful for user {user.UserName}.");
                return ResetPasswordConfirmation();
            }

            _logger.LogError($"Reset password failed for user {user.UserName}.");
            if (result.Errors != null && result.Errors.Any())
            {
                _logger.LogError($"Reset password errors {string.Join('.', result.Errors.Select(item => item.Code))}");
            }
            ModelState.AddModelError("", BuildStringFromResource("ResetPasswordAuthorizeAndValidateFailed"));
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        private IActionResult ResetPasswordConfirmation()
        {
            return View("ResetPasswordConfirmation");
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // no need to show prompt
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);
            if (vm.TriggerExternalSignout)
            {
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.SignOutAsync(vm.ExternalAuthenticationScheme,
                        new AuthenticationProperties { RedirectUri = url });
                }
                catch (NotSupportedException nex) // this is for the external providers that don't have signout
                {
                    _logger.LogError($"Error signing out from external provider for logout id {vm.LogoutId}.", nex);
                }
                catch (InvalidOperationException iex) // this is for Windows/Negotiate
                {
                    _logger.LogError($"Error signing out from external provider for logout id {vm.LogoutId}.", iex);
                }
            }

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await HttpContext.SignOutAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme);
                await _signInManager.SignOutAsync();
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            return View("LoggedOut", vm);
        }

        private string BuildStringFromResource(string resourceId)
        {
            return resourceId;
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null)
            {
                // this is meant to short circuit the UI and only trigger the one external IdP
                return new LoginViewModel
                {
                    EnableLocalLogin = false,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint
                };
            }

            return new LoginViewModel
            {
                AllowRememberLogin = true,
                EnableLocalLogin = true,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint
            };
        }
        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            vm.AllowRememberLogin = true;
            vm.ReturnUrl = model.ReturnUrl;
            return vm;
        }
        public async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = true,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };

            var user = _httpContextAccessor.HttpContext.User;
            if (user != null)
            {
                var idp = user.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServer4.IdentityServerConstants.LocalIdentityProvider)
                {
                    if (vm.LogoutId == null)
                    {
                        // if there's no current logout context, we need to create one
                        // this captures necessary info from the current logged in user
                        // before we signout and redirect away to the external IdP for signout
                        vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                    }

                    vm.ExternalAuthenticationScheme = idp;
                }
            }

            return vm;
        }

        public async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            if (logoutId == null)
            {
                logoutId = await _interaction.CreateLogoutContextAsync();
            }
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = false };

            var user = _httpContextAccessor.HttpContext.User;
            if (user == null || user.Identity.IsAuthenticated == false)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }
    }
}