using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using TB.TokenService.Models;
using TB.TokenService.Security;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TB.TokenService.Controllers
{

    [SecurityHeaders]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IConfiguration _configuration;
        public HomeController(IIdentityServerInteractionService interaction, IConfiguration configuration)
        {
            _interaction = interaction;
            _configuration = configuration;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("~/account/login");
            }

            var jsClientUrl = _configuration.GetValue<string>("TB_JS_CLIENT_URL");
            if (!string.IsNullOrEmpty(jsClientUrl))
            {
                return Redirect(jsClientUrl);
            }

            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel { ErrorId = errorId ?? Guid.NewGuid().ToString() };

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }
    }
}
