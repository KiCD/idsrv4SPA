using IdentityServer4.Models;

namespace TB.TokenService.ViewModels
{
    public class ErrorViewModel
    {
        public ErrorMessage Error { get; set; }
        public string RequestId { get; set; }
        public string ErrorId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}