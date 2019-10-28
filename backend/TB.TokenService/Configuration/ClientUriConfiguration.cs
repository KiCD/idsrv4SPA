using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TB.TokenService.Configuration
{
    public class ClientUriConfiguration
    {
        public string Base { get; set; }
        public string PostLogout { get; set; }
        public string PostLogin { get; set; }
        public string SilentRenew { get; set; }
    }
}
