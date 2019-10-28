using IdentityServer4.Models;
using System;

namespace TB.TokenService.Models
{
    public class ErrorViewModel
    {
        public ErrorMessage Error { get; set; }
        public string RequestId { get; set; }
        public string ErrorId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
