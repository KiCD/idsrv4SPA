using TB.TokenService.Models;

namespace TB.TokenService.ViewModels
{
    public class LoginViewModel : LoginInputModel
    {
        public bool AllowRememberLogin { get; set; }
        public bool EnableLocalLogin { get; set; }
    }
}
